using UnityEngine;
using TMPro;
using UnityEngine.UI; // Needed for Image and Slider
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
    
    // NEW: We need the Image component of the slider fill to change its color
    public Image timerFillImage; 
    
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    public float currentTime;

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
        lastBeepSecond = -1;
        UpdateScoreUI();

        if (timerSlider != null)
        {
            timerSlider.maxValue = totalTime;
            timerSlider.value = totalTime;
        }
        
        // Reset Color to Green
        if (timerFillImage != null)
        {
            timerFillImage.color = Color.green;
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

            // --- CLOCK COLOR LOGIC ---
            if (timerFillImage != null)
            {
                // Calculate percentage (0.0 to 1.0)
                float t = currentTime / totalTime;
                
                // Lerp from Red (Empty) to Green (Full)
                timerFillImage.color = Color.Lerp(Color.red, Color.green, t);
            }
            // -------------------------

            // Audio Cue Logic
            if (currentTime <= 5.0f && currentTime > 0.0f)
            {
                int currentSecondInt = Mathf.CeilToInt(currentTime);
                if (currentSecondInt != lastBeepSecond)
                {
                    if(AudioManager.Instance != null) AudioManager.Instance.PlayBeep();
                    lastBeepSecond = currentSecondInt;
                }
            }

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
        
        // --- TRIGGER THE DOOR (From your Guide) ---
        if (DoorController.Instance != null) 
        {
            DoorController.Instance.CloseShut();
        }
        // ------------------------------------------

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