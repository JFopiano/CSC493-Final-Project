using UnityEngine;
using UnityEngine.SceneManagement;

// This script manages the victory sequence in the game. 
// It handles the transition to the victory scene when the player reaches the victory trigger, 
// and provides a method to return to the start screen from the victory scene.
public class VictoryController : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void GoToStartScreen()
    {
        SceneManager.LoadScene("StartScreen");
    }
}