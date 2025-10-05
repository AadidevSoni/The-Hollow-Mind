using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    void Start()
    {
        Invoke("TriggerDialogue", 2f);
    }

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}

