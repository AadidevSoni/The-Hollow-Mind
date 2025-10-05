using UnityEngine;
using System.Collections;

public class DimensionSwitcher : MonoBehaviour
{
    public MusicManager musicManager;
    private bool inDemon = false;

    // Property to check current dimension
    public bool IsInDemonDimension => inDemon;

    private void OnEnable()
    {
        StartCoroutine(WaitForFKeyManager());
    }

    private IEnumerator WaitForFKeyManager()
    {
        while (FKeyManager.Instance == null)
            yield return null;

        FKeyManager.Instance.OnFKeyPressed += OnFKeyPressed;
    }

    private void OnDisable()
    {
        if (FKeyManager.Instance != null)
            FKeyManager.Instance.OnFKeyPressed -= OnFKeyPressed;
    }

    private void OnFKeyPressed()
    {
        inDemon = !inDemon;

        if (inDemon)
            musicManager.EnterDemonDimension();
        else
            musicManager.ExitDemonDimension();
    }
}
