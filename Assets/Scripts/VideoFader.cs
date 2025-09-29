using UnityEngine;
using UnityEngine.UI;

public class VideoFader : MonoBehaviour
{
    public CanvasGroup fadePanel;  // Assign black panel
    public float fadeDuration = 2f; // Seconds

    void Start()
    {
        fadePanel.alpha = 1f; // Fully visible
        StartCoroutine(FadeOut());
    }

    private System.Collections.IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadePanel.alpha = 1f - (t / fadeDuration);
            yield return null;
        }
        fadePanel.alpha = 0f; // fully transparent
    }
}
