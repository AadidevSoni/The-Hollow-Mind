using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinController : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasGroup fadeCanvas;
    public TextMeshProUGUI typewriterText;
    public TextMeshProUGUI blinkingText;
    public Button mainMenuButton;            // Reference to main menu button

    [Header("Settings")]
    public float fadeDuration = 2f;
    public float typeSpeed = 0.05f;
    public float blinkSpeed = 0.5f;

    [Header("Audio")]
    public AudioSource musicSource;
    public AudioClip winMusic;

    [Header("Other Canvases")]
    public Canvas[] otherCanvases;

    [Header("Cursor Lock Reference")]
    public CursorLock cursorLockScript;

    [Header("Win Text Content")]
    [TextArea] public string winMessage = "You managed to stop the demons from entering the real world by restoring the power of the Sun Temple by destroying all the demon crystals.";
    [TextArea] public string blinkMessage = "YOU WIN";

    private bool winTriggered = false;

    public void TriggerWin()
    {
        if (winTriggered) return;
        winTriggered = true;

        AudioSource[] allSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in allSources)
        {
            if (source != musicSource) source.Stop();
        }
        if (musicSource != null) musicSource.Stop();

        foreach (Canvas c in otherCanvases)
        {
            if (c != null)
                c.gameObject.SetActive(false);
        }

        fadeCanvas.gameObject.SetActive(true);
        fadeCanvas.alpha = 0f;
        fadeCanvas.interactable = false;
        fadeCanvas.blocksRaycasts = false;

        if (cursorLockScript != null)
            cursorLockScript.UnlockCursor();

        PlayerControlling player = FindObjectOfType<PlayerControlling>();
        if (player != null)
            player.enabled = false;

        Time.timeScale = 0f;

        if (blinkingText != null)
        {
            blinkingText.text = blinkMessage;
            StartCoroutine(BlinkText(blinkingText));
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.gameObject.SetActive(false);
        }

        StartCoroutine(FadeInWinScreen());
    }

    private IEnumerator FadeInWinScreen()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            fadeCanvas.alpha = Mathf.Clamp01(timer / fadeDuration);
            yield return null;
        }
        fadeCanvas.alpha = 1f;

        if (musicSource != null && winMusic != null)
        {
            musicSource.clip = winMusic;
            musicSource.Play();
        }

        if (typewriterText != null)
        {
            typewriterText.text = "";
            foreach (char c in winMessage)
            {
                typewriterText.text += c;
                yield return new WaitForSecondsRealtime(typeSpeed);
            }
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.gameObject.SetActive(true);
            mainMenuButton.interactable = true;
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        fadeCanvas.interactable = true;
        fadeCanvas.blocksRaycasts = true;
    }

    private IEnumerator BlinkText(TextMeshProUGUI text)
    {
        while (true)
        {
            text.enabled = !text.enabled;
            yield return new WaitForSecondsRealtime(blinkSpeed);
        }
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
