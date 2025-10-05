using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Sun_Temple;

public class PlayerHealth : MonoBehaviour
{
    public HealthBar healthBar;
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hurtClip;
    public AudioClip clickClip;

    [Header("UI")]
    public GameObject deathScreen;
    public Button menuButton;

    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
            healthBar.SetHealth(currentHealth);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (deathScreen != null)
            deathScreen.SetActive(false);

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

        if (deathScreen != null)
            deathScreen.SetActive(true);

        CursorLock cursorLock = FindObjectOfType<CursorLock>();
        if (cursorLock != null)
        {
            cursorLock.UnlockCursor();
        }

        var playerControl = GetComponent<PlayerControlling>();
        if (playerControl != null)
            playerControl.enabled = false;

        var character = GetComponent<CharacterController>();
        if (character != null)
            character.enabled = false;
    }

    private void OnMenuButtonClicked()
    {
        if (audioSource != null && clickClip != null)
        {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(clickClip, 1f);
        }

        SceneManager.LoadScene("MainMenu");
    }
}
