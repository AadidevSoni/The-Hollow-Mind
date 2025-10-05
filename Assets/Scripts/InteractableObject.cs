using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public bool isOneTime = true;
    private bool interacted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (interacted) return;

        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
        if (playerInteraction != null)
        {
            FKeyManager fKeyManager = FindObjectOfType<FKeyManager>();
            if (fKeyManager != null)
                fKeyManager.EnableFKey();

            interacted = true;

            if (isOneTime)
                gameObject.SetActive(false);
        }
    }
}
