using UnityEngine;
using TMPro;

public class FKeyCooldown : MonoBehaviour
{
    public TextMeshProUGUI cooldownText;
    public float cooldownDuration = 5f;

    private float timer = 0f;

    private void Start()
    {
        if (FKeyManager.Instance != null)
        {
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
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            cooldownText.text = Mathf.CeilToInt(timer).ToString();

            if (timer <= 0f)
            {
                timer = 0f;
                cooldownText.text = "";

                if (FKeyManager.Instance != null)
                    FKeyManager.Instance.EnableFKey();
            }
        }
    }

    private void OnFKeyPressedHandler()
    {
        if (FKeyManager.Instance != null)
            FKeyManager.Instance.DisableFKey();

        timer = cooldownDuration;
        cooldownText.text = Mathf.CeilToInt(timer).ToString();
    }
}
