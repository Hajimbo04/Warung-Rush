using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; 

public class SceneTransitionTrigger : MonoBehaviour
{
    public string targetSceneName = "Gameplay";
    public float moveDistance = 0.5f; 
    public float animationDuration = 0.2f; 

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public Color hoverColor = new Color(0.8f, 0.8f, 0.8f); 
    private bool isClicked = false; 

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    void OnMouseDown()
    {
        if (isClicked) return; 
        
        isClicked = true;
        StartCoroutine(ClickAnimationAndLoadScene());
    }

    IEnumerator ClickAnimationAndLoadScene()
    {
        Debug.Log("Object clicked. Starting up animation...");

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.up * moveDistance;
        
        float timeElapsed = 0f;

        while (timeElapsed < animationDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, timeElapsed / animationDuration);
            timeElapsed += Time.deltaTime;
            yield return null; 
        }

        transform.position = targetPos;

        Debug.Log($"Animation finished. Transitioning to scene: {targetSceneName}");
        SceneManager.LoadScene(targetSceneName);
    }

    void OnMouseEnter()
    {
        if (!isClicked && spriteRenderer != null)
        {
            spriteRenderer.color = hoverColor;
        }
    }

    void OnMouseExit()
    {
        if (!isClicked && spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }
}