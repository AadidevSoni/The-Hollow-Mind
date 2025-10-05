using UnityEngine;

public class CursorLock : MonoBehaviour
{
    private bool isLocked = true;
    public bool allowUnlock = true;

    void Update()
    {
        if (allowUnlock && Input.GetKeyDown(KeyCode.Escape))
        {
            isLocked = !isLocked;
        }

        if (isLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void UnlockCursor()
    {
        isLocked = false;
        allowUnlock = false;
    }

    public void LockCursor()
    {
        isLocked = true;
        allowUnlock = true;
    }
}
