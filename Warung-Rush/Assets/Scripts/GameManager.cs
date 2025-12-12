using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public float totalTime = 20f;
    public bool isGameActive = false;

    [Header("Score & Combo")]
    public int currentScore = 0;
    public int currentCombo = 1;
    public int maxCombo = 5;

    [Header("UI References")]
    public Slider timerSlider; 
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    public float currentTime;

    // NEW: Track the last second we beeped at to prevent spamming
    private int lastBeepSecond = -1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        currentTime = totalTime;
        isGameActive = true;
        currentScore = 0;
        currentCombo = 1;
        lastBeepSecond = -1; // Reset beep tracker
        UpdateScoreUI();

        if (timerSlider != null)
        {
            timerSlider.maxValue = totalTime;
            timerSlider.value = totalTime;
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (CustomerSpawner.Instance != null) CustomerSpawner.Instance.SpawnNextCustomer();
    }

    void Update()
    {
        if (isGameActive)
        {
            currentTime -= Time.deltaTime;
            
            if (timerSlider != null) timerSlider.value = currentTime;

            // --- PART 4: AUDIO CUE LOGIC ---
            // Check if we are in the last 5 seconds (and not at 0)
            if (currentTime <= 5.0f && currentTime > 0.0f)
            {
                // CeilToInt creates a countdown effect (5, 4, 3, 2, 1)
                int currentSecondInt = Mathf.CeilToInt(currentTime);

                // If we haven't beeped for this second yet...
                if (currentSecondInt != lastBeepSecond)
                {
                    if(AudioManager.Instance != null) AudioManager.Instance.PlayBeep();
                    lastBeepSecond = currentSecondInt;
                }
            }
            // -------------------------------

            if (currentTime <= 0) EndGame();
        }
    }

    public void AddScore(int basePoints)
    {
        int pointsToAdd = basePoints * currentCombo;
        currentScore += pointsToAdd;
        if (currentCombo < maxCombo) currentCombo++;
        UpdateScoreUI();
    }

    public void ResetCombo()
    {
        currentCombo = 1;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "RM " + currentScore.ToString();
        if (comboText != null) comboText.text = "x" + currentCombo.ToString();
    }

    public void EndGame()
    {
        isGameActive = false;
        currentTime = 0;
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if(finalScoreText != null) finalScoreText.text = "Total Sales: RM " + currentScore;
        }
        
        if(AudioManager.Instance != null) AudioManager.Instance.PlayGameOver();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}