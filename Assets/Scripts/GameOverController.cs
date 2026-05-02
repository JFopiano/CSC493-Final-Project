using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{   

    // UI Elements
    [Header("UI")]
    public GameObject jumpScareImage;
    public GameObject buttonPanel;

    // Timing
    [Header("Timing")]
    public float buttonDelay = 2.5f;

    // This method is called when the Game Over scene is loaded
    private void Start()
    {
        // Ensure the cursor is visible and unlocked in the Game Over scene
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Hide the buttons and show the jump scare image
        if (buttonPanel != null)
        {
            buttonPanel.SetActive(false);
        }

        // Show the jump scare image immediately
        if (jumpScareImage != null)
        {
            jumpScareImage.SetActive(true);
        }

        // Start the coroutine to show the buttons after a delay
        StartCoroutine(ShowButtonsAfterDelay());
    }
    
    // Coroutine to show the buttons after a delay
    private IEnumerator ShowButtonsAfterDelay()
    {
        yield return new WaitForSeconds(buttonDelay);

        if (buttonPanel != null)
        {
            buttonPanel.SetActive(true);
        }
    }

    // Method to retry the game by loading the maze scene again
    public void RetryGame()
    {
        SceneManager.LoadScene("MazeScene");
    }

    // Method to go back to the start screen
    public void GoToStartScreen()
    {
        SceneManager.LoadScene("StartScreen");
    }
}