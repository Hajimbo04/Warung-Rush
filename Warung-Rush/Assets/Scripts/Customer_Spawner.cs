using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomerSpawner : MonoBehaviour
{
    public static CustomerSpawner Instance;

    [Header("Visuals")]
    public List<Sprite> bodySprites; // Drag your different body shapes here

    [Header("Settings")]
    public GameObject customerPrefab;
    public Transform spawnPoint;

    [Header("Pacing Settings")]
    public float normalSpawnDelay = 0.5f; 
    public float panicSpawnDelay = 0.0f; 

    [Header("Special Customer Chances")]
    [Range(0f, 1f)] public float specialChance = 0.02f; // 2% chance for EACH type

    [Header("Menu Data")]
    public List<string> easyOrders = new List<string>();
    public List<string> mediumOrders = new List<string>();
    public List<string> hardOrders = new List<string>();

    [Header("Runtime State")]
    public Customer currentCustomer;

    // Internal flag to track if the NEXT spawn should be instant (Student Kid effect)
    private bool forceInstantSpawn = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void SpawnNextCustomer()
    {
        StartCoroutine(SpawnProcess());
    }

    IEnumerator SpawnProcess()
    {
        float timeRemaining = 0;
        if (GameManager.Instance != null) 
            timeRemaining = GameManager.Instance.currentTime;

        // 1. Calculate Delay
        float delay = normalSpawnDelay;

        // Condition A: Panic Mode (Last 6s) -> Instant
        if (timeRemaining <= 6.0f)
        {
            delay = panicSpawnDelay;
        }
        // Condition B: Student Group Effect -> Instant
        else if (forceInstantSpawn)
        {
            delay = 0f;
            forceInstantSpawn = false; // Reset the flag after using it
        }

        yield return new WaitForSeconds(delay);

        // 2. Determine Difficulty List
        List<string> selectedList = new List<string>();
        int basePoints = 150;

        // --- NEW PRICING LOGIC (IN CENTS) ---
        if (timeRemaining > 14.0f)
        {
            selectedList = easyOrders;
            basePoints = 200; // RM 2.00 (Teh O / Roti)
        }
        else if (timeRemaining > 7.0f) 
        {
            selectedList = mediumOrders;
            basePoints = 550; // RM 5.50 (Mee Goreng / Nasi Lemak)
        }
        else 
        {
            selectedList = hardOrders;
            basePoints = 850; // RM 8.50 (Mamak Special)
        }
        // -------------------------------------

        if (selectedList == null || selectedList.Count == 0) selectedList = easyOrders;

        // 3. Pick Basic Order
        string finalOrder = selectedList[Random.Range(0, selectedList.Count)];
        bool isSpecialVisual = false; // To turn them Gold

        // 4. Roll for SPECIAL CUSTOMERS (The Logic Injection)
        // We use a random roll to see if we get a special type
        float roll = Random.value; 

        if (roll < specialChance) 
        {
            // TYPE 1: TOK ABAH (2x Points, Simple Order)
            // "Cucu, bagi atuk air kosong je."
            basePoints *= 2;
            finalOrder = "Air Kosong"; 
            isSpecialVisual = true; 
            Debug.Log("Spawned: Tok Abah!");
        }
        else if (roll < (specialChance * 2))
        {
            // TYPE 2: MAT SALLEH (+100 Bonus)
            basePoints += 100;
            // Order stays the same (just mispronounced in lore)
            isSpecialVisual = true;
            Debug.Log("Spawned: Mat Salleh!");
        }
        else if (roll < (specialChance * 3))
        {
            // TYPE 3: STUDENT GROUP KID (Next Spawn is Instant)
            // No score bonus, but sets the flag for the NEXT loop
            forceInstantSpawn = true;
            // Maybe make them slightly faster to type? Keep standard order.
            Debug.Log("Spawned: Student Kid (Rush incoming!)");
        }

        // 5. Instantiate
        if (customerPrefab != null && spawnPoint != null)
        {
            GameObject newObj = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
            currentCustomer = newObj.GetComponent<Customer>();
            
            // Pass the modified points and order to the existing system
            currentCustomer.SetupCustomer(finalOrder, basePoints, isSpecialVisual);

            // This block handles setting the sprite and clearing the color for NON-special customers.
            if (bodySprites.Count > 0 && currentCustomer.bodyRenderer != null && !isSpecialVisual)
            {
                // Pick a random body shape
                Sprite randomBody = bodySprites[Random.Range(0, bodySprites.Count)];
                currentCustomer.bodyRenderer.sprite = randomBody;
                
                // Set color back to white (no tinting), removing the old Random.ColorHSV line.
                currentCustomer.bodyRenderer.color = Color.white; 
            }

        }
    }

    public void OnOrderCompleted()
    {
        if (currentCustomer != null)
        {
            currentCustomer.Leave(false);
            currentCustomer = null;
        }
        SpawnNextCustomer();
    }

    public void OnOrderFailed()
    {
        if (currentCustomer != null)
        {
            currentCustomer.Leave(true);
            currentCustomer = null;
        }
        SpawnNextCustomer();
    }
}