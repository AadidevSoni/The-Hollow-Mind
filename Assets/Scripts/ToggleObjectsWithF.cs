using UnityEngine;

public class ToggleObjectsWithF : MonoBehaviour
{
    public GameObject[] objectsToToggle; // Assign in Inspector
    private bool isVisible = false;
    private bool canToggle = true; // Control flag

    void Update()
    {
        if (canToggle && Input.GetKeyDown(KeyCode.F))
        {
            isVisible = !isVisible;

            foreach (GameObject obj in objectsToToggle)
            {
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
