using UnityEngine;

public class BackgroundWalker : MonoBehaviour
{
    private float moveSpeed;
    private Vector3 moveDirection;

    // Called by the Spawner to initialize this NPC
    public void SetupWalker(Sprite sprite, float speed, Vector3 direction, float lifetime)
    {
        // 1. Set the visual
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.sprite = sprite;

        // 2. Set movement settings
        moveSpeed = speed;
        moveDirection = direction;

        // 3. Set the timer to disappear
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move in the specific direction every frame
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }
}