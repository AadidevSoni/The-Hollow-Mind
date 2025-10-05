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
    public PlayerControlling playerController;
    public AudioMixer masterMixer;
    public Slider sensitivitySlider;
    public Slider volumeSlider;

    [Header("Sensitivity Range")]
    public float minSensitivity = 50f;
    public float maxSensitivity = 1000f;

    void Start()
    {
        if (playerController != null)
        {
            playerController.sensitivity = 300f;
            sensitivitySlider.minValue = minSensitivity;
            sensitivitySlider.maxValue = maxSensitivity;

            sensitivitySlider.value = playerController.sensitivity;

            sensitivitySlider.onValueChanged.RemoveAllListeners();
            sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
        }

        if (masterMixer != null)
        {
            float currentVolume;
            if (masterMixer.GetFloat("MasterVolume", out currentVolume))
            {
                volumeSlider.value = Mathf.Pow(10, currentVolume / 20f);
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
