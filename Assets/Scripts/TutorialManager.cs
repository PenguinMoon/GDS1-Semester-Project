using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    //Tutorial Objects
    [SerializeField] Player player;
    [SerializeField] SmartTurret turret;
    [SerializeField] TurretPlate turretPlate;
    [SerializeField] TurretPlate turretPlate2;
    [SerializeField] GameObject workbench;
    [SerializeField] GameObject workbench2;

    [SerializeField] GameObject arrow;

    DialogueManager dialogueManager;
    MultiplayerManager multiplayerManager;

    [SerializeField]
    int stepIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        multiplayerManager = FindObjectOfType<MultiplayerManager>();
        player = FindObjectOfType<Player>();
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (stepIndex)
        {
            case 0:
            arrow.transform.position = turretPlate.transform.position;
            break;

            case 1:
            arrow.transform.position = turretPlate.transform.position;
            break;

            case 4:
            arrow.transform.position = workbench.transform.position;
            break;

            case 5:
            arrow.transform.position = turretPlate2.transform.position;
            break;

            case 6:
            arrow.transform.position = turret.transform.position;
            break;

            case 7:
            arrow.transform.position = workbench2.transform.position;
            break;

            case 8:
            arrow.transform.position = turret.transform.position;
            break;

            default:
                arrow.transform.position = new Vector3(1000,1000,1000);
            break;
        }

        if (player == null) {
            player = FindObjectOfType<Player>();
            turret = FindObjectOfType<SmartTurret>();


            if (player) {
                dialogueManager.DisplayNextSentence();
                StartCoroutine(AutoDialogue(5f));
            }
            return;
        }

        if (stepIndex == 0 && player.selectedObject.tag == "Turret") // Player picks up initial turret
        {
            stepIndex++;
            dialogueManager.DisplayNextSentence();
        }

        if (stepIndex == 1 && turretPlate.placedObject && turretPlate.placedObject.tag == "Turret") // Player places turret down on correct plate
        {
            stepIndex++;
            dialogueManager.DisplayNextSentence();
        }

        if (stepIndex == 2) // Wait for enemies to spawn currency
        {
            stepIndex++;
            StartCoroutine("AutoDialogue", 3f);
        }

        if (stepIndex == 3 && multiplayerManager.CanAffordItem(3, 0)) // Wait for the player to pick up at least 3 currency
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
