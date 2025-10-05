using UnityEngine;
using UnityEngine.UI;

public class VideoFader : MonoBehaviour
{
    public CanvasGroup fadePanel;
    public float fadeDuration = 2f;

    void Start()
    {
        fadePanel.alpha = 1f;
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
        fadePanel.alpha = 0f;
    }
}
