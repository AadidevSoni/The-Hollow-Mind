using UnityEngine;
using System.Collections;

public class ToggleVisibility : MonoBehaviour
{
    [Header("Object to Toggle")]
    public GameObject objectToHide;

    [Header("Dimension Switcher Reference")]
    public DimensionSwitcher dimensionSwitcher;

    private bool isVisible = true; // Start visible by default

    private void OnEnable()
    {
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

    private IEnumerator WaitForFKeyManager()
    {
        while (FKeyManager.Instance == null)
            yield return null;

        FKeyManager.Instance.OnFKeyPressed += OnFKeyPressed;
    }

    private void Start()
    {
        if (objectToHide != null)
            objectToHide.SetActive(isVisible); // visible at start
    }

    private void Update()
    {
        // Force object visible in Real World
        if (dimensionSwitcher != null && !dimensionSwitcher.IsInDemonDimension)
        {
            if (objectToHide != null && !objectToHide.activeSelf)
            {
                objectToHide.SetActive(true);
            }
        }
    }


    private void OnFKeyPressed()
    {
        // Only toggle in Demon World
        if (dimensionSwitcher != null && dimensionSwitcher.IsInDemonDimension)
        {
            if (objectToHide != null)
            {
                isVisible = !isVisible;
                objectToHide.SetActive(isVisible);
            }
        }
    }
}
