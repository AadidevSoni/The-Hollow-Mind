using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMainMenu : MonoBehaviour
{
    public CanvasGroup fadePanel;
    public float fadeDuration = 1f;
    public AudioSource audioSource;

    private bool isFading = false;

    public void OnMainMenuButtonClick()
    {
        if (!isFading)
            StartCoroutine(FadeOutAndLoad("MainMenu"));
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        isFading = true;
        if (fadePanel != null) fadePanel.blocksRaycasts = true;

        float startVolume = audioSource != null ? audioSource.volume : 1f;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float lerp = t / fadeDuration;

            if (fadePanel != null)
                fadePanel.alpha = Mathf.Lerp(0f, 1f, lerp);

            if (audioSource != null)
                audioSource.volume = Mathf.Lerp(startVolume, 0f, lerp);

            yield return null;
        }

        if (fadePanel != null) fadePanel.alpha = 1f;
        if (audioSource != null) audioSource.volume = 0f;

        SceneManager.LoadScene("MainMenu");
    }
}
