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
    public int currentScore = 0; // Stored in Cents (Sen)
    public int currentStreak = 0; 
    public int highestStreak = 0; 
    public int currentComboMultiplier = 1; 
    public int maxComboMultiplier = 5;

    [Header("UI References")]
    public Slider timerSlider; 
    public Image timerFillImage; 
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalComboText; 
    public TextMeshProUGUI highScoreText; 

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

            // Shake
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
        int pointsToAdd = basePoints * currentComboMultiplier;
        currentScore += pointsToAdd;

        if (currentComboMultiplier < maxComboMultiplier) currentComboMultiplier++;

        currentStreak++;
        if (currentStreak > highestStreak) highestStreak = currentStreak;

        UpdateScoreUI();
    }

    public void ResetCombo()
    {
        currentComboMultiplier = 1;
        currentStreak = 0; 
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        // --- NEW DISPLAY LOGIC ---
        // Convert cents to Ringgit (Float) and format with 2 decimals
        float ringgitValue = currentScore / 100.0f;
        
        if (scoreText != null) scoreText.text = "RM " + ringgitValue.ToString("F2");
        if (comboText != null) comboText.text = "x" + currentComboMultiplier.ToString();
    }

    public void EndGame()
    {
        isGameActive = false;
        currentTime = 0;
        
        if (DoorController.Instance != null) DoorController.Instance.CloseShut();

        int savedHighScore = PlayerPrefs.GetInt("HighScore", 0);
        if (currentScore > savedHighScore)
        {
            savedHighScore = currentScore;
            PlayerPrefs.SetInt("HighScore", savedHighScore);
            PlayerPrefs.Save();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            // Format Final Score as RM
            float finalRM = currentScore / 100.0f;
            float highRM = savedHighScore / 100.0f;

            if(finalScoreText != null) 
                finalScoreText.text = "Total Sales: RM " + finalRM.ToString("F2");

            if(finalComboText != null) 
                finalComboText.text = "Best Combo: " + highestStreak;

            if(highScoreText != null)
                highScoreText.text = "Best Sales: RM " + highRM.ToString("F2");
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