using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Required for Coroutines

public class SceneTransitionTrigger : MonoBehaviour
{
    [Tooltip("The name of the scene to load when this object is clicked.")]
    public string targetSceneName = "Gameplay";

    [Header("Click Animation Settings")]
    [Tooltip("The vertical distance the object moves up.")]
    public float moveDistance = 0.5f; 
    [Tooltip("The duration of the upward movement.")]
    public float animationDuration = 0.2f; 

    // OPTIONAL: Add visual feedback for a better feel
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public Color hoverColor = new Color(0.8f, 0.8f, 0.8f); // Slightly darker when hovering

    private bool isClicked = false; // Flag to prevent multiple clicks during transition

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    // This function is automatically called by Unity when the left mouse button
    // is clicked while the mouse is over this object's Collider.
    void OnMouseDown()
    {
        if (isClicked) return; // Ignore input if already clicked
        
        isClicked = true;
        StartCoroutine(ClickAnimationAndLoadScene());
    }

    IEnumerator ClickAnimationAndLoadScene()
    {
        Debug.Log("Object clicked. Starting up animation...");

        // 1. Define start and end points for the animation
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.up * moveDistance;
        
        float timeElapsed = 0f;

        // 2. Animate the object moving up using linear interpolation (Lerp)
        while (timeElapsed < animationDuration)
        {
            // Move the position smoothly from startPos to targetPos
            transform.position = Vector3.Lerp(startPos, targetPos, timeElapsed / animationDuration);
            timeElapsed += Time.deltaTime;
            yield return null; // Wait until the next frame
        }

        // Ensure the object reaches the exact final position
        transform.position = targetPos;

        // 3. Load the target scene after the animation finishes
        Debug.Log($"Animation finished. Transitioning to scene: {targetSceneName}");
        SceneManager.LoadScene(targetSceneName);
    }

    void OnMouseEnter()
    {
        // Change color/scale when the mouse moves over the object, only if not clicked
        if (!isClicked && spriteRenderer != null)
        {
            spriteRenderer.color = hoverColor;
        }
    }

    void OnMouseExit()
    {
        // Restore color/scale when the mouse moves away, only if not clicked
        if (!isClicked && spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }
}