using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    public float interactDistance = 2.5f;
    public KeyCode interactKey = KeyCode.E;
    public float openForce = 8f;

    private Rigidbody rb;
    private Camera playerCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCamera = Camera.main;
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
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.gameObject == gameObject || hit.collider.transform.IsChildOf(transform))
            {
                rb.AddForceAtPosition(playerCamera.transform.forward * openForce, hit.point, ForceMode.Impulse);
            }
        }
    }
}