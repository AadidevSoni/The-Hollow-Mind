using UnityEngine;
using System.Collections;

public class ToggleObjectsWithF : MonoBehaviour
{
    [Header("Objects to Toggle")]
    public GameObject[] objectsToToggle;

    [Header("Player Inventory (Optional)")]
    public PlayerInventory playerInventory;

    [Header("Dimension Switcher Reference")]
    public DimensionSwitcher dimensionSwitcher;

    private bool isVisible = false;

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

    private void Update()
    {
        if (dimensionSwitcher != null && !dimensionSwitcher.IsInDemonDimension)
        {
            foreach (GameObject obj in objectsToToggle)
            {
                if (playerInventory != null && obj == playerInventory.GetEquippedItem())
                    continue;

                obj.SetActive(false);
            }
            isVisible = false;
        }
    }

    private void OnFKeyPressed()
    {
        if (dimensionSwitcher != null && dimensionSwitcher.IsInDemonDimension)
        {
            isVisible = !isVisible;

            foreach (GameObject obj in objectsToToggle)
            {
                if (playerInventory != null && obj == playerInventory.GetEquippedItem())
                    continue;

                obj.SetActive(isVisible);
            }
        }
    }
}
