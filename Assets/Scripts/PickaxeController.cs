using UnityEngine;
using System.Collections;

public class PickaxeController : MonoBehaviour
{
    [Header("Idle Motion")]
    public float idleAmplitude = 5f;
    public float idleSpeed = 1f;

    [Header("Attack Swing")]
    public float swingAngle = 90f;
    public float swingDuration = 0.3f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip swingSound;
    public AudioClip hitSound;

    [Header("Hit Settings")]
    public LayerMask hitLayer;

    private Quaternion startRotation;
    private bool isSwinging = false;
    private float idleTime = 0f;

    private PlayerInventory playerInventory;

    void Start()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
        transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        startRotation = transform.localRotation;
    }

    void Update()
    {
        if (playerInventory == null || playerInventory.GetEquippedItem() != gameObject) return;

        // Idle motion
        if (!isSwinging)
        {
            idleTime += Time.deltaTime * idleSpeed;
            float idleRotation = Mathf.Sin(idleTime) * idleAmplitude;
            transform.localRotation = startRotation * Quaternion.Euler(idleRotation, 0f, 0f);
        }

        // Swing attack
        if (!isSwinging && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(SwingAttack());
        }
    }

    IEnumerator SwingAttack()
    {
        isSwinging = true;

        // Play swing sound
        if (audioSource && swingSound)
            audioSource.PlayOneShot(swingSound);

        float elapsed = 0f;
        Quaternion initialRotation = transform.localRotation;
        Quaternion topRotation = startRotation * Quaternion.Euler(0f, 0f, -swingAngle);
        Quaternion bottomRotation = startRotation;

        // Swing down
        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / swingDuration;
            transform.localRotation = Quaternion.Slerp(initialRotation, topRotation, t);
            yield return null;
        }

        // Swing finished → check for hits here
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.8f, hitLayer);
        bool hitSomething = false;

        foreach (var hit in hits)
        {
            CrystalBreak crystal = hit.GetComponent<CrystalBreak>();
            if (crystal != null)
            {
                crystal.BreakOneCrystal(); // Break one child crystal
                hitSomething = true;
                break; // Only break one crystal per swing
            }
        }

        // Slight delay so crystal disappearance syncs with impact
        yield return new WaitForSeconds(0.05f);

        // Swing back
        elapsed = 0f;
        while (elapsed < swingDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (swingDuration * 0.5f);
            transform.localRotation = Quaternion.Slerp(topRotation, bottomRotation, t);
            yield return null;
        }

        transform.localRotation = startRotation;

        // Play hit sound if a crystal was hit
        if (hitSomething && audioSource && hitSound)
            audioSource.PlayOneShot(hitSound);

        isSwinging = false;
    }
}
