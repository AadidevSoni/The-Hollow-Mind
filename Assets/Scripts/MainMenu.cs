using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public CanvasGroup fadePanel;      // Full-screen black panel with CanvasGroup
    public float fadeDuration = 1f;    // Duration of fade
    public AudioSource audioSource;    // Assign the audio source on the canvas

    private bool isFading = false;

    public void PlayGame()
    {
        if (!isFading)
            StartCoroutine(FadeOutAudioAndScreen());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator FadeOutAudioAndScreen()
    {
        isFading = true;

        if (fadePanel != null)
            fadePanel.blocksRaycasts = true; // Block input during fade

        float startVolume = audioSource != null ? audioSource.volume : 1f;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float lerp = t / fadeDuration;

            // Fade panel
            if (fadePanel != null)
                fadePanel.alpha = Mathf.Lerp(0f, 1f, lerp);

            // Fade audio
            if (audioSource != null)
                audioSource.volume = Mathf.Lerp(startVolume, 0f, lerp);

            yield return null;
        }

        // Ensure fully black and audio silent
        if (fadePanel != null)
            fadePanel.alpha = 1f;
        if (audioSource != null)
            audioSource.volume = 0f;

        // Load next scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
