using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public bool canUseF = false; // F key initially disabled

    void Update()
    {
        if (canUseF && Input.GetKeyDown(KeyCode.F))
        {
            Interact();
        }
    }

    void Interact()
    {
        // Your interaction logic here
        Debug.Log("Interacted with object using F key!");
    }

    // Call this when player interacts with a special object to enable F
    public void EnableFKey()
    {
        canUseF = true;
        Debug.Log("F key is now enabled!");
    }
}
