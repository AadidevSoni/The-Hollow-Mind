using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public CanvasGroup fadePanel;     // Black panel covering the screen
    public float fadeDuration = 1f;   // Seconds to fade
    public string sceneName;
    public float changeTime;
    public float fKeyDelay;
    public AudioSource audioSource;    // Assign the audio source on the canvas

    void Update()
    {
        changeTime -= Time.deltaTime;
        fKeyDelay -= Time.deltaTime;

        if (fKeyDelay < 0 && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(FadeOutAndLoadScene());
        }

        if (changeTime < 0)
        {
            StartCoroutine(FadeOutAndLoadScene());
        }
    }

    IEnumerator FadeOutAndLoadScene()
    {
        float startVolume = audioSource != null ? audioSource.volume : 1f;
        float t = 0f;
        fadePanel.blocksRaycasts = true; // Make sure input is blocked during fade

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

        SceneManager.LoadScene(sceneName);
    }
}
