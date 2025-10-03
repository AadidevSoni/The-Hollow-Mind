using UnityEngine;

public class TorchController : MonoBehaviour
{
    public Light torchLight;         // Assign the Light component in Inspector
    public AudioSource clickSource;  // Assign an AudioSource with click sound
    private bool isOn = false;

    void Start()
    {
        if (torchLight == null)
            torchLight = GetComponentInChildren<Light>();

        // Start with torch ON by default
        torchLight.enabled = true;
        isOn = true; // make sure the toggle state matches
    }


    // This is now called by the inventory system
    public void ToggleTorch()
    {
        isOn = !isOn;

        if (torchLight != null)
            torchLight.enabled = isOn;

        if (clickSource != null)
            clickSource.Play();
    }
}
