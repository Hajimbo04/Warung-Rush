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
    public int myScoreValue;
    
    // NEW: Track when this customer appeared
    [HideInInspector]
    public float spawnTime; 

    public void SetupCustomer(string order, int points, bool isSpecial)
    {
        myOrderString = order;
        myScoreValue = points;
        
        // RECORD THE TIME NOW
        spawnTime = Time.time;

        if (orderText != null) orderText.text = myOrderString;

        // Visuals for Special Customer
        if (isSpecial && bodyRenderer != null)
        {
            bodyRenderer.material.color = new Color(1f, 0.8f, 0f); // Gold
            orderText.fontSize += 2;
        }
        else if (bodyRenderer != null)
        {
            bodyRenderer.material.color = Color.white; 
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