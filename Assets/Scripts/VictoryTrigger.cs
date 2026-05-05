using UnityEngine;
using UnityEngine.SceneManagement;

// This script handles the victory trigger in the game. 
// When the player collides with the victory trigger, it transitions to the victory scene.

public class VictoryTrigger : MonoBehaviour
{
    public string playerTag = "Player";
    public string victorySceneName = "VictoryScene";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            SceneManager.LoadScene(victorySceneName);
        }
    }
}