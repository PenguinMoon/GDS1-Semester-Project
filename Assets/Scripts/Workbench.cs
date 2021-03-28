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

    public void OnTurretMakeButtonPressed()
    {
        if (playerRef && turretPrefab)
        {
            if (playerRef.currencyCount >= turretCost)
            {
                playerRef.ReceiveTurret(turretPrefab, turretCost);
            }
        }
    }
}
