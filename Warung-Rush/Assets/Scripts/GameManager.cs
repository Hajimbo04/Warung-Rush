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
    public Image timerFillImage; 
    
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    [Header("Juice Settings")]
    public float shakeIntensity = 5f; 

    [Header("Ambience (Day/Night Cycle)")]
    public Image ambienceOverlay; // Drag a UI Panel here
    // Start: Transparent (Clear day)
    public Color dayColor = new Color(1f, 1f, 1f, 0f); 
    // End: Dark Purple/Orange tint (Maghrib vibe)
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
        currentCombo = 1;
        lastBeepSecond = -1;
        UpdateScoreUI();

        if (timerSlider != null)
        {
            timerSlider.maxValue = totalTime;
            timerSlider.value = totalTime;
            if (timerRect != null) timerRect.anchoredPosition = originalTimerPos;
        }
        
        if (timerFillImage != null) timerFillImage.color = Color.green;

        // Reset Ambience
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

            // --- COLOR LOGIC (Timer) ---
            if (timerFillImage != null)
            {
                float t = currentTime / totalTime;
                timerFillImage.color = Color.Lerp(Color.red, Color.green, t);
            }

            // --- AMBIENCE LOGIC (Day to Night) ---
            if (ambienceOverlay != null)
            {
                float t = currentTime / totalTime; // 1.0 (Start) -> 0.0 (End)
                // Lerp from Evening (0) to Day (1)
                ambienceOverlay.color = Color.Lerp(eveningColor, dayColor, t);
            }
            // -------------------------------------

            // --- SHAKE LOGIC ---
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
        
        if (DoorController.Instance != null) DoorController.Instance.CloseShut();

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