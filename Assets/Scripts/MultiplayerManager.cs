using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiplayerManager : MonoBehaviour
{
    PlayerInputManager inputManager;
    MultipleTargetCamera multiCam;

    public HUDController hudController;

    public static int playerCount = 0;
    private bool gameStarted = false;

    public Dictionary<string, int> inventory = new Dictionary<string, int>()
    {
        // Platform dependent compilation stuff
        // Basically when its played in the Unity Editor, bits & circuits = 100 for debug purposes
        // After the game is built, bits & circuits = 0 so players don't start with any currency
        #if UNITY_EDITOR
            {"Bits", 100 },
            {"Circuits", 100 }
        #else
            {"Bits", 0 },
            {"Circuits", 0 }
        #endif
    };

    private void Awake()
    {
        inputManager = GetComponent<PlayerInputManager>();
        multiCam = FindObjectOfType<MultipleTargetCamera>();
        hudController = FindObjectOfType<HUDController>();
    }

    private void Start()
    {   
        inputManager.DisableJoining();

        if (FindObjectOfType<IntroSequence>() == null) {
            IntroSequenceFinished();
        }
    }

    public void IntroSequenceFinished() {
        //Spawn one player at the start of the game
        inputManager.EnableJoining();
        inputManager.JoinPlayer();
        MultiplayerManager.playerCount = 1;
        gameStarted = true;
        hudController.UpdateCash(inventory);
    }

    public void OnPlayerJoined() {
        if (multiCam) {
            multiCam.NewPlayerSpawned();
        }

        if (gameStarted)
            MultiplayerManager.playerCount = inputManager.playerCount;
    }

    public void ReceiveCurrency(int bits, int circuits) {
        inventory["Bits"] += bits;
        inventory["Circuits"] += circuits;

        hudController.UpdateCash(inventory);
    }

    public void SpendCurrency(int bits, int circuits) {
        if (CanAffordItem(bits, circuits)) {
            inventory["Bits"] -= bits;
            inventory["Circuits"] -= circuits;
        }

        hudController.UpdateCash(inventory);
    }

    public bool CanAffordItem(int bits, int circuits) {
        return inventory["Bits"] >= bits && inventory["Circuits"] >= circuits;
    }
}
