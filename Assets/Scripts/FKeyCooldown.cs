using UnityEngine;
using TMPro;
using System.Collections;

public class FKeyCooldown : MonoBehaviour
{
    public TextMeshProUGUI cooldownText;
    public float cooldownDuration = 5f;

    [Header("Demon World Settings")]
    public GameObject demonWorldIndicator;
    public PlayerHealth playerHealth;
    public int demonDamagePerSecond = 1;
    [HideInInspector]
    public float timer = 0f;
    public float Timer => timer;

    [Header("Demon Scaling")]
    public DemonAI demonAI;
    public GameObject objectToCheck;
    public float sightIncreasePerSecond = 1f;
    public float autoSenseIncreasePerSecond = 1f;

    private bool isTakingDemonDamage = false;

    public static FKeyCooldown Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (FKeyManager.Instance != null)
            FKeyManager.Instance.OnFKeyPressed += OnFKeyPressedHandler;
    }

    private void Start()
    {
        if (FKeyManager.Instance != null)
            FKeyManager.Instance.OnFKeyPressed += OnFKeyPressedHandler;
    }

    private void OnDestroy()
    {
        if (FKeyManager.Instance != null)
            FKeyManager.Instance.OnFKeyPressed -= OnFKeyPressedHandler;
    }

    private void Update()
    {
        // Countdown the timer
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

        if (demonAI != null && objectToCheck != null && !objectToCheck.activeSelf && timer <= 0f)
        {
            demonAI.sightRange += sightIncreasePerSecond * Time.deltaTime;
            demonAI.autoSenseRadius += autoSenseIncreasePerSecond * Time.deltaTime;
        }

        if (!isTakingDemonDamage && timer <= 0f && demonWorldIndicator != null && demonWorldIndicator.activeSelf)
        {
            if (playerHealth != null)
                StartCoroutine(ApplyDemonDamage());
        }
    }

    private void OnFKeyPressedHandler()
    {
        if (FKeyManager.Instance != null)
            FKeyManager.Instance.DisableFKey();

        timer = cooldownDuration;
        cooldownText.text = Mathf.CeilToInt(timer).ToString();
    }

    private IEnumerator ApplyDemonDamage()
    {
        isTakingDemonDamage = true;

        while (demonWorldIndicator != null && demonWorldIndicator.activeSelf && playerHealth != null)
        {
            playerHealth.TakeDamage(demonDamagePerSecond);
            yield return new WaitForSecondsRealtime(1f);
        }

        isTakingDemonDamage = false;
    }
}
