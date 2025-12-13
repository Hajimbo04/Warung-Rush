using UnityEngine;
using System.Collections;
using TMPro;

public class Customer : MonoBehaviour
{
    [Header("Visual References")]
    public SpriteRenderer bodyRenderer;
    public SpriteRenderer faceRenderer;
    public TextMeshPro orderText; // Or TextMeshProUGUI if using World Canvas

    [Header("Expressions")]
    public Sprite faceNormal;
    public Sprite faceHappy;
    public Sprite faceSad;

    [Header("Animation Settings")]
    public float slideDuration = 0.3f;
    public float startXOffset = 10f; // Start 10 units to the right

    // Data
    [HideInInspector] public string myOrderString;
    [HideInInspector] public int myScoreValue;
    [HideInInspector] public float spawnTime;

    private Vector3 targetPos;

    public void SetupCustomer(string order, int points, bool isSpecial)
    {
        myOrderString = order;
        myScoreValue = points;
        spawnTime = Time.time;

        if (orderText != null) orderText.text = myOrderString;

        // 1. Set Default Face
        if (faceRenderer != null) faceRenderer.sprite = faceNormal;

        // 2. Handle Special Visuals (Gold Body)
        if (isSpecial && bodyRenderer != null)
        {
            bodyRenderer.color = new Color(1f, 0.8f, 0f); // Gold for Tok Abah
        }
        else if (bodyRenderer != null)
        {
            // Reset to white (so we can tint it later if we want)
            bodyRenderer.color = Color.white; 
        }

        // 3. Start Animation (Slide In)
        targetPos = transform.position; // The spawner placed us at the center (0,0)
        transform.position = new Vector3(targetPos.x + startXOffset, targetPos.y, targetPos.z);
        StartCoroutine(SlideToPosition(targetPos));
    }

    public void Leave(bool isAngry)
    {
        // 1. Change Face
        if (faceRenderer != null)
        {
            faceRenderer.sprite = isAngry ? faceSad : faceHappy;
        }

        // 2. Animate Out (Slide Left or Right?)
        // Let's slide Left for success, Right (back home) for fail, or just Left always.
        // Let's go Left (off screen) to keep flow moving.
        Vector3 exitPos = new Vector3(targetPos.x - startXOffset, targetPos.y, targetPos.z);
        
        StartCoroutine(SlideOutAndDestroy(exitPos));
    }

    // NEW FUNCTION: Call this when the player types a wrong letter/word
    public void ReactToError()
    {
        if (faceRenderer != null && faceSad != null)
        {
            faceRenderer.sprite = faceSad;
            
            // Optional: You could make the body turn red briefly too!
            // StartCoroutine(FlashRed()); 
        }
    }

    // -- ANIMATION COROUTINES --

    IEnumerator SlideToPosition(Vector3 destination)
    {
        float elapsed = 0f;
        Vector3 start = transform.position;

        while (elapsed < slideDuration)
        {
            // "SmoothStep" gives a nice ease-in/ease-out feel
            float t = elapsed / slideDuration;
            t = t * t * (3f - 2f * t); 

            transform.position = Vector3.Lerp(start, destination, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = destination;
    }

    IEnumerator SlideOutAndDestroy(Vector3 destination)
    {
        float elapsed = 0f;
        Vector3 start = transform.position;

        while (elapsed < slideDuration)
        {
            float t = elapsed / slideDuration;
            t = t * t * (3f - 2f * t);

            transform.position = Vector3.Lerp(start, destination, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = destination;
        
        // Bye bye!
        Destroy(gameObject);
    }
}
