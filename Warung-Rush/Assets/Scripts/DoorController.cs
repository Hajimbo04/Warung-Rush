using UnityEngine;
using System.Collections;
public class DoorController : MonoBehaviour
{
    public static DoorController Instance;
    public RectTransform doorRect; 
    public float dropDuration = 0.8f;
    
    private Vector2 openPosition;
    private Vector2 closedPosition = Vector2.zero; 
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    void Start()
    {
        if (doorRect != null)
        {
            openPosition = doorRect.anchoredPosition;
        }
    }
    public void CloseShut()
    {
        StartCoroutine(AnimateDoor());
    }
    IEnumerator AnimateDoor()
    {
        float elapsed = 0f;
        while (elapsed < dropDuration)
        {
            float t = elapsed / dropDuration;
            // bounce easing formula
            float bounceT = 0f;
            
            if (t < 1 / 2.75f) {
                bounceT = 7.5625f * t * t;
            } else if (t < 2 / 2.75f) {
                t -= 1.5f / 2.75f;
                bounceT = 7.5625f * t * t + 0.75f;
            } else if (t < 2.5 / 2.75f) {
                t -= 2.25f / 2.75f;
                bounceT = 7.5625f * t * t + 0.9375f;
            } else {
                t -= 2.625f / 2.75f;
                bounceT = 7.5625f * t * t + 0.984375f;
            }
            doorRect.anchoredPosition = Vector2.Lerp(openPosition, closedPosition, bounceT);
            elapsed += Time.deltaTime;
            yield return null;
        }
        doorRect.anchoredPosition = closedPosition;
    }
}
