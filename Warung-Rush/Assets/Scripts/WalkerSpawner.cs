using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WalkerSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject walkerPrefab; // The empty template
    public List<Sprite> npcSprites; // Drag all your 2D character sprites here

    [Header("Movement Settings")]
    public Vector3 spawnDirection = Vector3.right; // X=1 (Right), X=-1 (Left)
    public float walkSpeed = 3f;
    public float lifetime = 7f; // How long until they despawn

    [Header("Spawn Timing")]
    public float minSpawnTime = 1f;
    public float maxSpawnTime = 3f;

    // Internal flag to stop spawning if game ends (optional)
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
            
            // Wait random time before next one
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);
        }
    }

    void SpawnNPC()
    {
        if (walkerPrefab == null || npcSprites.Count == 0) return;

        // 1. Instantiate at this object's position
        GameObject newWalker = Instantiate(walkerPrefab, transform.position, Quaternion.identity);

        // 2. Pick a random sprite
        Sprite randomSprite = npcSprites[Random.Range(0, npcSprites.Count)];

        // 3. Configure the walker
        BackgroundWalker script = newWalker.GetComponent<BackgroundWalker>();
        if (script != null)
        {
            script.SetupWalker(randomSprite, walkSpeed, spawnDirection, lifetime);
        }
        
        // Optional: Randomize depth (Z-pos) slightly so they don't clip into each other
        // newWalker.transform.position += new Vector3(0, 0, Random.Range(0.1f, 1f));
    }
}