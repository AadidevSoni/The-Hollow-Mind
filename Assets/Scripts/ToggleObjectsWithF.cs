using UnityEngine;

public class ToggleObjectsWithF : MonoBehaviour
{
    [Header("Objects to Toggle")]
    public GameObject[] objectsToToggle;

    [Header("Player Inventory (Optional)")]
    public PlayerInventory playerInventory;

    private bool isVisible = false;

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

    private void OnFKeyPressed()
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
