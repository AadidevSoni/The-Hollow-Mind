using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public GameObject settingsPanel; // Assign the UI panel in Inspector
    private bool isOpen = false;

    void Update()
    {
        // Toggle with Escape key when in-game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettings();
        }
    }

    public void ToggleSettings()
    {
        isOpen = !isOpen;
        settingsPanel.SetActive(isOpen);

        // Pause game when settings are open
        Time.timeScale = isOpen ? 0f : 1f;

        // Show/unlock cursor in game
        Cursor.visible = isOpen;
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
