using UnityEngine;

public class FKeyManager : MonoBehaviour
{
    public static bool isFKeyEnabled = false;

    // Call this from the InteractableObject when player interacts
    public void EnableFKey()
    {
        isFKeyEnabled = true;
    }
}
