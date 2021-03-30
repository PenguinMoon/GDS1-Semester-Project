using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Workbench : MonoBehaviour
{
    [SerializeField] GameObject workbenchUI;
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
        workbenchUI.SetActive(true);
    }

    public void StopInteract()
    {
        workbenchUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Player playerRef = other.gameObject.GetComponent<Player>();
        if (playerRef)
            Interact(playerRef);
    }

    private void OnTriggerExit(Collider other)
    {
        StopInteract();
    }

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
            if (canAffordItem(item.name))
            {
                playerRef.ReceiveTurret(item);

                takeCurrencyFromPlayer(item.name);
            }
        }
    }
}
