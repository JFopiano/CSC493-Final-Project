using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// This script manages the game over sequence, including displaying a jump scare, 
// spawning a ragdoll, and showing game over UI with options to retry or return to the start screen.
public class GameOverController : MonoBehaviour
{   

    // UI elements for the jump scare, game over screen, and button panel, set in the Unity Inspector.
    [Header("UI")]
    public GameObject jumpScareImage;
    public GameObject gameOverImage;
    public GameObject buttonPanel;

    // Ragdoll prefab and spawn point for the ragdoll, set in the Unity Inspector.

    [Header("Ragdoll")]
    public GameObject ragdollPrefab;
    public Transform ragdollSpawnPoint;

    // Duration of the jump scare before transitioning to the game over screen, adjustable in the Unity Inspector.

    [Header("Timing")]
    public float jumpScareDuration = 1.0f;

    // Start is called before the first frame update to initialize the game over sequence.
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (buttonPanel != null)
        {
            buttonPanel.SetActive(false);
        }

        if (jumpScareImage != null)
        {
            jumpScareImage.SetActive(true);
        }

        StartCoroutine(GameOverSequence());
    }

    // Coroutine to handle the game over sequence, including jump scare, ragdoll spawn, and UI display.
    private IEnumerator GameOverSequence()
    {
        yield return new WaitForSeconds(jumpScareDuration);

        if (jumpScareImage != null)
        {
            jumpScareImage.SetActive(false);
        }

        SpawnRagdoll();

        yield return new WaitForSeconds(2.0f); // Short delay before showing buttons

        if (gameOverImage != null)
        {
            gameOverImage.SetActive(true);
        }

        if (buttonPanel != null)
        {
            buttonPanel.SetActive(true);
        }
    }

    // Method to spawn the ragdoll at the specified spawn point, with error handling for missing references.
    private void SpawnRagdoll()
    {
        if (ragdollPrefab == null || ragdollSpawnPoint == null)
        {
            Debug.LogWarning("Ragdoll prefab or spawn point is missing.");
            return;
        }

        GameObject ragdoll = Instantiate(
            ragdollPrefab,
            ragdollSpawnPoint.position,
            ragdollSpawnPoint.rotation
        );

        Animator animator = ragdoll.GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }
    }

    // Public method to retry the game by loading the maze scene, called by the retry button.
    public void RetryGame()
    {
        SceneManager.LoadScene("MazeScene");
    }

    // Public method to return to the start screen, called by the start screen button.
    public void GoToStartScreen()
    {
        SceneManager.LoadScene("StartScreen");
    }
}