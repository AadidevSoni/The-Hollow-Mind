using UnityEngine;
using System.Collections;

public class FadeScene : MonoBehaviour
{
    public CanvasGroup fadePanel;    // Assign your black full-screen panel
    public float fadeDuration = 1f;  // Duration of fade-in

    void Start()
    {
        if (fadePanel != null)
        {
            fadePanel.alpha = 1f; // Start fully black
            StartCoroutine(FadeIn());
        }
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadePanel.alpha = 1f - (t / fadeDuration);
            yield return null;
        }
        fadePanel.alpha = 0f; // Fully transparent
    }
}
