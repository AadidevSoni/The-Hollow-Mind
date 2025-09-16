using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;

    private Rigidbody rb;
    private float pitch = 0f; // up/down rotation

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // stop physics from rotating the player

        if (playerCamera == null)
        {
            Debug.LogError("Assign playerCamera in inspector!");
            enabled = false;
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();

        // Jumping
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate body left/right
        transform.Rotate(Vector3.up * mouseX);

        // Rotate camera up/down
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -80f, 80f);
        playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal"); // no smoothing
        float moveZ = Input.GetAxisRaw("Vertical");   // instant start/stop

        Vector3 move = (transform.right * moveX + transform.forward * moveZ).normalized;

        // New position = current + move * speed * dt
        Vector3 targetPos = rb.position + move * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(targetPos);
    }

    bool IsGrounded()
    {
        // Simple ground check (raycast down a bit below player center)
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
