using UnityEngine;
using UnityEngine.SceneManagement;

// This script handles the collision between the player and the monster in the maze scene. 
// When the player collides with the monster, it triggers a scene change to the Game Over screen.
public class MonsterCollision : MonoBehaviour
{
    public string playerTag = "Player";
    public string gameOverSceneName = "GameOverScene";

    // This method is called when another collider enters the trigger collider attached to the monster.
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Monster trigger touched: " + other.gameObject.name);

        if (other.CompareTag(playerTag))
        {
            Debug.Log("Player detected. Loading game over scene.");
            SceneManager.LoadScene(gameOverSceneName);
        }
    }
}