using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterCollision : MonoBehaviour
{
    public string playerTag = "Player";
    public string gameOverSceneName = "GameOverScene";

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