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
        float t = 0f;
        fadePanel.blocksRaycasts = true; // Make sure input is blocked during fade

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        fadePanel.alpha = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
