using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource footstepSource;
    public AudioClip[] footstepClips;

    [Header("Timing")]
    public float stepInterval = 0.45f;

    [Header("Movement Keys")]
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;

    private float nextStepTime = 0f;

    private void Start()
    {
        if (footstepSource == null)
        {
            footstepSource = GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        bool isMoving =
            Input.GetKey(forwardKey) ||
            Input.GetKey(backwardKey) ||
            Input.GetKey(leftKey) ||
            Input.GetKey(rightKey);

        if (!isMoving)
        {
            nextStepTime = 0f;
            return;
        }

        if (Time.time >= nextStepTime)
        {
            PlayFootstep();
            nextStepTime = Time.time + stepInterval;
        }
    }

    private void PlayFootstep()
    {
        Debug.Log("Trying to play footstep.");

        if (footstepSource == null)
        {
            Debug.LogWarning("Footstep AudioSource is missing.");
            return;
        }

        if (footstepClips == null || footstepClips.Length == 0)
        {
            Debug.LogWarning("No footstep clips assigned.");
            return;
        }

        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];

        if (clip == null)
        {
            Debug.LogWarning("Footstep clip slot is empty.");
            return;
        }

        footstepSource.pitch = Random.Range(0.9f, 1.1f);
        footstepSource.PlayOneShot(clip);

        Debug.Log("Footstep played: " + clip.name);
    }
}