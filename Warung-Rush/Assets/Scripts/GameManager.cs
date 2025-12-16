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

    [Header("Score & Stats")]
    public int currentScore = 0;
    public int currentStreak = 0; // Uncapped streak (for stats)
    public int highestStreak = 0; // The record for this session
    public int currentComboMultiplier = 1; // Capped at x5 (for score math)
    public int maxComboMultiplier = 5;

    [Header("UI References")]
    public Slider timerSlider; 
    public Image timerFillImage; 
    
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public GameObject gameOverPanel;
    
    [Header("Game Over UI")]
    public TextMeshProUGUI finalScoreText; // "Total Sales: RM 500"
    public TextMeshProUGUI finalComboText; // NEW: "Max Streak: 20"
    public TextMeshProUGUI highScoreText;  // NEW: "Best Record: RM 1200"

    [Header("Juice Settings")]
    public float shakeIntensity = 5f; 

    [Header("Ambience")]
    public Image ambienceOverlay; 
    public Color dayColor = new Color(1f, 1f, 1f, 0f); 
    public Color eveningColor = new Color(0.2f, 0.1f, 0.4f, 0.5f); 

    public float currentTime;

    private int lastBeepSecond = -1;
    private RectTransform timerRect;
    private Vector2 originalTimerPos;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (timerSlider != null)
        {
            timerRect = timerSlider.GetComponent<RectTransform>();
            originalTimerPos = timerRect.anchoredPosition; 
        }
        StartGame();
    }

    public void StartGame()
    {
        currentTime = totalTime;
        isGameActive = true;
        currentScore = 0;
        
        // Reset Stats
        currentStreak = 0;
        highestStreak = 0;
        currentComboMultiplier = 1;
        
        lastBeepSecond = -1;
        UpdateScoreUI();

        if (timerSlider != null)
        {
            timerSlider.maxValue = totalTime;
            timerSlider.value = totalTime;
            if (timerRect != null) timerRect.anchoredPosition = originalTimerPos;
        }
        
        if (timerFillImage != null) timerFillImage.color = Color.green;
        if (ambienceOverlay != null) ambienceOverlay.color = dayColor;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (CustomerSpawner.Instance != null) CustomerSpawner.Instance.SpawnNextCustomer();
    }

    void Update()
    {
        if (isGameActive)
        {
            currentTime -= Time.deltaTime;
            
            if (timerSlider != null) timerSlider.value = currentTime;

            // Colors & Ambience
            if (timerFillImage != null)
            {
                float t = currentTime / totalTime;
                timerFillImage.color = Color.Lerp(Color.red, Color.green, t);
                if (ambienceOverlay != null) ambienceOverlay.color = Color.Lerp(eveningColor, dayColor, t);
            }

            // Shake & Audio
            if (currentTime <= 5.0f && currentTime > 0.0f)
            {
                int currentSecondInt = Mathf.CeilToInt(currentTime);
                if (currentSecondInt != lastBeepSecond)
                {
                    if(AudioManager.Instance != null) AudioManager.Instance.PlayBeep();
                    lastBeepSecond = currentSecondInt;
                }

                if (timerRect != null)
                {
                    Vector2 shakeOffset = Random.insideUnitCircle * shakeIntensity;
                    timerRect.anchoredPosition = originalTimerPos + shakeOffset;
                }
            }
            else
            {
                if (timerRect != null) timerRect.anchoredPosition = originalTimerPos;
            }

            if (currentTime <= 0) EndGame();
        }
    }

    public void AddScore(int basePoints)
    {
        // 1. Calculate Score
        int pointsToAdd = basePoints * currentComboMultiplier;
        currentScore += pointsToAdd;

        // 2. Increase Multiplier (Capped)
        if (currentComboMultiplier < maxComboMultiplier) currentComboMultiplier++;

        // 3. Track Streak (Uncapped)
        currentStreak++;
        if (currentStreak > highestStreak)
        {
            highestStreak = currentStreak;
        }

        UpdateScoreUI();
    }

    public void ResetCombo()
    {
        currentComboMultiplier = 1;
        currentStreak = 0; // Reset streak on fail
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "RM " + currentScore.ToString();
        // Show Multiplier or Streak? Let's show Multiplier for gameplay feedback
        if (comboText != null) comboText.text = "x" + currentComboMultiplier.ToString();
    }

    public void EndGame()
    {
        isGameActive = false;
        currentTime = 0;
        
        if (DoorController.Instance != null) DoorController.Instance.CloseShut();

        // --- HANDLE HIGH SCORE ---
        int savedHighScore = PlayerPrefs.GetInt("HighScore", 0);
        if (currentScore > savedHighScore)
        {
            savedHighScore = currentScore;
            PlayerPrefs.SetInt("HighScore", savedHighScore);
            PlayerPrefs.Save();
        }
        // -------------------------

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            // 1. Total Sales
            if(finalScoreText != null) 
                finalScoreText.text = "Total Sales: RM " + currentScore;

            // 2. Max Streak (NEW)
            if(finalComboText != null) 
                finalComboText.text = "Max Combo: " + highestStreak;

            // 3. High Score (NEW)
            if(highScoreText != null)
                highScoreText.text = "Best Record: RM " + savedHighScore;
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