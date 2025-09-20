using UnityEngine;

public class DimensionSwitcher : MonoBehaviour
{
    public MusicManager musicManager;
    private bool inDemon = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            inDemon = !inDemon;
            if (inDemon) musicManager.EnterDemonDimension();
            else musicManager.ExitDemonDimension();
        }
    }
}
