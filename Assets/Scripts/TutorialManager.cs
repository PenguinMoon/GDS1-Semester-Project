using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    //Tutorial Objects
    [SerializeField] Player player;
    [SerializeField] SmartTurret turret;
    int defaultTurretCost = 0;
    [SerializeField] TurretPlate turretPlate;
    [SerializeField] TurretPlate turretPlate2;
    [SerializeField] GameObject workbench;
    [SerializeField] GameObject workbench2;

    [SerializeField] GameObject arrow;

    DialogueManager dialogueManager;
    MultiplayerManager multiplayerManager;

    [SerializeField]
    int stepIndex = 0;

    bool isWaiting = false;

    // Start is called before the first frame update
    void Start()
    {
        multiplayerManager = FindObjectOfType<MultiplayerManager>();
        player = FindObjectOfType<Player>();
        turret = FindObjectOfType<SmartTurret>();
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isWaiting || dialogueManager.isTyping)
        {
            return;
        }

        switch (stepIndex)
        {
            case 1:
                PointArrowTo(turretPlate.gameObject);
                break;

            case 3:
                PointArrowTo(workbench.gameObject);
                break;

            case 4:
                PointArrowTo(turretPlate2.gameObject);
                break;

            case 5:
                PointArrowTo(workbench2.gameObject);
                break;

            case 6:
                PointArrowTo(turret.gameObject);
                break;

            case 7:
                PointArrowTo(turretPlate2.gameObject);
                break;

            case 8:
                PointArrowTo(workbench.gameObject);
                break;

            case 9:
                PointArrowTo(turret.gameObject);
                break;

            default:
                if (player)
                    arrow.transform.position = player.transform.position;
                arrow.gameObject.SetActive(false);
                break;
        }

        if (player == null)
        {
            player = FindObjectOfType<Player>();
            turret = FindObjectOfType<SmartTurret>();
        }


        //                    ///
        //   TUTORIAL STEPS   ///
        //                    ///

        if (stepIndex == 0) // Player picks up initial turret
        {
            dialogueManager.DisplaySentenceAtIndex(stepIndex);

            //Showing: Intro Line

            StartCoroutine(IncrementStepAfter(4f));
        }

        //Showing: Place turret down

        else if (stepIndex == 1 && turretPlate.placedObject && turretPlate.placedObject.tag == "Turret") // Player places turret down on correct plate
        {
            ProgressToNextStep();
        }

        //Showing: Pickup required amount of bits

        else if (stepIndex == 2 && multiplayerManager.inventory["Bits"] >= 4) // Wait until player has enough to afford turret
        {
            ProgressToNextStep();
        }

        //Showing: Craft turret from workbench

        else if (stepIndex == 3 && player.selectedObject && player.selectedObject.tag == "Turret") // Wait until the player is now holding a turret (i.e. bought one)
        {
            ProgressToNextStep();
        }

        //Showing: Place new turret down

        else if (stepIndex == 4 && turretPlate2.placedObject && turretPlate2.placedObject.tag == "Turret") // Wait until the player has placed the turret down on the second plate
        {
            ProgressToNextStep();
        }

        //Showing: Craft battery from workbench

        else if (stepIndex == 5 && player.selectedObject && player.selectedObject.tag == "Object")
        {
            ProgressToNextStep();
        }

        //Showing: Place battery into one of the turrets

        else if (stepIndex == 6 && !player.selectedObject)
        {
            ProgressToNextStep();
        }

        //Showing: Get player to whack turret

        else if (stepIndex == 7)
        {
            StartCoroutine(IncrementStepAfter(5f));
        }

        //Showing: Upgrade turret

        else if (stepIndex == 8 && player.selectedObject && player.selectedObject.tag == "Turret")
        {
            ProgressToNextStep();
        }

        //Showing: Place turret onto another turret

        else if (stepIndex == 9 && !player.selectedObject)
        {
            ProgressToNextStep();
        }

        //Showing: Info about upgrades

        else if (stepIndex == 10)
        {
            StartCoroutine(IncrementStepAfter(5f));
        }

        //Showing: Congrats

        else if (stepIndex == 11)
        {
            StartCoroutine(IncrementStepAfter(4f));
        }
    }

    void ProgressToNextStep()
    {
        stepIndex++;
        dialogueManager.DisplaySentenceAtIndex(stepIndex);
    }

    IEnumerator IncrementStepAfter(float seconds)
    {
        isWaiting = true;

        yield return new WaitWhile(() => dialogueManager.isTyping);
        yield return new WaitForSeconds(seconds);
        stepIndex++;

        dialogueManager.DisplaySentenceAtIndex(stepIndex);

        isWaiting = false;
    }

    void PointArrowTo(GameObject target)
    {
        arrow.gameObject.SetActive(true);
        //arrow.transform.position = target.transform.position + Vector3.up;
        LeanTween.move(arrow, target.transform.position, 1f);
    }
}
