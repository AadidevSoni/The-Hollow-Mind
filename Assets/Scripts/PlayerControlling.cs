using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlling : MonoBehaviour
{
    public float speed = 5f;
    public float sprintSpeed = 10f;
    public float crouchSpeed = 2.5f;
    public float sensitivity = 300f;
    public float WaterHeight = 15.5f;
    CharacterController character;
    public GameObject cam;
    float moveFB, moveLR;
    float rotX, rotY;
    public bool webGLRightClickRotation = true;

    // ✅ Jump / Gravity
    public float jumpHeight = 2f;
    public float gravityForce = -9.8f;
    float yVelocity;

    // ✅ Stamina
    public float maxStamina = 5f;
    public float staminaRegenRate = 1.5f;
    public float sprintStaminaDrain = 1f;
    private float currentStamina;
    private bool isExhausted = false;

    // ✅ Crouch
    private bool isCrouching = false;

    // ✅ Footsteps
    public AudioSource footstepSource;
    public AudioClip footstepClip;   // 👣 Single clip
    public AudioClip jumpClip;       // 😤 Jump grunt
    public float baseStepInterval = 0.6f;
    private float stepCycle = 0f;

    // ✅ FOV Kick
    public float normalFOV = 60f;
    public float sprintFOV = 75f;
    public float fovTransitionSpeed = 5f;
    private Camera playerCam;

    private float yaw = 0f;
    private float pitch = 0f;

    void Start()
    {
        character = GetComponent<CharacterController>();
        playerCam = cam.GetComponent<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentStamina = maxStamina;
        playerCam.fieldOfView = normalFOV;

        if (Application.isEditor)
        {
            webGLRightClickRotation = false;
            sensitivity = sensitivity * 1.5f;
        }
    }

    void Update()
    {
        moveFB = Input.GetAxis("Vertical");
        moveLR = Input.GetAxis("Horizontal");

        rotX = Input.GetAxis("Mouse X") * sensitivity;
        rotY = Input.GetAxis("Mouse Y") * sensitivity;

        // ✅ Movement speed logic
        float currentSpeed = speed;

        if (Input.GetKey(KeyCode.LeftShift) && moveFB > 0.1f && !isCrouching && !isExhausted)
        {
            currentSpeed = sprintSpeed;
            currentStamina -= sprintStaminaDrain * Time.deltaTime;
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isExhausted = true;
            }
        }
        else if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        else
        {
            currentSpeed = speed;
            if (currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                if (currentStamina >= maxStamina * 0.5f)
                {
                    isExhausted = false;
                }
            }
        }

        Vector3 movement = new Vector3(moveLR * currentSpeed, 0, moveFB * currentSpeed);

        // ✅ Gravity / Jump
        if (character.isGrounded)
        {
            yVelocity = -1f;

            if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.J)) // Space or J
            {
                if (!isCrouching)
                {
                    yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravityForce);

                    // 🔊 Play jump grunt with random pitch & volume
                    if (footstepSource != null && jumpClip != null)
                    {
                        footstepSource.pitch = Random.Range(0.8f, 1.3f);   // random pitch
                        float volume = Random.Range(0.2f, 0.4f);           // random volume
                        footstepSource.PlayOneShot(jumpClip, volume);
                        footstepSource.pitch = 1f; // reset pitch
                    }
                }
            }
        }
        else
        {
            yVelocity += gravityForce * Time.deltaTime;
        }

        movement.y = yVelocity;

        // ✅ Crouch toggle
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;
            character.height = isCrouching ? 1.2f : 2f;
            cam.transform.localPosition = new Vector3(
                    cam.transform.localPosition.x,
                    isCrouching ? 0.5f : 1f,
                    cam.transform.localPosition.z
            );
        }

        // ✅ Footsteps
        HandleFootsteps(currentSpeed);

        // ✅ FOV Kick
        HandleFOV(currentSpeed);

        // ✅ Rotation
        CameraRotation(cam, rotX, rotY);

        movement = transform.rotation * movement;
        character.Move(movement * Time.deltaTime);
    }

    // ✅ Footsteps cadence with random pitch
    void HandleFootsteps(float currentSpeed)
    {
        if (!character.isGrounded) return;
        if (character.velocity.magnitude < 2f) return;

        stepCycle += Time.deltaTime;

        float interval = baseStepInterval;

        if (isCrouching) interval *= 1.5f;          // slower cadence
        else if (currentSpeed == sprintSpeed) interval *= 0.6f; // faster cadence

        if (stepCycle > interval)
        {
            stepCycle = 0f;
            if (footstepSource != null && footstepClip != null)
            {
                // 🎵 Random pitch variation
                footstepSource.pitch = Random.Range(0.9f, 1.1f);

                // 👣 Volume depends on state
                float volume = isCrouching ? 0.3f : (currentSpeed == sprintSpeed ? 0.7f : 0.5f);

                footstepSource.PlayOneShot(footstepClip, volume);

                // Reset pitch back
                footstepSource.pitch = 1f;
            }
        }
    }

    // ✅ FOV Kick
    void HandleFOV(float currentSpeed)
    {
        float targetFOV = (currentSpeed == sprintSpeed) ? sprintFOV : normalFOV;
        playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, targetFOV, Time.deltaTime * fovTransitionSpeed);
    }

    void CameraRotation(GameObject cam, float rotX, float rotY)
    {
        yaw += rotX * Time.deltaTime;
        pitch -= rotY * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, -80f, 80f);

        transform.localRotation = Quaternion.Euler(0, yaw, 0);
        cam.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
    }

    public void SetSensitivity(float value)
    {
        sensitivity = value;
    }
}
