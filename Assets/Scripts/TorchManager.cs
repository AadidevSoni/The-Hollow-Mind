using UnityEngine;
using UnityEngine.UI;

public class TorchManager : MonoBehaviour
{
    [Header("Torch Settings")]
    public Light torchLight;
    public Slider torchSlider;
    public float maxTorch = 100f;
    public float drainRate = 10f;
    public float refillRate = 5f;
    public float demonDrainRate = 2f;

    [Header("References")]
    public FKeyManager fKeyManager;
    public MusicManager musicManager;

    private float currentTorch;

    void Start()
    {
        currentTorch = maxTorch;

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
            currentTorch -= drainRate * Time.deltaTime;
        }
        else
        {
            if (inDemonWorld)
            {
                currentTorch -= demonDrainRate * Time.deltaTime;
            }
            else
            {
                currentTorch += refillRate * Time.deltaTime;
            }
        }

        currentTorch = Mathf.Clamp(currentTorch, 0f, maxTorch);

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
