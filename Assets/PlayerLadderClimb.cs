using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerLadderClimb : MonoBehaviour
{
    [Header("Climbing Settings")]
    public float climbSpeed = 3f;
    public KeyCode climbKey = KeyCode.W;
    public KeyCode descendKey = KeyCode.S;

    private CharacterController controller;
    private bool isClimbing = false;
    private bool isInLadderZone = false;
    private Transform ladderTransform;

    private PlayerControlling playerMovement;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerControlling>();
    }

    void Update()
    {
        if (isInLadderZone)
        {
            float verticalInput = 0f;

            if (Input.GetKey(climbKey))
                verticalInput = 1f;
            else if (Input.GetKey(descendKey))
                verticalInput = -1f;

            if (verticalInput != 0f)
            {
                if (!isClimbing) StartClimbing();
                HandleClimbing(verticalInput);
            }
            else if (isClimbing)
            {
                StopClimbing();
            }

            if (isClimbing && Input.GetKeyDown(KeyCode.Space))
                StopClimbing();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder") || other.GetComponent<Ladder>() != null)
        {
            isInLadderZone = true;
            ladderTransform = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder") || other.GetComponent<Ladder>() != null)
        {
            isInLadderZone = false;
            StopClimbing();
            ladderTransform = null;
        }
    }

    private void StartClimbing()
    {
        isClimbing = true;

        if (playerMovement != null)
            playerMovement.enabled = false;
    }

    private void HandleClimbing(float direction)
    {
        Vector3 climbMove = Vector3.up * direction * climbSpeed;
        controller.Move(climbMove * Time.deltaTime);

        if (ladderTransform)
        {
            Vector3 alignPos = new Vector3(ladderTransform.position.x, transform.position.y, ladderTransform.position.z);
            transform.position = Vector3.Lerp(transform.position, alignPos, Time.deltaTime * 5f);
        }
    }

    private void StopClimbing()
    {
        isClimbing = false;

        if (playerMovement != null)
            playerMovement.enabled = true;
    }
}
