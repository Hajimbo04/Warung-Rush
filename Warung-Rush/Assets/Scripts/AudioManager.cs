using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource sfxSource;
    public AudioSource musicSource;

    public AudioClip correctClip;
    public AudioClip wrongClip;
    public AudioClip gameOverClip;
    public AudioClip timerBeepClip; 
    public AudioClip ambienceClip;
    [Range(0f, 1f)] public float ambienceVolume = 0.5f; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        if (musicSource != null && ambienceClip != null)
        {
            musicSource.clip = ambienceClip;
            musicSource.volume = ambienceVolume; 
            musicSource.loop = true; 
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
    }

    public void PlayBeep()
    {
        if(timerBeepClip != null) sfxSource.PlayOneShot(timerBeepClip);
    }
}