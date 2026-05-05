using UnityEngine;
using UnityEngine.SceneManagement;

// This script allows the player to interact with doors in the game by pressing a key when looking at them, applying a force to open them.
public class DoorInteraction : MonoBehaviour
{   
    // Distance within which the player can interact with the door, adjustable in the Unity Inspector.
    public float interactDistance = 5f;

    // Key used to interact with the door, adjustable in the Unity Inspector.
    public KeyCode interactKey = KeyCode.E;

    // Force applied to the door when opening, adjustable in the Unity Inspector.
    public float openForce = 100f;

    // Reference to the door's Rigidbody component and the player's camera, initialized in Start.
    private Rigidbody rb;
    private Camera playerCamera;
    
    // Initialize references to the Rigidbody and player camera, with error handling if they are not found.
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCamera = Camera.main;

        if (rb == null)
        {
            Debug.LogWarning("Door has no Rigidbody.");
        }

        if (playerCamera == null)
        {
            Debug.LogWarning("No MainCamera found. Make sure the player camera is tagged MainCamera.");
        }
    }

    // Update is called once per frame to check for player input and attempt to open the door.
    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            TryOpenDoor();
        }


    }

    // Method to attempt to open the door by raycasting from the player's camera and applying a force if the door is hit.
    void TryOpenDoor()
    {
        if (playerCamera == null || rb == null)
        {
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red, 1f);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            Debug.Log("Ray hit: " + hit.collider.gameObject.name);

            if (hit.collider.gameObject == gameObject || hit.collider.transform.IsChildOf(transform))
            {
                Debug.Log("Door hit. Opening door.");
                rb.AddTorque(Vector3.up * openForce, ForceMode.Impulse);
            }
        }
        else
        {
            Debug.Log("Ray hit nothing.");
        }
    }
}