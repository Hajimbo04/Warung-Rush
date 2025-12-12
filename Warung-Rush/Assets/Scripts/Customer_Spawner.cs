using UnityEngine;
using System.Collections.Generic;

public class CustomerSpawner : MonoBehaviour
{
    public static CustomerSpawner Instance;

    [Header("Settings")]
    public GameObject customerPrefab;
    public Transform spawnPoint;

    [Header("Menu Data")]
    public List<string> easyOrders = new List<string>();
    public List<string> mediumOrders = new List<string>();
    public List<string> hardOrders = new List<string>();

    [Header("Runtime State")]
    public Customer currentCustomer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void SpawnNextCustomer()
    {
        if (customerPrefab == null || spawnPoint == null) return;

        float time = 0;
        if (GameManager.Instance != null) time = GameManager.Instance.currentTime;

        List<string> selectedList;
        int basePoints = 0;

        // 1. Determine Difficulty & Points
        if (time > 14) 
        {
            selectedList = easyOrders;
            basePoints = 100;
        }
        else if (time > 6) 
        {
            if(Random.value > 0.8f) { selectedList = hardOrders; basePoints = 300; }
            else { selectedList = mediumOrders; basePoints = 200; }
        }
        else 
        {
            // Panic Mode
            if(Random.value > 0.5f) { selectedList = easyOrders; basePoints = 100; }
            else { selectedList = mediumOrders; basePoints = 200; }
        }

        if (selectedList.Count == 0) selectedList = easyOrders;

        string randomOrder = selectedList[Random.Range(0, selectedList.Count)];

        // 2. Check for "Tok Abah" (Special Customer) - 10% Chance
        bool isSpecial = (Random.value < 0.1f);
        if (isSpecial)
        {
            basePoints *= 2; // Double points!
            randomOrder = "Air Kosong"; // Tok Abah always orders simple things
        }

        // 3. Spawn
        GameObject newObj = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
        currentCustomer = newObj.GetComponent<Customer>();
        
        // Pass the data to the customer
        currentCustomer.SetupCustomer(randomOrder, basePoints, isSpecial);
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