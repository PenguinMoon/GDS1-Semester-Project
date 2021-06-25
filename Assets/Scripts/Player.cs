using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    float startingSpeed = 7f;
    float movementSpeed = 7f;
    float rotationSpeed = 20f;

    public bool canInput; // Check if the player is accessing a menu and disable movement input

    bool isSprinting;   //Checks if the player is already sprinting
    float sprintTime, maxSprintTime;    // The amount of stamina the player has and the maximum amount of stamina
    Coroutine sprintRoutine;    // The current coroutine for sprinting that is active

    private Vector3 movementInput = Vector3.zero;

    [SerializeField] private PlayerInput playerInput = null;

    [SerializeField] GameObject interactObject;

    //Transform armsObject;

    [SerializeField] ParticleSystem sprintParticle;
    [SerializeField] ParticleSystem craftParticle;
    [SerializeField] ParticleSystem pickupParticle;

    Animator playerAnim;
    float currentWhackDelay = 0f;
    [SerializeField] float startingWhackDelay = 0.5f;


    public GameObject selectedObject;
    [SerializeField] Transform heldObjectPoint;

    public int currencyCount = 0;
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

    [SerializeField] HUDController hudController;
    [SerializeField] PlayerUI playerUI;
    Workbench workBench;

    // Action references for input UI
    [SerializeField] private InputActionReference interactAction = null;
    [SerializeField] private InputActionReference whackAction = null;

    bool hintTextVisible;

    private bool isInteractKeyHeld = false;
    private float maxRepairDelay = 0.1f;
    private float repairDelay = 0f;

    [Header("Audio Stuff")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] SFXData sfxData;
    float stepRate = 0.4f;  // Time between each footstep sound
    float stepDelay;    // Current counter of the time between each footstep

    [Space(10)]
    [SerializeField] CinemachineVirtualCamera vcam; // Player's follow cam
    bool isZoomOut = false;
    [SerializeField] IntroSequence introController;

    [SerializeField] Animator animator;

    [SerializeField] Material secondPlayerMaterial;
    [SerializeField] SkinnedMeshRenderer meshRenderer;
    [SerializeField] InputActionAsset defaultPlayerInput;

    //Manages inventory now that local multiplayer is a thing
    MultiplayerManager manager;

    private void Awake()
    {
        manager = FindObjectOfType<MultiplayerManager>();

        rb = GetComponent<Rigidbody>();
        //armsObject = gameObject.transform.Find("Arms");
        playerAnim = gameObject.GetComponentInChildren<Animator>();

        canInput = true;
        isSprinting = false;
        sprintTime = 5f;
        maxSprintTime = sprintTime;
        playerUI.SetSprintDuration(sprintTime);
        sprintRoutine = null;

        if (InputSystem.GetDevice<VirtualKeyboardDevice>() == null)
        {
            InputSystem.AddDevice<VirtualKeyboardDevice>();
        }

        var dvc = InputSystem.GetDevice<VirtualKeyboardDevice>();
        InputSystem.EnableDevice(dvc);

        //GetComponent<PlayerInput>().actions = defaultPlayerInput;

        stepDelay = 0;
    }

    private void Start()
    {
        if (MultiplayerManager.playerCount > 1) {
            Material[] mats = meshRenderer.materials;
            mats[0] = secondPlayerMaterial;
            meshRenderer.materials = mats;
        }

        Vector3 spawnPosition = Vector3.zero;

        if (GameObject.FindGameObjectWithTag("Respawn") != null)
            spawnPosition = GameObject.FindGameObjectWithTag("Respawn").transform.position;

        transform.position = spawnPosition;
    }

    void Update()
    {
        // if (hudController)
        //     hudController.UpdateCash(inventory);

        //OLD INPUT//
        //Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        //input = Vector3.ClampMagnitude(input, 1f);

        // Check if player is pressing input button
        //if (Input.GetKeyDown(KeyCode.E))
        //    Interact();

        // Animate arms
        //if (selectedObject == null)
        //{
        //    armsObject.localPosition = new Vector3(0.5f, 0, 0.125f);
        //}
        //else
        //{
        //    armsObject.localPosition = new Vector3(0.5f, -0.1f, 0.875f);
        //}

        // Highlight selected object
        if (interactObject)
            EnableOutlineObject();


        // Reduce hit time so player can whack again
        if (currentWhackDelay > 0)
            currentWhackDelay -= Time.deltaTime;

        //Check should dump bits into objective
        if (isInteractKeyHeld && interactObject && interactObject.tag == "KioskObjective")
            InteractWithObjective();

        //Decrement repair delay if necessary
        if (repairDelay > 0)
            repairDelay -= Time.deltaTime;

        stepDelay -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        // Check if there is movement input before rotating
        if (movementInput != Vector3.zero)
        {
            if (stepDelay <= 0f)
            {
                audioSource.PlayOneShot(sfxData.Footstep);
                stepDelay = stepRate;
            }
            Quaternion lookDirection = Quaternion.LookRotation(movementInput);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookDirection, rotationSpeed * Time.deltaTime);
        }

        Vector3 vel = movementInput * movementSpeed;

        animator.SetFloat("WalkSpeed", movementInput.magnitude * movementSpeed);

        // Player floating fix - Reactivate if needed
        //vel += Physics.gravity;

        rb.velocity = vel;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.performed || introController)
        {
            return;
        }
        // Handle player movement
        if (canInput)
            movementInput = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);

        // Handle UI input
        if (!canInput)
        {
            // Reset player movementInput vector so that player doesn't move when exiting menu
            movementInput = new Vector3(0, 0);
            if (!context.performed)
            {
                return;
            }
            if (context.ReadValue<Vector2>().x < 0)
            {
                //workBench.MoveMenuUp();
                //workBench.SelectButton();
            }
            else if (context.ReadValue<Vector2>().x > 0)
            {
                //workBench.MoveMenuDown();
                //workBench.SelectButton();
            }
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed || introController)
        {
            isInteractKeyHeld = false;
            return;
        }

        isInteractKeyHeld = true;
        Interact();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (!context.performed || !canInput || introController)
        {
            return;
        }

        // Context started and cancelled changes sprint to hold
        // To change back, change the Sprint input action from pass through to button
        if (context.started)
        {
            isSprinting = true;
            //Debug.Log("Start sprint!");
        }
        if (context.canceled)
        {
            isSprinting = false;
            //Debug.Log("Stop sprint!");
        }
        Sprint();
    }

    public void OnWhack(InputAction.CallbackContext context)
    {
        if (!context.performed || selectedObject != null || currentWhackDelay > 0f || introController)
        {
            return;
        }

        if (canInput)
        {
            Whack();
            currentWhackDelay = startingWhackDelay;
        }

        if (!canInput)
        {
            //workBench.SubmitButton();
        }


    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        if (introController)
        {
            introController.SkipCutscene();
            return;
        }

        manager.hudController.PauseGame();
    }

    public void OnFastForward(InputAction.CallbackContext context)
    {
        if (!context.performed || introController)
        {
            return;
        }
        manager.hudController.FastForward();
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        if (!context.performed || introController)
        {
            return;
        }
        Zoom();
    }

    // Activates sprinting only if the player has enough stamina remaining
    void Sprint()
    {
        if (sprintTime > 0f)
        {
            // If the player is not already sprinting, set them to start sprinting
            // If the player is already sprinting, stop the sprint
            if (!isSprinting)
            {
                if (sprintRoutine != null)
                    StopCoroutine(sprintRoutine);
                sprintRoutine = StartCoroutine(Sprinting());
            }
            else
            {
                if (sprintRoutine != null)
                    StopCoroutine(sprintRoutine);
                playerUI.isDraining = false;
                sprintRoutine = StartCoroutine(RecoverSprint());
                movementSpeed = 7f;
                stepRate = 0.4f;
                isSprinting = false;
                sprintParticle.Stop();
                //Debug.Log("Stop Sprinting");
            }
        }
    }

    // Increase movement speed according to the amount of sprint time/stamina remaining
    // After all sprint time/stamina is gone, reset to default movement speed
    IEnumerator Sprinting()
    {
        movementSpeed = 12f;
        isSprinting = true;
        sprintParticle.Play();
        //Debug.Log("Sprinting");
        playerUI.isDraining = true;
        stepRate = 0.2f;
        while (sprintTime > 0f)
        {
            sprintTime -= Time.deltaTime * maxSprintTime;
            if (sprintTime < 0f)
                sprintTime = 0f;
            yield return new WaitForSeconds(Time.deltaTime * maxSprintTime);
        }
        movementSpeed = 7f;
        isSprinting = false;
        sprintParticle.Stop();
        sprintRoutine = StartCoroutine(RecoverSprint());
        //Debug.Log("Stop Sprinting");
        stepRate = 0.4f;
    }

    // Recovers sprint time/stamina over time after a 1 second delay
    IEnumerator RecoverSprint()
    {
        yield return new WaitForSeconds(1f);
        playerUI.isDraining = false;
        playerUI.isRecovering = true;
        while (sprintTime < maxSprintTime)
        {
            sprintTime += Time.deltaTime * maxSprintTime;
            if (sprintTime > maxSprintTime)
                sprintTime = maxSprintTime;
            yield return new WaitForSeconds(Time.deltaTime * maxSprintTime);
        }
        //Debug.Log("Sprint Full");
    }

    private void Whack()
    {
        playerAnim.SetTrigger("PlayWhack");

        if (interactObject)
            switch (interactObject.tag)
            {
                case "Turret":
                    //WhackTurret();
                    break;
            }
    }

    private void Zoom()
    {
        //if (isZoomOut)
        //{
        //    ((CinemachineVirtualCamera)vcam).GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = 15f;
        //    isZoomOut = false;
        //}
        //else
        //{
        //    ((CinemachineVirtualCamera)vcam).GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = 32f;
        //    isZoomOut = true;
        //}

        manager.ZoomCamera();
    }

    private void PickupCurrency(GameObject coin)
    {
        if (coin.name.Contains("Circuit"))
            manager.ReceiveCurrency(0, 1);//inventory["Circuits"]++;
        else if (coin.name.Contains("Bit"))
            manager.ReceiveCurrency(1, 0);//inventory["Bits"]++;

        pickupParticle.Play();
        audioSource.PlayOneShot(sfxData.CurrencyPickup);
        Destroy(coin);
    }


    private void OnTriggerStay(Collider other)
    {
        DisableOutlineObject();

        if(!hintTextVisible)
        ShowHintText();

        switch (other.gameObject.tag)
        {
            case "Turret":
                if (!other.gameObject.GetComponent<Object>().isBeingHeld)
                    interactObject = other.gameObject;
                break;
            case "Object":
                interactObject = other.gameObject;
                break;
            case "TurretPlate":
                interactObject = other.gameObject;
                break;
            case "ObjectPlate":
                interactObject = other.gameObject;
                break;
            case "Workbench":
                interactObject = other.gameObject;
                workBench = interactObject.GetComponent<Workbench>();
                if (interactObject.tag == "Workbench")
                {
                    workBench.Interact(this);
                }
                break;
            case "Currency":
                PickupCurrency(other.gameObject);
                break;
            case "KioskObjective":
                interactObject = other.gameObject;
                break;
            case "Bin":
                interactObject = other.gameObject;
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        DisableOutlineObject();
        HideHintText();
        interactObject = null;
    }

    private void Interact()
    {
        if (interactObject)
            switch (interactObject.tag)
            {
                case "TurretPlate":
                    if (selectedObject != null && selectedObject.tag == "Turret")
                        PlaceSelectedObject();
                    break;
                case "ObjectPlate":
                    if ((selectedObject != null && selectedObject.tag == "Object") || (selectedObject != null && selectedObject.tag == "Turret")) // Reload turret if player is holding ammo
                        PlaceSelectedObject();
                    break;
                case "Turret":
                    if (selectedObject == null || (selectedObject != null && selectedObject.tag == "Turret"))
                        PickupObject();
                    else if (selectedObject != null && selectedObject.GetComponent<Ammo>() != null)
                    {
                        ReloadTurret();
                    }
                    break;
                case "Object":
                    PickupObject();
                    break;
                case "Workbench":
                    //WhackWorkbench();
                    //BuyWorkbench();
                    break;
                case "Bin":
                    DisposeSelectedObject();
                    break;
            }
    }

    private void InteractWithObjective()
    {
        if (manager.CanAffordItem(1, 0) && repairDelay <= 0)
        {
            manager.SpendCurrency(1, 0);//inventory["Bits"]--;
            interactObject.GetComponent<KioskObjective>().ReceiveBits(1);
            repairDelay = maxRepairDelay;
        }
    }

    private void PlaceSelectedObject()
    {
        Debug.Log("Trying to place object...");

        if (selectedObject)
        {
            audioSource.PlayOneShot(sfxData.TowerPlace);
            selectedObject.transform.position = interactObject.transform.position;
            //selectedObject.transform.rotation = interactObject.transform.rotation;
            selectedObject.transform.SetParent(interactObject.transform);
            selectedObject.transform.LookAt(interactObject.transform); // Face the object towards the plate

            interactObject.GetComponent<ObjectPlate>().placedObject = selectedObject;
            selectedObject.GetComponent<Object>().plate = interactObject;
            selectedObject.GetComponent<Object>().isBeingHeld = false;

            //hudController.UpdateItemSlot(null);
            selectedObject = null;
        }
    }

    private void PickupObject()
    {
        if (selectedObject && selectedObject.tag == "Turret") 
        {
            SwapObjects();
        }

        else if (!selectedObject)
        {
            audioSource.PlayOneShot(sfxData.TowerPickup);
            interactObject.transform.position = heldObjectPoint.position;
            interactObject.transform.rotation = heldObjectPoint.transform.rotation;
            interactObject.transform.SetParent(heldObjectPoint);

            selectedObject = interactObject;
            //hudController.UpdateItemSlot(selectedObject);

            if (selectedObject.GetComponent<Object>())
            {
                selectedObject.GetComponent<Object>().isBeingHeld = true;
                selectedObject.GetComponent<Object>().plate.GetComponent<ObjectPlate>().placedObject = null;
            }

        }
    }

    public GameObject swapToObject;
    private void SwapObjects() 
    {
        GameObject currentHeldObject = selectedObject;
        swapToObject = interactObject;
        GameObject turretPlate = interactObject.GetComponent<Object>().plate;

        Debug.Log("Should swap turrets");

        if (swapToObject)
        {
            audioSource.PlayOneShot(sfxData.TowerPickup);
            swapToObject.transform.position = heldObjectPoint.position;
            swapToObject.transform.rotation = heldObjectPoint.transform.rotation;
            swapToObject.transform.SetParent(heldObjectPoint);

            selectedObject = swapToObject;
            //hudController.UpdateItemSlot(selectedObject);

            if (selectedObject.GetComponent<Object>())
            {
                selectedObject.GetComponent<Object>().isBeingHeld = true;
                selectedObject.GetComponent<Object>().plate.GetComponent<ObjectPlate>().placedObject = null;
            }

            audioSource.PlayOneShot(sfxData.TowerPlace);
            currentHeldObject.transform.position = turretPlate.transform.position;
            currentHeldObject.transform.SetParent(turretPlate.transform);
            currentHeldObject.transform.LookAt(turretPlate.transform); // Face the object towards the plate

            turretPlate.GetComponent<ObjectPlate>().placedObject = currentHeldObject;
            currentHeldObject.GetComponent<Object>().plate = turretPlate;
            currentHeldObject.GetComponent<Object>().isBeingHeld = false;


        }


    }

    private void ReloadTurret()
    {
        if (interactObject.tag == "Turret")
        {
            interactObject.GetComponent<SmartTurret>().ReloadAmmo(selectedObject.GetComponent<Ammo>().reloadObject());
            selectedObject.GetComponent<Ammo>().deleteObject();
            selectedObject = null;
            audioSource.PlayOneShot(sfxData.TowerPlace);
            hudController.UpdateItemSlot(selectedObject);

        }
    }

    private void WhackTurret()
    {
        if (interactObject.tag == "Turret")
        {
            audioSource.PlayOneShot(sfxData.Whack);
            interactObject.GetComponent<SmartTurret>().HitByPlayer();
        }
    }

    private void WhackWorkbench()
    {
        if (interactObject.tag == "Workbench")
        {
            workBench.Interact(this);
        }
    }

    private void DisposeSelectedObject()
    {
        if (selectedObject)
        {
            manager.ReceiveCurrency(selectedObject.GetComponent<Object>().bitsPrice, selectedObject.GetComponent<Object>().circuitsPrice);
            //inventory["Bits"] += selectedObject.GetComponent<Object>().bitsPrice;
            //inventory["Circuits"] += selectedObject.GetComponent<Object>().circuitsPrice;
            selectedObject.GetComponent<Object>().DestroyObject();
            hudController.UpdateItemSlot(null);
            selectedObject = null;
        }
    }

    public void EnterMenu()
    {
        movementSpeed = 0f;
        canInput = false;
    }

    public void ExitMenu()
    {
        movementSpeed = startingSpeed;
        canInput = true;
    }

    public void ReceiveTurret(GameObject turret)
    {
        if (!selectedObject)
        {
            selectedObject = Instantiate(turret, heldObjectPoint.position, heldObjectPoint.rotation);
            selectedObject.transform.SetParent(heldObjectPoint);
            selectedObject.GetComponent<Object>().isBeingHeld = true;
            //hudController.UpdateItemSlot(turret);
            audioSource.PlayOneShot(sfxData.TowerPickup);
            craftParticle.Play();
        }
    }

    private void EnableOutlineObject()
    {
        if (interactObject)
        {
            if (interactObject.TryGetComponent(out Outline outline))
                outline.enabled = true;
        }
    }

    private void DisableOutlineObject()
    {
        if (interactObject)
        {
            if (interactObject.TryGetComponent(out Outline outline))
                outline.enabled = false;
        }
    }

    private void ShowHintText()
    {
        if (interactObject)
        {
            if (interactObject.tag == "Workbench" && !selectedObject)
            {
                int bindingIndex = interactAction.action.GetBindingIndexForControl(interactAction.action.controls[0]);
                playerUI.UpdateHintText("[" + InputControlPath.ToHumanReadableString(interactAction.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice) + "] " + "Use Workbench");
                playerUI.ShowHintUIText();
            }
            if (interactObject.tag == "Turret")
            {
                int bindingIndex = interactAction.action.GetBindingIndexForControl(interactAction.action.controls[0]);
                playerUI.UpdateHintText("[" + InputControlPath.ToHumanReadableString(interactAction.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice) + "] " + "Pick up");
                playerUI.ShowHintUIText();
            }
            if (interactObject.tag == "Turret" && selectedObject.tag == "Turret")
            {
                int bindingIndex = interactAction.action.GetBindingIndexForControl(interactAction.action.controls[0]);
                playerUI.UpdateHintText("[" + InputControlPath.ToHumanReadableString(interactAction.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice) + "] " + "Swap");
                playerUI.ShowHintUIText();
            }
            if (interactObject.tag == "Turret" && selectedObject.tag == "Object")
            {
                int bindingIndex = interactAction.action.GetBindingIndexForControl(interactAction.action.controls[0]);
                playerUI.UpdateHintText("[" + InputControlPath.ToHumanReadableString(interactAction.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice) + "] " + "Use");
                playerUI.ShowHintUIText();
            }
            if ((interactObject.tag == "TurretPlate" || interactObject.tag == "ObjectPlate") && selectedObject)
            {
                int bindingIndex = interactAction.action.GetBindingIndexForControl(interactAction.action.controls[0]);
                playerUI.UpdateHintText("[" + InputControlPath.ToHumanReadableString(interactAction.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice) + "] " + "Place");
                playerUI.ShowHintUIText();
            }
            if (interactObject.tag == "KioskObjective")
            {
                int bindingIndex = interactAction.action.GetBindingIndexForControl(interactAction.action.controls[0]);
                playerUI.UpdateHintText("Hold [" + InputControlPath.ToHumanReadableString(interactAction.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice) + "] " + "Repair");
                playerUI.ShowHintUIText();
            }
            if (interactObject.tag == "Bin")
            {
                int bindingIndex = interactAction.action.GetBindingIndexForControl(interactAction.action.controls[0]);
                playerUI.UpdateHintText("[" + InputControlPath.ToHumanReadableString(interactAction.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice) + "] " + "Dispose");
                playerUI.ShowHintUIText();
            }
            hintTextVisible = true;
        }
    }

    private void HideHintText()
    {
        hintTextVisible = false;
        if (interactObject)
        {
            playerUI.HideHintUIText();
        }
        if (interactObject && interactObject.tag == "Workbench")
        {
            workBench.StopInteract();
        }
    }

}
