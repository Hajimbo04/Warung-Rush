using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;

    [Header("State")]
    public int currentScore = 0;
    public int currentCombo = 1; // Starts at x1
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        UpdateUI();
    }

    public void AddScore(int basePoints)
    {
        // 1. Calculate Score based on Combo
        int pointsToAdd = basePoints * currentCombo;
        currentScore += pointsToAdd;

        // 2. Increase Combo (Cap at x5)
        if (currentCombo < 5)
        {
            currentCombo++;
        }

        UpdateUI();
    }

    public void ResetCombo()
    {
        currentCombo = 1;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null) 
            scoreText.text = "RM " + currentScore.ToString();
        
        if (comboText != null) 
            comboText.text = "x" + currentCombo.ToString();

        // Optional: Change Combo color based on streak
        if (comboText != null)
        {
            if (currentCombo >= 5) comboText.color = Color.red; // ON FIRE!
            else if (currentCombo >= 3) comboText.color = Color.yellow;
            else comboText.color = Color.white;
        }
    }
}