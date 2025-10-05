using UnityEngine;
using UnityEngine.UI;

public class TorchManager : MonoBehaviour
{
    [Header("Torch Settings")]
    public Light torchLight;               // Assign your torch Light here
    public Slider torchSlider;             // Assign your UI Slider here
    public float maxTorch = 100f;          // Max torch energy
    public float drainRate = 10f;          // Drain rate when torch ON
    public float refillRate = 5f;          // Refill rate when torch OFF in real world
    public float demonDrainRate = 2f;      // Drain rate when torch OFF in demon world

    [Header("References")]
    public FKeyManager fKeyManager;        // Assign your FKeyManager
    public MusicManager musicManager;      // Assign your MusicManager

    private float currentTorch;

    void Start()
    {
        currentTorch = maxTorch;

        // Respect the current light state in scene
        if (torchSlider != null)
        {
            torchSlider.minValue = 0;
            torchSlider.maxValue = maxTorch;
            torchSlider.value = currentTorch;
        }
    }

    void Update()
    {
        HandleTorchToggle();
        HandleTorchLogic();
        UpdateUI();
    }

    void HandleTorchToggle()
    {
        // Right-click toggles torch light
        if (Input.GetMouseButtonDown(1) && torchLight != null)
        {
            torchLight.enabled = !torchLight.enabled;
        }
    }

    void HandleTorchLogic()
    {
        bool inDemonWorld = musicManager != null && musicManager.IsInDemonDimension;

        if (torchLight != null && torchLight.enabled)
        {
            // Torch drains when ON in both dimensions
            currentTorch -= drainRate * Time.deltaTime;
        }
        else
        {
            if (inDemonWorld)
            {
                // Torch drains slightly even when OFF in demon world
                currentTorch -= demonDrainRate * Time.deltaTime;
            }
            else
            {
                // Torch refills when OFF in real world
                currentTorch += refillRate * Time.deltaTime;
            }
        }

        currentTorch = Mathf.Clamp(currentTorch, 0f, maxTorch);

        // Automatically turn off if depleted
        if (currentTorch <= 0f && torchLight.enabled)
        {
            torchLight.enabled = false;
        }
    }

    void UpdateUI()
    {
        if (torchSlider != null)
        {
            torchSlider.value = currentTorch;
        }
    }
}
