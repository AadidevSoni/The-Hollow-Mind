using UnityEngine;

public class CursorLock : MonoBehaviour
{
    private bool isLocked = true;
    public bool allowUnlock = true; // allow external control

    void Update()
    {
        // Toggle lock with Escape only if allowed
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

    // External call to unlock cursor (e.g., after death)
    public void UnlockCursor()
    {
        isLocked = false;
        allowUnlock = false; // prevent locking again
    }

    // Optional: relock cursor (if needed)
    public void LockCursor()
    {
        isLocked = true;
        allowUnlock = true;
    }
}
