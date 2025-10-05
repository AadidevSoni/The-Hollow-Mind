using UnityEngine;

public class TorchController : MonoBehaviour
{
    public Light torchLight;
    public AudioSource clickSource;
    private bool isOn = false;

    void Start()
    {
        if (torchLight == null)
            torchLight = GetComponentInChildren<Light>();

        torchLight.enabled = true;
        isOn = true;
    }


    public void ToggleTorch()
    {
        isOn = !isOn;

        if (torchLight != null)
            torchLight.enabled = isOn;

        if (clickSource != null)
            clickSource.Play();
    }
}
