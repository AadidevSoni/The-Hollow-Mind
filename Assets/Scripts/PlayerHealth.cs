using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public HealthBar healthBar;
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Audio")]
    public AudioSource audioSource; // assign in inspector
    public AudioClip hurtClip;      // the yell sound

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
            healthBar.SetHealth(currentHealth);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        if (healthBar != null)
            healthBar.SetHealth(currentHealth);

        PlayHurtSound();
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        if (healthBar != null)
            healthBar.SetHealth(currentHealth);
    }

    private void PlayHurtSound()
    {
        if (audioSource != null && hurtClip != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f); // slight variation
            audioSource.PlayOneShot(hurtClip, 1f);
        }
    }
}
