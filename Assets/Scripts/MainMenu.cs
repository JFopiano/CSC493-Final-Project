using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{   
    // This method is called when the Main Menu scene is loaded
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // This method is called when the "Start Game" button is clicked
    public void StartGame()
    {
        SceneManager.LoadScene("MazeScene");
    }
}