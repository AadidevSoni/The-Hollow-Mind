using UnityEngine;

public class ToggleObjectsWithF : MonoBehaviour
{
    public GameObject[] objectsToToggle;
    private bool isVisible = false;
    private bool canToggle = true; // Control flag

    public PlayerInventory playerInventory; // Reference to inventory

    void Update()
    {
        if (canToggle && Input.GetKeyDown(KeyCode.F))
        {
            isVisible = !isVisible;

            foreach (GameObject obj in objectsToToggle)
            {
                // Skip currently equipped item
                if (playerInventory != null && obj == playerInventory.GetEquippedItem())
                    continue;

                obj.SetActive(isVisible);
            }
        }
    }

    // Call this from another script/event to disable F key toggling
    public void DisableToggle()
    {
        canToggle = false;
    }

    // Call this to re-enable toggling
    public void EnableToggle()
    {
        canToggle = true;
    }
}
