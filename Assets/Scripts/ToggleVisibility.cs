using UnityEngine;
using System.Collections;

public class ToggleVisibility : MonoBehaviour
{
    [Header("Object to Toggle")]
    public GameObject objectToHide;

    [Header("Dimension Switcher Reference")]
    public DimensionSwitcher dimensionSwitcher;

    private bool isVisible = true;
    private bool lastDimensionWasDemon = false;

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
            objectToHide.SetActive(isVisible);
    }

    private void Update()
    {
        if (dimensionSwitcher == null || objectToHide == null) return;

        if (!dimensionSwitcher.IsInDemonDimension)
        {
            objectToHide.SetActive(true);
            lastDimensionWasDemon = false;
        }
        else
        {
            if (!lastDimensionWasDemon)
            {
                isVisible = false;
                objectToHide.SetActive(isVisible);
                lastDimensionWasDemon = true;
            }
        }
    }

    private void OnFKeyPressed()
    {
        if (dimensionSwitcher != null && dimensionSwitcher.IsInDemonDimension && objectToHide != null)
        {
            isVisible = !isVisible;
            objectToHide.SetActive(isVisible);
        }
    }
}
