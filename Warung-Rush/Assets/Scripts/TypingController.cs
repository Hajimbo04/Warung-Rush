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
            
            // 1. Add Score & Combo
            if(GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(targetCustomer.myScoreValue);
            }

            // 2. Move queue
            CustomerSpawner.Instance.OnOrderCompleted(); 
            currentInput = "";
        }
        else
        {
            Debug.Log("Salah Order!");
            
            // 1. Reset Combo :(
            if(GameManager.Instance != null)
            {
                GameManager.Instance.ResetCombo();
            }

            currentInput = ""; 
        }
    }
}