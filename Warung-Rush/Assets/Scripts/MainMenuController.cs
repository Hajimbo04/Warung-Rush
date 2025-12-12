using UnityEngine;
using UnityEngine.SceneManagement; // Required for switching scenes

public class MainMenuController : MonoBehaviour
{
    // Call this when "Start Game" button is clicked
    public void PlayGame()
    {
        // Make sure your gameplay scene is named "GameScene" (or change this string)
        SceneManager.LoadScene("Gameplay");
    }

    // Call this when "Quit" button is clicked
    public void QuitGame()
    {
        Debug.Log("Quit Game!");
        Application.Quit();
    }
}