using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;

    [Header("Clips")]
    public AudioClip correctClip;
    public AudioClip wrongClip;
    public AudioClip gameOverClip;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void PlayCorrect()
    {
        if(correctClip != null) sfxSource.PlayOneShot(correctClip);
    }

    public void PlayWrong()
    {
        if(wrongClip != null) sfxSource.PlayOneShot(wrongClip);
    }

    public void PlayGameOver()
    {
        if(gameOverClip != null) sfxSource.PlayOneShot(gameOverClip);
    }
}