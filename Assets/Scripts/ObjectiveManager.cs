using UnityEngine;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    [Header("UI Reference")]
    public TextMeshProUGUI objectiveText;

    [Header("Fade Settings (Optional)")]
    public float fadeDuration = 0.5f;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        canvasGroup = objectiveText.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = objectiveText.gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void Start()
    {
        SetObjective("OBJETIVE: Find the flashlight");
    }

    public void SetObjective(string newObjective)
    {
        StopAllCoroutines();
        StartCoroutine(FadeObjectiveText(newObjective));
    }

    private System.Collections.IEnumerator FadeObjectiveText(string newText)
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1 - (t / fadeDuration);
            yield return null;
        }

        objectiveText.text = newText;

        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = t / fadeDuration;
            yield return null;
        }

        canvasGroup.alpha = 1;
    }
}
