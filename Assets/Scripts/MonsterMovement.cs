using UnityEngine;
using UnityEngine.AI;

public class MonsterMovement : MonoBehaviour
{
    // Reference to the NavMeshAgent component
    private NavMeshAgent agent;
    
    // Checkpoint positions for patrolling
    private Vector3[] checkpointPositions;
    private int currentCheckpointIndex = -1;

    // Patrol settings
    [Header("Patrol Settings")]
    public float reachedDistance = 0.3f;

    // Player detection settings
    [Header("Player Detection")]
    public string playerTag = "Player";
    public float viewDistance = 8f;
    public float viewAngle = 60f;
    public float eyeHeight = 1.5f;
    public LayerMask obstacleMask;

    // Chase settings
    [Header("Chase Settings")]
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 3.5f;
    public float losePlayerTime = 2f;

    [Header("Monster Audio")]
    public AudioSource farAudioSource;
    public AudioSource closeAudioSource;
    public AudioSource chaseAudioSource;
    private AudioManager audioManager;
    private bool previousChaseState = false;

    public float closeAudioDistance = 8f;
    public float audioFadeSpeed = 3f;

    [Range(0f, 1f)] public float farMaxVolume = 0.25f;
    [Range(0f, 1f)] public float closeMaxVolume = 0.7f;
    [Range(0f, 1f)] public float chaseMaxVolume = 1f;

    [Header("Monster Animation")]
    private Animator animator;

    // Internal state
    private Transform player;
    private bool isChasing = false;
    private float timeSinceLastSeen = 0f;

    // Initialize references
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    // Set the initial patrol destination and player reference
    private void Start()
    {   
        // Find the player in the scene by tag
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        audioManager = FindAnyObjectByType<AudioManager>();
        StartMonsterAudio();

        // If a player object was found, store its transform for later use
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("No object with Player tag was found.");
        }

