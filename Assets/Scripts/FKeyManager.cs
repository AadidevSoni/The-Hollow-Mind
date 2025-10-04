using UnityEngine;
using System;

public class FKeyManager : MonoBehaviour
{
    public static FKeyManager Instance { get; private set; }

    // Event fired when F key is pressed
    public event Action OnFKeyPressed;

    // Controls whether F key is enabled
    public bool IsFKeyEnabled { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (IsFKeyEnabled && Input.GetKeyDown(KeyCode.F))
        {
            OnFKeyPressed?.Invoke();
        }
    }

    // Call this to enable F key
    public void EnableFKey()
    {
        IsFKeyEnabled = true;
        Debug.Log("F key enabled!");
    }

    // Call this to disable F key
    public void DisableFKey()
    {
        IsFKeyEnabled = false;
        Debug.Log("F key disabled!");
    }
}
