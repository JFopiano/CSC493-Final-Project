using UnityEngine;

// This script is responsible for playing footstep sounds when the player moves. 
//It checks for movement input and plays a random footstep sound from the assigned clips at 
// regular intervals while the player is moving.
public class PlayerFootsteps : MonoBehaviour
{   
    // Public variables for audio source, footstep clips, step interval, and movement keys, set in the Unity Inspector.
    [Header("Audio")]
    public AudioSource footstepSource;
    public AudioClip[] footstepClips;

    // Timing and movement settings for footstep sounds, allowing customization of how often footsteps are 
    // played and which keys trigger movement.
    [Header("Timing")]
    public float stepInterval = 0.45f;

    // Movement keys for detecting player input, allowing for flexible control over 
    // which keys trigger footstep sounds.
    [Header("Movement Keys")]
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;

    private float nextStepTime = 0f;

    // This method initializes the footstep audio source if it hasn't been assigned 
    // in the Inspector, ensuring that the script can function even if the audio source reference is missing.
    private void Start()
    {
        if (footstepSource == null)
        {
            footstepSource = GetComponent<AudioSource>();
        }
    }

    // This method checks for player movement input and plays footstep sounds at 
    // regular intervals while the player is moving, providing audio feedback for player movement.
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

    // This method plays a random footstep sound from the assigned clips, with a random pitch for variation,
    // providing a more immersive audio experience for player movement. 
    // It also includes error handling to ensure that missing audio sources or clips do not cause issues.
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