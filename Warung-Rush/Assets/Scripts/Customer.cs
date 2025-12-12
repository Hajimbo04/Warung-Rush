using UnityEngine;
using TMPro;

public class Customer : MonoBehaviour
{
    [Header("Visuals")]
    public TextMeshPro orderText;
    public MeshRenderer bodyRenderer;

    [HideInInspector]
    public string myOrderString;
    [HideInInspector]
    public int myScoreValue; // How much this customer is worth

    public void SetupCustomer(string order, int points, bool isSpecial)
    {
        myOrderString = order;
        myScoreValue = points;

        if (orderText != null) orderText.text = myOrderString;

        // Visuals for Special Customer (Tok Abah)
        if (isSpecial && bodyRenderer != null)
        {
            // Turn Gold!
            bodyRenderer.material.color = new Color(1f, 0.8f, 0f); 
            // Make text bigger for special
            orderText.fontSize += 2;
        }
        else if (bodyRenderer != null)
        {
            // Reset to normal (Blue/White) just in case
            bodyRenderer.material.color = Color.white; // Or whatever your default is
        }
    }

    public void Leave(bool isAngry)
    {
        if (isAngry && bodyRenderer != null)
        {
            bodyRenderer.material.color = Color.red;
        }
        Destroy(gameObject);
    }
}