using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Workbench : MonoBehaviour
{
    [SerializeField] GameObject workbench;
    [SerializeField] ListPositionCtrl workbenchMenu;
    [SerializeField] Button defaultSelectedButton;
    [SerializeField] GameObject turretPrefab;
    [SerializeField] int turretCost = 1;
    public AnimationCurve curve;

    private Player playerRef;

    private void Awake()
    {
        StartCoroutine("ResetWorkbenchUI");
        workbench.GetComponent<Canvas>().transform.localScale = new Vector3(0,0,1);
        workbench.GetComponent<Canvas>().enabled = false;
    }

    IEnumerator ResetWorkbenchUI()
    {
        for (int i = 0; i < workbenchMenu.listBank.GetListLength() - 1; i++)
        {
            yield return new WaitForSeconds(0.01f);
            MoveMenuUp();
        }
        workbench.GetComponent<Canvas>().enabled = true;
        workbench.SetActive(false);
    }

    public void Interact(Player player)
    {
        if (!workbench.activeSelf)
        {
            playerRef = player;
            StartCoroutine("ActivateWorkbench");
        }
    }

    IEnumerator ActivateWorkbench()
    {
        yield return new WaitForSeconds(0.01f);
        playerRef.EnterMenu();
        StartCoroutine("StartSelectButton", 0f);
        workbench.SetActive(true);
        workbench.GetComponent<Canvas>().enabled = true;
        workbench.GetComponent<Canvas>().transform.LeanScale(new Vector3(0.06f, 0.06f, 1), 0.5f).setEase(curve);
    }

    public void StopInteract()
    {
            workbench.GetComponent<Canvas>().transform.LeanScale(new Vector3(0.0f, 0.0f, 1), 0.3f).setEaseOutExpo().setOnComplete(DeactivateWorkbench);
            playerRef.ExitMenu();
    }

    public void DeactivateWorkbench()
    {
        workbench.GetComponent<Canvas>().enabled = false;
        if (!LeanTween.isTweening(workbench.GetComponent<Canvas>().transform.gameObject))
        {
            workbench.SetActive(false);
        }
    }

    public void DenyAnimation()
    {
        if (!LeanTween.isTweening(workbench.GetComponent<Canvas>().transform.gameObject))
        workbench.GetComponent<Canvas>().transform.LeanMoveLocalX(0.5f, 0.1f).setLoopPingPong(2);
    }

    public void SelectButton()
    {
        StartCoroutine("StartSelectButton", 0.15f);
    }

    IEnumerator StartSelectButton(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        defaultSelectedButton = workbenchMenu.listBoxes[workbenchMenu.GetCenteredContentID()].GetComponent<Button>();
        defaultSelectedButton.Select();
    }

    public void SubmitButton()
    {
        Debug.Log(workbenchMenu.GetCenteredContentID());
        defaultSelectedButton = workbenchMenu.listBoxes[workbenchMenu.GetCenteredContentID()].GetComponent<Button>();
        defaultSelectedButton.Select();
    }

    public void MoveMenuUp()
    {
        workbenchMenu.MoveOneUnitUp();
    }

    public void MoveMenuDown()
    {
        workbenchMenu.MoveOneUnitDown();
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //Player playerRef = other.gameObject.GetComponent<Player>();
    //   if (playerRef)
    //Interact(playerRef);
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    StopInteract();
    //}

    private bool canAffordItem(Object item)
    {

        if (item.bitsPrice <= playerRef.inventory["Bits"] && item.circuitsPrice <= playerRef.inventory["Circuits"])
            return true;
        else
            return false;

        // Old shop system
        /*        switch (itemName)
                {
                    case "Ammo":
                    case "Turret_Revised":
                        return playerRef.inventory["Bits"] >= 1;
                    case "MachineTurret_Revised":
                        return playerRef.inventory["Bits"] >= 2 && playerRef.inventory["Circuits"] >= 1;
                    case "CannonTurret":
                        return playerRef.inventory["Bits"] >= 3 && playerRef.inventory["Circuits"] >= 2;
                    case "Flamethrower":
                        return playerRef.inventory["Bits"] >= 1 && playerRef.inventory["Circuits"] >= 2;
                }*/
    }

    private bool canHoldItem()
    {
        if (playerRef.selectedObject == null)
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    private void takeCurrencyFromPlayer(Object item)
    {

        playerRef.inventory["Bits"] -= item.bitsPrice;
        playerRef.inventory["Circuits"] -= item.circuitsPrice;
        
        // Old shop system
        /*        switch (itemName)
                {
                    case "Ammo":
                    case "Turret_Revised":
                        playerRef.inventory["Bits"]--;
                        break;
                    case "MachineTurret_Revised":
                        playerRef.inventory["Bits"] -= 2;
                        playerRef.inventory["Circuits"]--;
                        break;
                    case "CannonTurret":
                        playerRef.inventory["Bits"] -= 3;
                        playerRef.inventory["Circuits"] -= 2;
                        break;
                    case "Flamethrower":
                        playerRef.inventory["Bits"] -= 1;
                        playerRef.inventory["Circuits"] -=2;
                        break;
                }*/
    }

    public void OnTurretMakeButtonPressed(Object item)
    {
        if (playerRef && item)
        {
            if (canAffordItem(item) && canHoldItem())
            {
                playerRef.ReceiveTurret(item.gameObject);
                takeCurrencyFromPlayer(item);
                StopInteract();
            }
            else
            {
                DenyAnimation();
                Debug.Log("PLAYER CANNOT AFFORD ITEM");
            }
        }
    }
}
