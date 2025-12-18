using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomerSpawner : MonoBehaviour
{
    public static CustomerSpawner Instance;
    public List<Sprite> bodySprites; // Drag your different body shapes here
    public GameObject customerPrefab;
    public Transform spawnPoint;
    public float normalSpawnDelay = 0.5f; 
    public float panicSpawnDelay = 0.0f; 
    [Range(0f, 1f)] public float specialChance = 0.02f; // 2% chance for EACH type
    public List<string> easyOrders = new List<string>();
    public List<string> mediumOrders = new List<string>();
    public List<string> hardOrders = new List<string>();

    [Header("Runtime State")]
    public Customer currentCustomer;
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

        float delay = normalSpawnDelay;

        if (timeRemaining <= 6.0f)
        {
            delay = panicSpawnDelay;
        }
        else if (forceInstantSpawn)
        {
            delay = 0f;
            forceInstantSpawn = false; 
        }

        yield return new WaitForSeconds(delay);

        List<string> selectedList = new List<string>();
        int basePoints = 150;

        if (timeRemaining > 14.0f)
        {
            selectedList = easyOrders;
            basePoints = 200; 
        }
        else if (timeRemaining > 7.0f) 
        {
            selectedList = mediumOrders;
            basePoints = 550; 
        }
        else 
        {
            selectedList = hardOrders;
            basePoints = 850; 
        }

        if (selectedList == null || selectedList.Count == 0) selectedList = easyOrders;

        string finalOrder = selectedList[Random.Range(0, selectedList.Count)];
        bool isSpecialVisual = false; 

        float roll = Random.value; 

        if (roll < specialChance) 
        {
            basePoints *= 2;
            finalOrder = "Air Kosong"; 
            isSpecialVisual = true; 
            Debug.Log("Spawned: Tok Abah!");
        }
        else if (roll < (specialChance * 2))
        {
            basePoints += 100;
            isSpecialVisual = true;
            Debug.Log("Spawned: Mat Salleh!");
        }
        else if (roll < (specialChance * 3))
        {
            forceInstantSpawn = true;
            Debug.Log("Spawned: Student Kid (Rush incoming!)");
        }

        if (customerPrefab != null && spawnPoint != null)
        {
            GameObject newObj = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
            currentCustomer = newObj.GetComponent<Customer>();
            
            currentCustomer.SetupCustomer(finalOrder, basePoints, isSpecialVisual);

            if (bodySprites.Count > 0 && currentCustomer.bodyRenderer != null && !isSpecialVisual)
            {
                Sprite randomBody = bodySprites[Random.Range(0, bodySprites.Count)];
                currentCustomer.bodyRenderer.sprite = randomBody;
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