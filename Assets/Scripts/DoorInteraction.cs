using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorInteraction : MonoBehaviour
{
    public float interactDistance = 5f;
    public KeyCode interactKey = KeyCode.E;
    public float openForce = 100f;

    private Rigidbody rb;
    private Camera playerCamera;

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

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            TryOpenDoor();
        }


    }

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