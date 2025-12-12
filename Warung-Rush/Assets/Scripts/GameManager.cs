using UnityEngine;
using TMPro;
using UnityEngine.UI; // REQUIRED for Slider

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
    public Slider timerSlider; // NEW: The visual bar
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    public float currentTime;

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
        UpdateScoreUI();

        // Reset Slider
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
            
            // Update Slider
            if (timerSlider != null)
            {
                timerSlider.value = currentTime;
                
                // Visual Panic: Change color if low (Optional, requires Image reference)
                // simpler to just do this in the Afternoon "Juice" session.
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
        Debug.Log("Tutup Kedai! Score: " + currentScore);
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if(finalScoreText != null) finalScoreText.text = "Total Sales: RM " + currentScore;
        }
    }
}