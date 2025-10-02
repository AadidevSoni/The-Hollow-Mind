using UnityEngine;

public class PickaxeController : MonoBehaviour
{
    [Header("Idle Motion")]
    public float idleAmplitude = 5f;   // Degrees of idle rotation
    public float idleSpeed = 1f;       // How fast it sways

    [Header("Attack Swing")]
    public float swingAngle = 90f;     // Max swing rotation for attack
    public float swingDuration = 0.3f; // Seconds for one swing

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip swingSound;
    public AudioClip hitSound;

    [Header("Hit Settings")]
    public LayerMask hitLayer; // Only objects in these layers count as a hit

    private Quaternion startRotation;
    private bool isSwinging = false;
    private float idleTime = 0f;
    private bool hitSomething = false;

    // reference to player inventory
    private PlayerInventory playerInventory;

    void Start()
    {
        // Cache inventory from player
        playerInventory = FindObjectOfType<PlayerInventory>();

        // Make the pickaxe straight (pointing forward)
        transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        startRotation = transform.localRotation;
    }

    void Update()
    {
        // Only run logic if pickaxe is actually equipped
        if (playerInventory == null || playerInventory.GetEquippedItem() != gameObject) return;

        // Handle idle motion
        if (!isSwinging)
        {
            idleTime += Time.deltaTime * idleSpeed;
            float idleRotation = Mathf.Sin(idleTime) * idleAmplitude;
            transform.localRotation = startRotation * Quaternion.Euler(idleRotation, 0f, 0f);
        }

        // Attack swing on left click
        if (!isSwinging && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(SwingAttack());
        }
    }

    System.Collections.IEnumerator SwingAttack()
    {
        isSwinging = true;
        hitSomething = false;

        // play swing sound at start
        if (audioSource && swingSound)
        {
            audioSource.clip = swingSound;
            audioSource.loop = false;
            audioSource.Play();
        }

        float elapsed = 0f;

        Quaternion initialRotation = transform.localRotation;
        Quaternion topRotation = startRotation * Quaternion.Euler(0f, 0f, -swingAngle); // Swing from top
        Quaternion bottomRotation = startRotation; // Finish at resting angle

        // Swing down
        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / swingDuration;
            transform.localRotation = Quaternion.Slerp(initialRotation, topRotation, t);

            // Check for hits via LayerMask during swing
            Collider[] hits = Physics.OverlapSphere(transform.position, 0.5f, hitLayer);
            if (hits.Length > 0)
            {
                hitSomething = true;
            }

            yield return null;
        }

        // Swing back to bottom
        elapsed = 0f;
        while (elapsed < swingDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (swingDuration * 0.5f);
            transform.localRotation = Quaternion.Slerp(topRotation, bottomRotation, t);
            yield return null;
        }

        transform.localRotation = startRotation;

        // play hit sound if something was hit
        if (hitSomething && audioSource && hitSound)
        {
            audioSource.PlayOneShot(hitSound);
        }

        isSwinging = false;
    }

    // Remove old collision checks or keep for additional feedback
    private void OnCollisionEnter(Collision collision)
    {
        if (playerInventory == null || playerInventory.GetEquippedItem() != gameObject) return;

        if (isSwinging && ((1 << collision.gameObject.layer) & hitLayer) != 0)
        {
            hitSomething = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerInventory == null || playerInventory.GetEquippedItem() != gameObject) return;

        if (isSwinging && ((1 << other.gameObject.layer) & hitLayer) != 0)
        {
            hitSomething = true;
        }
    }
}
