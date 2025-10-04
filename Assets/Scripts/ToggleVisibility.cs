using UnityEngine;
using System.Collections;

public class ToggleVisibility : MonoBehaviour
{
    public GameObject objectToHide;  // Assign the object in Inspector
    private bool isVisible = true;    // Start visible

    private void OnEnable()
    {
        // Wait until FKeyManager singleton exists
        StartCoroutine(WaitForFKeyManager());
    }

    private IEnumerator WaitForFKeyManager()
    {
        while (FKeyManager.Instance == null)
            yield return null;

        FKeyManager.Instance.OnFKeyPressed += OnFKeyPressed;
    }

    private void OnDisable()
    {
        if (FKeyManager.Instance != null)
            FKeyManager.Instance.OnFKeyPressed -= OnFKeyPressed;
    }

    private void Start()
    {
        // Ensure initial state matches isVisible
        if (objectToHide != null)
            objectToHide.SetActive(isVisible);
    }

    private void OnFKeyPressed()
    {
        // Only toggle if F key is enabled and object exists
        if (FKeyManager.Instance != null && FKeyManager.Instance.IsFKeyEnabled && objectToHide != null)
        {
            isVisible = !isVisible;
            objectToHide.SetActive(isVisible);
        }
    }
}
