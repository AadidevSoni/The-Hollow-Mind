using UnityEngine;
using System;

public class FKeyManager : MonoBehaviour
{
    public static FKeyManager Instance { get; private set; }

    public event Action OnFKeyPressed;

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

    public void EnableFKey()
    {
        IsFKeyEnabled = true;
        Debug.Log("F key enabled!");
    }
    public void DisableFKey()
    {
        IsFKeyEnabled = false;
        Debug.Log("F key disabled!");
    }
}
