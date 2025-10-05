using UnityEngine;
using TMPro;

public class FKeyCooldown : MonoBehaviour
{
    public TextMeshProUGUI cooldownText; // Assign in Inspector
    public float cooldownDuration = 5f;  // Duration before F can be pressed again

    private float timer = 0f;

    private void Start()
    {
        if (FKeyManager.Instance != null)
        {
            // Subscribe to F key press event
            FKeyManager.Instance.OnFKeyPressed += OnFKeyPressedHandler;
        }
    }

    private void OnDestroy()
    {
        if (FKeyManager.Instance != null)
        {
            FKeyManager.Instance.OnFKeyPressed -= OnFKeyPressedHandler;
        }
    }

    private void Update()
    {
        // If timer is active, count down
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            cooldownText.text = Mathf.CeilToInt(timer).ToString();

            if (timer <= 0f)
            {
                timer = 0f;
                cooldownText.text = "";

                // Re-enable F key when timer ends
                if (FKeyManager.Instance != null)
                    FKeyManager.Instance.EnableFKey();
            }
        }
    }

    private void OnFKeyPressedHandler()
    {
        // Disable F key immediately
        if (FKeyManager.Instance != null)
            FKeyManager.Instance.DisableFKey();

        // Start cooldown
        timer = cooldownDuration;
        cooldownText.text = Mathf.CeilToInt(timer).ToString();
    }
}
