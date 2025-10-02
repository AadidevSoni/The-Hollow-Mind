using UnityEngine;

public class ToggleVisibility : MonoBehaviour
{
    public GameObject objectToHide;  // Assign the object in Inspector
    private bool isVisible = true;   // Start visible

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isVisible = !isVisible;
            objectToHide.SetActive(isVisible);
        }
    }
}
