using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlling : MonoBehaviour
{
    public float speed = 5f;
    public float runSpeed = 10f;
    public float crouchSpeed = 2.5f;
    public float sensitivity = 300f;
    CharacterController character;
    public GameObject cam;
    float moveFB, moveLR;
    float rotX, rotY;
    public bool webGLRightClickRotation = true;
    public float jumpHeight = 2f;
    public float gravityForce = -9.8f;
    float yVelocity;
    public float maxStamina = 100f;
    public float staminaRegenRate = 15f;
    public float runStaminaDrain = 10f;
    private float currentStamina;
    private bool isExhausted = false;
    private bool isCrouching = false;
    public AudioSource footstepSource;
    public AudioClip footstepClip;
    public float baseStepInterval = 0.6f;
    private float stepCycle = 0f;
    public float normalFOV = 60f;
    public float runFOV = 75f;
    public float fovTransitionSpeed = 5f;
    private Camera playerCam;
    private float yaw = 0f;
    private float pitch = 0f;
    private float idleBobSpeed = 1.2f;
    private float idleBobAmount = 0.02f;
    private Vector3 originalCamLocalPos;
    private float idleTimer = 0f;

    void Start()
    {
        character = GetComponent<CharacterController>();
        playerCam = cam.GetComponent<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentStamina = maxStamina;
        playerCam.fieldOfView = normalFOV;

        originalCamLocalPos = cam.transform.localPosition;

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

        HandleMovement();

        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;
            character.height = isCrouching ? 1.2f : 2f;
            cam.transform.localPosition = new Vector3(
                    cam.transform.localPosition.x,
                    isCrouching ? 0.5f : 1f,
                    cam.transform.localPosition.z
            );
            originalCamLocalPos = cam.transform.localPosition;
        }

        HandleFootsteps();
        HandleFOV();
        HandleIdleBobbing();
        CameraRotation(cam, rotX, rotY);
    }

    void HandleMovement()
    {
        float currentSpeed = speed;

        bool wantsToRun = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.R) && moveFB > 0.1f && !isCrouching;

        if (wantsToRun && currentStamina > 0)
        {
            currentSpeed = runSpeed;
            currentStamina -= runStaminaDrain * Time.deltaTime;
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isExhausted = true;
            }
            else
            {
                isExhausted = false;
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
                if (currentStamina > maxStamina)
                    currentStamina = maxStamina;
            }
        }

        Vector3 movement = new Vector3(moveLR * currentSpeed, 0, moveFB * currentSpeed);

        if (character.isGrounded)
        {
            yVelocity = -1f;

            if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.J))
            {
                if (!isCrouching)
                {
                    yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravityForce);
                }
            }
        }
        else
        {
            yVelocity += gravityForce * Time.deltaTime;
        }

        movement.y = yVelocity;
        movement = transform.rotation * movement;
        character.Move(movement * Time.deltaTime);
    }

    void HandleFootsteps()
    {
        if (!character.isGrounded) return;

        float horizontalSpeed = new Vector3(character.velocity.x, 0, character.velocity.z).magnitude;
        if (horizontalSpeed < 1f) return;

        float interval = baseStepInterval;

        if (isCrouching) interval *= 1.5f;
        else if (horizontalSpeed > runSpeed - 1f) interval *= 0.7f;
        else interval *= 1.0f;

        stepCycle += Time.deltaTime;

        if (stepCycle > interval)
        {
            stepCycle = 0f;
            if (footstepSource != null && footstepClip != null)
            {
                footstepSource.pitch = Random.Range(0.95f, 1.05f);
                float volume = isCrouching ? 0.3f : (horizontalSpeed > runSpeed - 1f ? 0.6f : 0.5f);
                footstepSource.PlayOneShot(footstepClip, volume);
                footstepSource.pitch = 1f;
            }
        }
    }

    void HandleFOV()
    {
        float targetFOV = normalFOV;
        if ((Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.LeftShift)) && moveFB > 0.1f && !isCrouching && !isExhausted)
            targetFOV = runFOV;
        playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, targetFOV, Time.deltaTime * fovTransitionSpeed);
    }
    void HandleIdleBobbing()
    {
        float horizontalSpeed = new Vector3(character.velocity.x, 0, character.velocity.z).magnitude;

        if (horizontalSpeed < 0.1f && character.isGrounded && !isCrouching)
        {
            idleTimer += Time.deltaTime * idleBobSpeed;
            float newY = originalCamLocalPos.y + Mathf.Sin(idleTimer) * idleBobAmount;
            float newX = originalCamLocalPos.x + Mathf.Sin(idleTimer * 0.5f) * idleBobAmount * 0.5f;
            float newZ = originalCamLocalPos.z;

            cam.transform.localPosition = new Vector3(newX, newY, newZ);
        }
        else
        {
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, originalCamLocalPos, Time.deltaTime * 4f);
            idleTimer = 0f;
        }
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

    public float GetCurrentStamina()
    {
        return currentStamina;
    }

    public float GetMaxStamina()
    {
        return maxStamina;
    }
}
