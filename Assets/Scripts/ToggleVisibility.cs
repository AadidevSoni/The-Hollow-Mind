using UnityEngine;

public class ToggleVisibility : MonoBehaviour
{
    [Header("Object to Toggle")]
    public GameObject objectToHide;  // Assign in Inspector

    private bool isVisible = true;

    private void OnEnable()
    {
        // Subscribe to F key press event
        if (FKeyManager.Instance != null)
            FKeyManager.Instance.OnFKeyPressed += OnFKeyPressed;
        else
            StartCoroutine(WaitForFKeyManager());
    }

    private void OnDisable()
    {
        if (FKeyManager.Instance != null)
            FKeyManager.Instance.OnFKeyPressed -= OnFKeyPressed;
    }

    private System.Collections.IEnumerator WaitForFKeyManager()
    {
        while (FKeyManager.Instance == null)
            yield return null;

        FKeyManager.Instance.OnFKeyPressed += OnFKeyPressed;
    }

    private void Start()
    {
        if (objectToHide != null)
            objectToHide.SetActive(isVisible);
    }

    private void OnFKeyPressed()
    {
        if (objectToHide != null)
        {
            isVisible = !isVisible;
            objectToHide.SetActive(isVisible);
        }
    }
}
