using UnityEngine;
using System.Collections;
using TMPro;

public class Customer : MonoBehaviour
{
    public SpriteRenderer bodyRenderer;
    public SpriteRenderer faceRenderer;
    public TextMeshPro orderText; 
    public Sprite faceNormal;
    public Sprite faceHappy;
    public Sprite faceSad;
    public float slideDuration = 0.3f;
    public float startXOffset = 10f;

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
        if (faceRenderer != null) faceRenderer.sprite = faceNormal;

        if (isSpecial && bodyRenderer != null)
        {
            bodyRenderer.color = new Color(1f, 0.8f, 0f); 
        }
        else if (bodyRenderer != null)
        {
            bodyRenderer.color = Color.white; 
        }

        targetPos = transform.position; 
        transform.position = new Vector3(targetPos.x + startXOffset, targetPos.y, targetPos.z);
        StartCoroutine(SlideToPosition(targetPos));
    }

    public void Leave(bool isAngry)
    {
        if (faceRenderer != null)
        {
            faceRenderer.sprite = isAngry ? faceSad : faceHappy;
        }

        Vector3 exitPos = new Vector3(targetPos.x - startXOffset, targetPos.y, targetPos.z);
        StartCoroutine(SlideOutAndDestroy(exitPos));
    }

    public void ReactToError()
    {
        if (faceRenderer != null && faceSad != null)
        {
            faceRenderer.sprite = faceSad;
        }
    }

    IEnumerator SlideToPosition(Vector3 destination)
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
        
        Destroy(gameObject);
    }
}
