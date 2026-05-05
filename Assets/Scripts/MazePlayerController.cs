using UnityEngine;

// This script controls the player's movement and camera rotation in the maze scene, 
//allowing for first-person navigation using keyboard and mouse input.

public class MazePlayerController : MonoBehaviour
{
    // Public variables for movement speed, mouse sensitivity, and reference to the player's camera, set in the Unity Inspector.
    public float moveSpeed = 3.5f;
    public float mouseSensitivity = 2f;
    public Transform playerCamera;

    // Private variables for the player's Rigidbody component and current x-axis rotation of the camera.
    private Rigidbody rb;
    private float xRotation;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        LookAround();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }


    // Method to handle player movement based on keyboard input, calculating the new position and moving the Rigidbody accordingly.
    void MovePlayer()
    {
        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            move += transform.forward;

        if (Input.GetKey(KeyCode.S))
            move -= transform.forward;

        if (Input.GetKey(KeyCode.A))
            move -= transform.right;

        if (Input.GetKey(KeyCode.D))
            move += transform.right;

        Vector3 newPosition = rb.position + move.normalized * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    // Method to handle camera rotation based on mouse input, allowing the player to look around while navigating the maze.
    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}