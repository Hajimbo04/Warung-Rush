using UnityEngine;
using System.Collections;

public class FeedbackManager : MonoBehaviour
{
    public static FeedbackManager Instance;

    public Transform cameraTransform;
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.3f;

    [Header("Particles")]
    public ParticleSystem successParticles;

    private Vector3 originalPos;
    private bool isShaking = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        originalPos = cameraTransform.localPosition;
    }

    public void TriggerSuccessFX(Vector3 position)
    {
        if (successParticles != null)
        {
            successParticles.transform.position = position;
            successParticles.Play();
        }
    }

    public void TriggerFailFX()
    {
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        if (isShaking) yield break; 
        isShaking = true;

        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            cameraTransform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        cameraTransform.localPosition = originalPos;
        isShaking = false;
    }
}