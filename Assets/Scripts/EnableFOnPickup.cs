using UnityEngine;

public class EnableFOnPickup : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Dialogue dialogue; // assign in inspector
    [TextArea(2, 3)]
    public string newObjectiveText; // the objective to set

    [Header("Settings")]
    public float interactionRange = 3f;

    private bool hasTriggered = false; // to ensure it triggers only once

    private void Update()
    {
        if (hasTriggered || player == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            float distance = Vector3.Distance(player.position, transform.position);
            if (distance <= interactionRange)
            {
                // Enable F key
                if (FKeyManager.Instance != null)
                    FKeyManager.Instance.EnableFKey();

                // Trigger dialogue
                if (dialogue != null)
                {
                    DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
                    if (dialogueManager != null)
                    {
                        dialogueManager.StartDialogue(dialogue);
                    }
                }

                // Set objective
                if (!string.IsNullOrEmpty(newObjectiveText))
                {
                    ObjectiveManager.Instance?.SetObjective("OBJECTIVE: " + newObjectiveText);
                }

                hasTriggered = true;
                Debug.Log("F key enabled, dialogue started, and objective set.");
            }
        }
    }
}
