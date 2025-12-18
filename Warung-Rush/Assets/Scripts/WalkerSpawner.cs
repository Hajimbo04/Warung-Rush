using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WalkerSpawner : MonoBehaviour
{
    public GameObject walkerPrefab;
    public List<Sprite> npcSprites;
    public Vector3 spawnDirection = Vector3.right; 
    public float walkSpeed = 3f;
    public float lifetime = 7f; 

    [Header("Spawn Timing")]
    public float minSpawnTime = 1f;
    public float maxSpawnTime = 3f;

    private bool isSpawning = true;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (isSpawning)
        {
            SpawnNPC();
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);
        }
    }

    void SpawnNPC()
    {
        if (walkerPrefab == null || npcSprites.Count == 0) return;

        GameObject newWalker = Instantiate(walkerPrefab, transform.position, Quaternion.identity);
        Sprite randomSprite = npcSprites[Random.Range(0, npcSprites.Count)];

        BackgroundWalker script = newWalker.GetComponent<BackgroundWalker>();
        if (script != null)
        {
            script.SetupWalker(randomSprite, walkSpeed, spawnDirection, lifetime);
        }
    }
}