using UnityEngine;
using TMPro;

public class Interacting : MonoBehaviour
{
    [Header("Camera & Distance")]
    public Camera playerCamera;
    public float interactDistance = 3f;

    [Header("UI")]
    public TextMeshProUGUI interactText;

    [Header("Layer Mask")]
    public LayerMask interactableLayer;

    private GameObject currentInteractable;

    void Update()
    {
        CheckForInteractable();
    }

    void CheckForInteractable()
    {
        currentInteractable = null;
        interactText.enabled = false;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayer))
        {
            currentInteractable = hit.collider.gameObject;
            interactText.text = "Press E to Interact";
            interactText.enabled = true;
        }
    }
}
