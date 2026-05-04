using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [Header("UI")]
    public GameObject jumpScareImage;
    public GameObject gameOverImage;
    public GameObject buttonPanel;

    [Header("Ragdoll")]
    public GameObject ragdollPrefab;
    public Transform ragdollSpawnPoint;

    [Header("Timing")]
    public float jumpScareDuration = 1.0f;

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

    public void RetryGame()
    {
        SceneManager.LoadScene("MazeScene");
    }

    public void GoToStartScreen()
    {
        SceneManager.LoadScene("StartScreen");
    }
}