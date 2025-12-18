using UnityEngine;

public class BackgroundWalker : MonoBehaviour
{
    private float moveSpeed;
    private Vector3 moveDirection;

    public void SetupWalker(Sprite sprite, float speed, Vector3 direction, float lifetime)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.sprite = sprite;

        moveSpeed = speed;
        moveDirection = direction;

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }
}