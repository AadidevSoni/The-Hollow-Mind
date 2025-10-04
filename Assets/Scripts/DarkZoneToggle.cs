using UnityEngine;

public class DarkZoneToggle : MonoBehaviour
{
    public GameObject darkZone;
    private bool isActive = false;

    private void OnEnable()
    {
        FKeyManager.Instance.OnFKeyPressed += ToggleDarkZone;
    }

    private void OnDisable()
    {
        if (FKeyManager.Instance != null)
            FKeyManager.Instance.OnFKeyPressed -= ToggleDarkZone;
    }

    private void ToggleDarkZone()
    {
        isActive = !isActive;
        darkZone.SetActive(isActive);
    }
}
