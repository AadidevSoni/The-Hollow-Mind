using UnityEngine;
using UnityEngine.Rendering.PostProcessing; // only if you’re using Built-in with PostProcessing package

public class DarkZoneToggle : MonoBehaviour
{
    public GameObject darkZone; // assign your DarkZone GameObject in inspector
    private bool isActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isActive = !isActive; // flip state
            darkZone.SetActive(isActive); // turn post process on/off
        }
    }
}
