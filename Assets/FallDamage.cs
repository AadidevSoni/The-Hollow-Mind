using UnityEngine;

public class FallDamage : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;
    public PlayerHealth healthSystem;

    [Header("Fall Settings")]
    public float safeFallSpeed = 10f;
    public float lethalFallSpeed = 25f;
    public float damageMultiplier = 5f;

    private float lastYVelocity;
    private bool wasGroundedLastFrame;

    void Update()
    {
        bool isGrounded = controller.isGrounded;

        if (!wasGroundedLastFrame && isGrounded)
        {
            float fallSpeed = Mathf.Abs(lastYVelocity);
            if (fallSpeed > safeFallSpeed)
            {
                float damage = (fallSpeed - safeFallSpeed) * damageMultiplier;
                if (healthSystem != null)
                    healthSystem.TakeDamage(damage);
                else
                    Debug.Log($"Player took {damage:F1} fall damage!");
            }
        }

        lastYVelocity = controller.velocity.y;
        wasGroundedLastFrame = isGrounded;
    }
}
