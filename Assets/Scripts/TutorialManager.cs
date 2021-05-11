using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    //Tutorial Objects
    [SerializeField] Player player;
    [SerializeField] TurretPlate turretPlate;
    [SerializeField] TurretPlate turretPlate2;

    DialogueManager dialogueManager;
    int stepIndex;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (stepIndex == 0 && player.selectedObject.tag == "Turret") // Player picks up initial turret
        {
            stepIndex++;
            dialogueManager.DisplayNextSentence();
        }

        if (stepIndex == 1 && turretPlate.placedObject.tag == "Turret") // Player places turret down on correct plate
        {
            stepIndex++;
            dialogueManager.DisplayNextSentence();
        }

        if (stepIndex == 2) // Wait for enemies to spawn currency
        {
            stepIndex++;
            StartCoroutine("AutoDialogue", 3f);
        }

        if (stepIndex == 3 && player.inventory["Bits"] >= 3) // Wait for the player to pick up at least 3 currency
        {
            stepIndex++;
            dialogueManager.DisplayNextSentence();
        }

        if (stepIndex == 4 && player.selectedObject.tag == "Turret") // Wait for the player to craft new turret
        {
            stepIndex++;
            dialogueManager.DisplayNextSentence();
        }

        if (stepIndex == 5 && turretPlate2.placedObject.tag == "Turret") // Wait for the player to place new turret down
        {
            stepIndex++;
            dialogueManager.DisplayNextSentence();
        }

        if (stepIndex == 6) // Wait for a few seconds while the player whacks the turret
        {
            stepIndex++;
            StartCoroutine("AutoDialogue", 8f);
        }

        if (stepIndex == 7 && player.selectedObject.tag == "Object") // Wait player to buy ammo
        {
            stepIndex++;
            dialogueManager.DisplayNextSentence();
        }

        if (stepIndex == 8 && player.selectedObject == null) // Wait player to place ammo
        {
            stepIndex++;
            dialogueManager.DisplayNextSentence();
        }

        if (stepIndex == 9) // Display congrats text for short time
        {
            stepIndex++;
            StartCoroutine("AutoDialogue", 8f);
        }
    }

    IEnumerator AutoDialogue(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        dialogueManager.DisplayNextSentence();
    }
}
