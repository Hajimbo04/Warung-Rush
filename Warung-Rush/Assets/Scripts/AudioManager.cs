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
    
    // NEW: The countdown beep
    public AudioClip timerBeepClip; 

    // NEW: Background Ambience
    public AudioClip ambienceClip;
    [Range(0f, 1f)] public float ambienceVolume = 0.5f; // Add this volume slider (Default 0.5)

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        // Automatically play the ambience when the game starts
        if (musicSource != null && ambienceClip != null)
        {
            musicSource.clip = ambienceClip;
            musicSource.volume = ambienceVolume; // Apply the volume here
            musicSource.loop = true; // Crucial: Makes it loop forever
            musicSource.Play();
        }
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
        
        // Optional: If you want to stop the background noise on Game Over, uncomment this:
        // if(musicSource != null) musicSource.Stop();
    }

    public void PlayBeep()
    {
        if(timerBeepClip != null) sfxSource.PlayOneShot(timerBeepClip);
    }
}