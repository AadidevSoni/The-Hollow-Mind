using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public bool canUseF = false;

    void Update()
    {
        if (canUseF && Input.GetKeyDown(KeyCode.F))
        {
            Interact();
        }
    }

    void Interact()
    {
        Debug.Log("Interacted with object using F key!");
    }

    public void EnableFKey()
    {
        canUseF = true;
        Debug.Log("F key is now enabled!");
    }
}
