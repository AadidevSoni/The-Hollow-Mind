using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectDialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    private bool isPlayerInRange = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Player entered trigger zone.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Player left trigger zone.");
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed, starting dialogue...");
            FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
            Destroy(gameObject); // remove item after interaction
        }
    }
}
