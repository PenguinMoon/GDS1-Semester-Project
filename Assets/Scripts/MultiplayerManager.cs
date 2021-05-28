using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiplayerManager : MonoBehaviour
{
    PlayerInputManager inputManager;
    MultipleTargetCamera multiCam;

    private void Awake()
    {
        inputManager = GetComponent<PlayerInputManager>();
        multiCam = FindObjectOfType<MultipleTargetCamera>();
    }

    private void Start()
    {
        //Spawn one player at the start of the game
        inputManager.JoinPlayer();
    }

    public void OnPlayerJoined() {
        if (multiCam) {
            multiCam.NewPlayerSpawned();
        }
    }
}
