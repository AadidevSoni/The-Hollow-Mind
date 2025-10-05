using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text dialogueText;
    public float typingSpeed = 0.03f;
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
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(1f);
        dialogueText.text = "";
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
