using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    private void Start()
    {
        StartCoroutine("DialogueOnStart");
    }
    public void TriggerDialogue()
    {
        //FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        //FindObjectOfType<DialogueManager>().LoadDialogue(dialogue);
    }

    IEnumerator DialogueOnStart()
    {
        yield return new WaitForSeconds(0.01f);
        TriggerDialogue();
    }
}
