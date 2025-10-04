using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;   // for TextMeshPro

public class DialogueManager : MonoBehaviour
{
    public TMP_Text dialogueText;       // The text UI for sentences
    public float typingSpeed = 0.03f;   // Adjustable typing speed
    private Queue<string> sentences;

    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        StopAllCoroutines();
        StartCoroutine(DisplaySentences());
    }

    IEnumerator DisplaySentences()
    {
        while (sentences.Count > 0)
        {
            string sentence = sentences.Dequeue();
            yield return StartCoroutine(TypeSentence(sentence));
            yield return new WaitForSeconds(1f); // pause between sentences
        }

        // After all sentences are shown, wait 1 sec and clear
        yield return new WaitForSeconds(1f);
        dialogueText.text = "";
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed); // uses adjustable speed
        }
    }
}
