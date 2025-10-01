using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject settingsPanel;
    private bool isOpen = false;

    [Header("References")]
    public PlayerControlling playerController; // Your player script
    public AudioMixer masterMixer;             // Assign your AudioMixer
    public Slider sensitivitySlider;           // Slider for mouse sensitivity
    public Slider volumeSlider;                // Slider for master volume

    [Header("Sensitivity Range")]
    public float minSensitivity = 50f;
    public float maxSensitivity = 1000f;

    void Start()
    {
        // Ensure sliders are initialized safely
        if (playerController != null)
        {
            playerController.sensitivity = 300f;     // Force default
            sensitivitySlider.minValue = minSensitivity;
            sensitivitySlider.maxValue = maxSensitivity;

            // Set slider value to player's current sensitivity
            sensitivitySlider.value = playerController.sensitivity;

            // Add listener via script (avoids Inspector override)
            sensitivitySlider.onValueChanged.RemoveAllListeners();
            sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
        }

        if (masterMixer != null)
        {
            float currentVolume;
            if (masterMixer.GetFloat("MasterVolume", out currentVolume))
            {
                volumeSlider.value = Mathf.Pow(10, currentVolume / 20f); // Convert dB to 0–1
            }

            volumeSlider.onValueChanged.RemoveAllListeners();
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettings();
        }
    }

    public void ToggleSettings()
    {
        isOpen = !isOpen;
        settingsPanel.SetActive(isOpen);

        Time.timeScale = isOpen ? 0f : 1f;
        Cursor.visible = isOpen;
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void SetSensitivity(float value)
    {
        if (playerController != null)
        {
            playerController.sensitivity = value;
        }
    }

    public void SetVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        masterMixer.SetFloat("MasterVolume", dB);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
