using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public int currentScore = 0;
    public int currentCombo = 1; 
    
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
        int pointsToAdd = basePoints * currentCombo;
        currentScore += pointsToAdd;

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

        if (comboText != null)
        {
            if (currentCombo >= 5) comboText.color = Color.red; // ON FIRE!
            else if (currentCombo >= 3) comboText.color = Color.yellow;
            else comboText.color = Color.white;
        }
    }
}