        // Start patrolling immediately
        if (agent != null)
        {
            agent.speed = patrolSpeed;
        }
    }

    // Update is called once per frame
    private void Update()
    {   
        // If the agent is not ready, do nothing
        if (agent == null || !agent.enabled || !agent.isOnNavMesh) return;

        // Check if the monster can see the player
        bool canSeePlayer = CanSeePlayer();

        // If the monster can see the player, start chasing. 
        // If it loses sight of the player, start a timer. 
        // If the timer runs out, go back to patrolling.
        if (canSeePlayer)
        {
            isChasing = true;
            timeSinceLastSeen = 0f;
        }
        else if (isChasing)
        {
            timeSinceLastSeen += Time.deltaTime;

            if (timeSinceLastSeen >= losePlayerTime)
            {
                isChasing = false;
                agent.speed = patrolSpeed;
                GoToRandomCheckpoint();
            }
        }

        // If currently chasing the player, update the destination to the player's position. 
        // Otherwise, continue patrolling.
        if (isChasing && player != null)
        {
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
        }
        else
        {
            PatrolUpdate();
        }

        UpdateMonsterAudio();
        UpdateAnimation();
        UpdateChaseMusic();



    }

    // This method is called from MazeRenderer after the monster is spawned and placed on the NavMesh.
    private void PatrolUpdate()
    {
        if (checkpointPositions == null || checkpointPositions.Length == 0) return;
        if (agent.pathPending) return;

        if (agent.remainingDistance <= Mathf.Max(agent.stoppingDistance, reachedDistance))
        {
            GoToRandomCheckpoint();
        }
    }

    // This method is called by MazeRenderer to set the patrol checkpoints after the maze is generated.
    public void SetCheckpoints(Vector3[] newCheckpointPositions)
    {
        checkpointPositions = newCheckpointPositions;
        GoToRandomCheckpoint();
    }

    // This method is called by MazeRenderer to set the patrol checkpoints after the maze is generated.
    private void GoToRandomCheckpoint()
    {
        if (checkpointPositions == null || checkpointPositions.Length == 0) return;

        int nextIndex = Random.Range(0, checkpointPositions.Length);

        if (checkpointPositions.Length > 1)
        {
            while (nextIndex == currentCheckpointIndex)
            {
                nextIndex = Random.Range(0, checkpointPositions.Length);
            }
        }

        currentCheckpointIndex = nextIndex;
        agent.SetDestination(checkpointPositions[currentCheckpointIndex]);

        Debug.Log("Monster moving to checkpoint: " + currentCheckpointIndex);
    }

    // This method checks if the monster can see the player based on distance, angle, and line of sight.
    private bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 eyePosition = transform.position + Vector3.up * eyeHeight;
        Vector3 playerPosition = player.position + Vector3.up * eyeHeight;

        Vector3 directionToPlayer = playerPosition - eyePosition;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > viewDistance)
        {
            return false;
        }

        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer > viewAngle / 2f)
        {
            return false;
        }

        if (Physics.Raycast(eyePosition, directionToPlayer.normalized, out RaycastHit hit, viewDistance))
        {
            if (hit.collider.CompareTag(playerTag))
            {
                return true;
            }

            // Something else, probably a wall, blocked the view.
            return false;
        }

        return false;
    }

    // This method draws gizmos in the editor to visualize the monster's field of view and detection range.
    private void OnDrawGizmosSelected()
    {
        Vector3 eyePosition = transform.position + Vector3.up * eyeHeight;

        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Vector3 leftBoundary = Quaternion.Euler(0f, -viewAngle / 2f, 0f) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0f, viewAngle / 2f, 0f) * transform.forward;

        Gizmos.DrawRay(eyePosition, leftBoundary * viewDistance);
        Gizmos.DrawRay(eyePosition, rightBoundary * viewDistance);
    }

    private void StartMonsterAudio()
    {
        if (farAudioSource != null && !farAudioSource.isPlaying)
        {
            farAudioSource.volume = farMaxVolume;
            farAudioSource.Play();
        }

        if (closeAudioSource != null && !closeAudioSource.isPlaying)
        {
            closeAudioSource.volume = 0f;
            closeAudioSource.Play();
        }

        if (chaseAudioSource != null && !chaseAudioSource.isPlaying)
        {
            chaseAudioSource.volume = 0f;
            chaseAudioSource.Play();
        }
    }

    private void UpdateMonsterAudio()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        float targetFarVolume = farMaxVolume;
        float targetCloseVolume = 0f;
        float targetChaseVolume = 0f;

        if (isChasing)
        {
            targetFarVolume = 0f;
            targetCloseVolume = 0f;
            targetChaseVolume = chaseMaxVolume;
        }
        else if (distanceToPlayer <= closeAudioDistance)
        {
            targetFarVolume = 0f;

            float closeness = 1f - Mathf.Clamp01(distanceToPlayer / closeAudioDistance);
            targetCloseVolume = Mathf.Lerp(0.2f, closeMaxVolume, closeness);
        }

        FadeAudioSource(farAudioSource, targetFarVolume);
        FadeAudioSource(closeAudioSource, targetCloseVolume);
        FadeAudioSource(chaseAudioSource, targetChaseVolume);
    }

    private void FadeAudioSource(AudioSource source, float targetVolume)
    {
        if (source == null) return;

        source.volume = Mathf.MoveTowards(
            source.volume,
            targetVolume,
            audioFadeSpeed * Time.deltaTime
        );
    }

    private void UpdateAnimation()
    {
        if (animator == null || agent == null) return;

        bool isMoving = agent.velocity.magnitude > 0.1f;

        animator.SetInteger("Walk", isMoving ? 1 : 0);
    }

    private void UpdateChaseMusic()
    {
        if (audioManager == null) return;

        if (isChasing != previousChaseState)
        {
            audioManager.SetChaseMusic(isChasing);
            previousChaseState = isChasing;
        }
    }
}

