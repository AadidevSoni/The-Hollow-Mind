using UnityEngine;
using System.Collections;

public class ToggleObjectsWithF : MonoBehaviour
{
    public GameObject[] objectsToToggle;
    private bool isVisible = false;
    public PlayerInventory playerInventory;

    private void OnEnable()
    {
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

    private void OnFKeyPressed()
    {
        if (FKeyManager.Instance != null && FKeyManager.Instance.IsFKeyEnabled)
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
