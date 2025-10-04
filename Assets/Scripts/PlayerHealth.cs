using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Sun_Temple; // <-- Added this to recognize CursorLock

public class PlayerHealth : MonoBehaviour
{
    public HealthBar healthBar;
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Audio")]
    public AudioSource audioSource; // assign in inspector
    public AudioClip hurtClip;      // the yell sound
    public AudioClip clickClip;     // menu button click sound

    [Header("UI")]
    public GameObject deathScreen;  // assign your death panel in inspector
    public Button menuButton;       // assign button in inspector

    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
            healthBar.SetHealth(currentHealth);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Hide death screen at start
        if (deathScreen != null)
            deathScreen.SetActive(false);

        // Assign menu button click
        if (menuButton != null)
            menuButton.onClick.AddListener(OnMenuButtonClicked);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        if (healthBar != null)
            healthBar.SetHealth(currentHealth);

        PlayHurtSound();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        if (healthBar != null)
            healthBar.SetHealth(currentHealth);
    }

    private void PlayHurtSound()
    {
        if (audioSource != null && hurtClip != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(hurtClip, 1f);
        }
    }

    private void Die()
    {
        isDead = true;

        // Show death screen
        if (deathScreen != null)
            deathScreen.SetActive(true);

        // Unlock cursor using CursorLock
        CursorLock cursorLock = FindObjectOfType<CursorLock>();
        if (cursorLock != null)
        {
            cursorLock.UnlockCursor();
        }

        // Disable player controls
        var playerControl = GetComponent<PlayerControlling>();
        if (playerControl != null)
            playerControl.enabled = false;

        // Disable character controller to stop movement
        var character = GetComponent<CharacterController>();
        if (character != null)
            character.enabled = false;
    }

    private void OnMenuButtonClicked()
    {
        // Play click sound
        if (audioSource != null && clickClip != null)
        {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(clickClip, 1f);
        }

        // Load menu scene
        SceneManager.LoadScene("MainMenu"); // ensure scene name matches
    }
}
