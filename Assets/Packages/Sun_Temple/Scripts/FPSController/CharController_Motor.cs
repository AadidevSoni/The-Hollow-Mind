using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController_Motor : MonoBehaviour
{
	public float speed = 5f;
	public float sprintSpeed = 10f;
	public float crouchSpeed = 2.5f;
	public float sensitivity = 30.0f;
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

	// ✅ Head bobbing
	public float walkBobSpeed = 6f;
	public float walkBobAmount = 0.05f;
	public float sprintBobSpeed = 9f;
	public float sprintBobAmount = 0.1f;
	public float crouchBobSpeed = 4f;
	public float crouchBobAmount = 0.025f;
	public float idleBobSpeed = 1.5f;
	public float idleBobAmount = 0.015f;

	private float defaultCamY;
	private float bobTimer = 0f;

	// ✅ Stamina
	public float maxStamina = 5f;
	public float staminaRegenRate = 1.5f;
	public float sprintStaminaDrain = 1f;
	private float currentStamina;
	private bool isExhausted = false;

	// ✅ Crouch
	private bool isCrouching = false;

	// ✅ NEW FEATURE: Footsteps
	public AudioSource footstepSource;
	public AudioClip[] walkClips;
	public AudioClip[] sprintClips;
	public AudioClip[] crouchClips;
	public float stepInterval = 0.5f;
	private float stepCycle = 0f;

	// ✅ NEW FEATURE: FOV Kick
	public float normalFOV = 60f;
	public float sprintFOV = 75f;
	public float fovTransitionSpeed = 5f;
	private Camera playerCam;

	void Start()
	{
		character = GetComponent<CharacterController>();
		playerCam = cam.GetComponent<Camera>();

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		defaultCamY = cam.transform.localPosition.y;
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
					yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravityForce);
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
					isCrouching ? defaultCamY - 0.5f : defaultCamY,
					cam.transform.localPosition.z
			);
		}

		// ✅ Head bobbing
		HandleHeadBobbing(currentSpeed);

		// ✅ Footsteps
		HandleFootsteps(currentSpeed);

		// ✅ FOV Kick
		HandleFOV(currentSpeed);

		// ✅ Rotation (kept from your original code)
		if (webGLRightClickRotation)
		{
			CameraRotation(cam, rotX, rotY);
		}
		else
		{
			CameraRotation(cam, rotX, rotY);
		}

		movement = transform.rotation * movement;
		character.Move(movement * Time.deltaTime);
	}

	void HandleHeadBobbing(float currentSpeed)
	{
		if (character.isGrounded)
		{
			if (Mathf.Abs(moveFB) > 0.1f || Mathf.Abs(moveLR) > 0.1f)
			{
				bobTimer += Time.deltaTime * (isCrouching ? crouchBobSpeed : (currentSpeed == sprintSpeed ? sprintBobSpeed : walkBobSpeed));
				float bobAmount = isCrouching ? crouchBobAmount : (currentSpeed == sprintSpeed ? sprintBobAmount : walkBobAmount);
				float newY = defaultCamY + Mathf.Sin(bobTimer) * bobAmount;
				cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, newY, cam.transform.localPosition.z);
			}
			else
			{
				bobTimer += Time.deltaTime * idleBobSpeed;
				float newY = defaultCamY + Mathf.Sin(bobTimer) * idleBobAmount;
				cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, newY, cam.transform.localPosition.z);
			}
		}
	}

	// ✅ NEW FEATURE: Footsteps
	void HandleFootsteps(float currentSpeed)
	{
		if (!character.isGrounded) return;

		if (character.velocity.magnitude > 2f)
		{
			stepCycle += Time.deltaTime * (character.velocity.magnitude + currentSpeed);
			if (stepCycle > stepInterval)
			{
				stepCycle = 0f;

				if (footstepSource != null)
				{
					AudioClip clip = null;

					if (isCrouching && crouchClips.Length > 0)
						clip = crouchClips[Random.Range(0, crouchClips.Length)];
					else if (currentSpeed == sprintSpeed && sprintClips.Length > 0)
						clip = sprintClips[Random.Range(0, sprintClips.Length)];
					else if (walkClips.Length > 0)
						clip = walkClips[Random.Range(0, walkClips.Length)];

					if (clip != null) footstepSource.PlayOneShot(clip);
				}
			}
		}
	}

	// ✅ NEW FEATURE: FOV Kick
	void HandleFOV(float currentSpeed)
	{
		float targetFOV = (currentSpeed == sprintSpeed) ? sprintFOV : normalFOV;
		playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, targetFOV, Time.deltaTime * fovTransitionSpeed);
	}

	// Add these variables at the top
	private float yaw = 0f;
	private float pitch = 0f;

	void CameraRotation(GameObject cam, float rotX, float rotY)
	{
		yaw += rotX * Time.deltaTime;
		pitch -= rotY * Time.deltaTime;

		pitch = Mathf.Clamp(pitch, -80f, 80f);

		transform.localRotation = Quaternion.Euler(0, yaw, 0);
		cam.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
	}
}
