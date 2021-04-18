using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Workbench : MonoBehaviour
{
    [SerializeField] GameObject workbenchUI;
    [SerializeField] Button defaultSelectedButton;
    [SerializeField] GameObject turretPrefab;
    [SerializeField] int turretCost = 1;

    private Player playerRef;

    private void Awake()
    {
        workbenchUI.SetActive(false);
    }

    public void Interact(Player player)
    {
        playerRef = player;

        defaultSelectedButton.Select();
        playerRef.EnterMenu();
        StartCoroutine("ActivateWorkshop");
    }

    IEnumerator ActivateWorkshop()
    {
        yield return new WaitForSeconds(0.01f);
        workbenchUI.SetActive(true);
    }

    public void StopInteract()
    {
        workbenchUI.SetActive(false);
        playerRef.ExitMenu();
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

    private bool canAffordItem(string itemName)
    {
        switch (itemName)
        {
            case "Ammo":
            case "Turret":
                return playerRef.inventory["Bits"] >= 1;
            case "Better Turret":
                return playerRef.inventory["Bits"] >= 2 && playerRef.inventory["Circuits"] >= 1;
        }

        return false;
    }

    private bool canHoldItem()
    {
        if (playerRef.selectedObject == null)
            return true;
        else
            return false;
    }

    private void takeCurrencyFromPlayer(string itemName)
    {
        switch (itemName)
        {
            case "Ammo":
            case "Turret":
                playerRef.inventory["Bits"]--;
                break;
            case "Better Turret":
                playerRef.inventory["Bits"] -= 2;
                playerRef.inventory["Circuits"]--;
                break;
        }
    }

    public void OnTurretMakeButtonPressed(GameObject item)
    {
        if (playerRef && item)
        {
            if (canAffordItem(item.name) && canHoldItem())
            {
                playerRef.ReceiveTurret(item);

                takeCurrencyFromPlayer(item.name);
            }
        }
    }
}
