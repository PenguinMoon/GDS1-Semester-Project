using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{

    public List<string> sentences;
    public TextMeshProUGUI dialogueText;

    public bool isTyping = false;

    public void DisplaySentenceAtIndex(int index)
    {
        if (index < 0 || index >= sentences.Count)
            dialogueText.text = "";
        else
            StartCoroutine(TypeSentence(sentences[index]));
    }

    public void DisplayNextSentence()
    {
        Debug.Log("Function depreceated");
    }

    IEnumerator TypeSentence(string Sentence)
    {
        isTyping = true;
        dialogueText.text = "";

        yield return new WaitForSeconds(1f);

        foreach (char letter in Sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.03f);
        }
        isTyping = false;
    }

    void EndDialogue()
    {

    }
}
