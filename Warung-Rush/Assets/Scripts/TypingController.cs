using UnityEngine;
using TMPro;

public class TypingController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI playerInputText; 

    private string currentInput = "";

    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.isGameActive) return;
        if (CustomerSpawner.Instance == null || CustomerSpawner.Instance.currentCustomer == null) return;

        foreach (char c in Input.inputString)
        {
            if (c == '\b') 
            {
                if (currentInput.Length > 0)
                    currentInput = currentInput.Substring(0, currentInput.Length - 1);
            }
            else if ((c == '\n') || (c == '\r')) 
            {
                CheckOrder();
            }
            else
            {
                currentInput += c;
                // Optional: Add a quiet "click" sound here later!
            }
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        if (playerInputText != null) playerInputText.text = currentInput;
    }

    void CheckOrder()
    {
        Customer targetCustomer = CustomerSpawner.Instance.currentCustomer;
        string target = targetCustomer.myOrderString;

        string cleanInput = currentInput.Trim().ToLower();
        string cleanTarget = target.Trim().ToLower();

        if (cleanInput == cleanTarget)
        {
            Debug.Log("Sedap!");
            
            // --- JUICE ADDED HERE ---
            if(AudioManager.Instance != null) AudioManager.Instance.PlayCorrect();
            if(FeedbackManager.Instance != null) FeedbackManager.Instance.TriggerSuccessFX(targetCustomer.transform.position);
            // ------------------------

            if(GameManager.Instance != null) GameManager.Instance.AddScore(targetCustomer.myScoreValue);
            CustomerSpawner.Instance.OnOrderCompleted(); 
            currentInput = "";
        }
        else
        {
            Debug.Log("Salah Order!");
            
            // --- JUICE ADDED HERE ---
            if(AudioManager.Instance != null) AudioManager.Instance.PlayWrong();
            if(FeedbackManager.Instance != null) FeedbackManager.Instance.TriggerFailFX(); // Shake!
            // ------------------------

            if(GameManager.Instance != null) GameManager.Instance.ResetCombo();
            currentInput = ""; 
        }
    }
}