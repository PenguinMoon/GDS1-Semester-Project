using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    Rigidbody rb;
    float startingSpeed = 7f;
    float movementSpeed = 7f;
    float rotationSpeed = 20f;

    bool canInput; // Check if the player is accessing a menu and disable movement input

    bool isSprinting;   //Checks if the player is already sprinting
    float sprintTime, maxSprintTime;    // The amount of stamina the player has and the maximum amount of stamina
    Coroutine sprintRoutine;    // The current coroutine for sprinting that is active

    private Vector3 movementInput = Vector3.zero;

    [SerializeField] private PlayerInput playerInput = null;

    [SerializeField] GameObject interactObject;

    Transform armsObject;

    [SerializeField] ParticleSystem sprintParticle;

    Animator playerAnim;
    float currentWhackDelay = 0f;
    [SerializeField] float startingWhackDelay = 0.5f;


    public GameObject selectedObject;
    [SerializeField] Transform heldObjectPoint;

    public int currencyCount = 0;
    public Dictionary<string, int> inventory = new Dictionary<string, int>()
    {
        {"Bits", 0 },
        {"Circuits", 0 }
    };

    [SerializeField] HUDController hudController;
    [SerializeField] PlayerUI playerUI;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        armsObject = gameObject.transform.Find("Arms");
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
    }

    void Update()
    {
        hudController.UpdateCash(inventory);

        //OLD INPUT//
        //Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        //input = Vector3.ClampMagnitude(input, 1f);

        // Check if player is pressing input button
        //if (Input.GetKeyDown(KeyCode.E))
        //    Interact();

        // Check if there is movement input before rotating
        if (movementInput != Vector3.zero)
        {
            Quaternion lookDirection = Quaternion.LookRotation(movementInput);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookDirection, rotationSpeed * Time.deltaTime);
        }


        // Animate arms
        if (selectedObject == null)
        {
            armsObject.localPosition = new Vector3(0.5f, 0, 0.125f);
        }
        else
        {
            armsObject.localPosition = new Vector3(0.5f, -0.1f, 0.875f);
        }

        rb.velocity = movementInput * movementSpeed;

        // Highlight selected object
        if (interactObject)
            EnableOutlineObject();


        // Reduce hit time so player can whack again
        if (currentWhackDelay > 0)
            currentWhackDelay -= Time.deltaTime;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (canInput)
            movementInput = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        Interact();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        Sprint();
    }

    public void OnWhack(InputAction.CallbackContext context)
    {
        if (!context.performed || selectedObject != null || currentWhackDelay > 0f || !canInput)
        {
            return;
        }
        Whack();
        currentWhackDelay = startingWhackDelay;
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        hudController.PauseGame();
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
                isSprinting = false;
                sprintParticle.Stop();
                Debug.Log("Stop Sprinting");
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
        Debug.Log("Sprinting");
        playerUI.isDraining = true;
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
        Debug.Log("Stop Sprinting");
    }

    // Recovers sprint time/stamina over time after a 2 second delay
    IEnumerator RecoverSprint()
    {
        yield return new WaitForSeconds(2f);
        playerUI.isDraining = false;
        playerUI.isRecovering = true;
        while (sprintTime < maxSprintTime)
        {
            sprintTime += Time.deltaTime * maxSprintTime;
            if (sprintTime > maxSprintTime)
                sprintTime = maxSprintTime;
            yield return new WaitForSeconds(Time.deltaTime * maxSprintTime);
        }
        Debug.Log("Sprint Full");
    }

    private void Whack()
    {
        playerAnim.SetTrigger("PlayWhack");

        if (interactObject)
            switch (interactObject.tag)
            {
                case "Turret":
                    WhackTurret();
                    break;
                case "Workbench":
                    WhackWorkbench();
                    break;
            }
    }

    private void PickupCurrency(GameObject coin)
    {
        if (coin.name.Contains("Circuit"))
            inventory["Circuits"]++;
        else if (coin.name.Contains("Bit"))
            inventory["Bits"]++;

        Destroy(coin);
    }


    private void OnTriggerStay(Collider other)
    {
        DisableOutlineObject();
        switch (other.gameObject.tag)
        {
            case "Turret":
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
                break;
            case "Currency":
                PickupCurrency(other.gameObject);
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        DisableOutlineObject();
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
                    if (selectedObject == null)
                        PickupObject();
                    else if (selectedObject != null && selectedObject.GetComponent<Ammo>() != null)
                    {
                        ReloadTurret();
                    }
                    break;
                case "Object":
                    PickupObject();
                    break;
            }
    }
    private void PlaceSelectedObject()
    {
        Debug.Log(selectedObject);

        if (selectedObject)
        {
            selectedObject.transform.position = interactObject.transform.position;
            selectedObject.transform.rotation = interactObject.transform.rotation;
            selectedObject.transform.SetParent(interactObject.transform);

            interactObject.GetComponent<ObjectPlate>().placedObject = selectedObject;
            selectedObject.GetComponent<Object>().plate = interactObject;
            selectedObject.GetComponent<Object>().isBeingHeld = false;

            selectedObject = null;
        }
    }

    private void PickupObject()
    {
        if (!selectedObject)
        {
            interactObject.transform.position = heldObjectPoint.position;
            interactObject.transform.rotation = heldObjectPoint.transform.rotation;
            interactObject.transform.SetParent(heldObjectPoint);

            selectedObject = interactObject;

            if (selectedObject.GetComponent<Object>())
            {
                selectedObject.GetComponent<Object>().isBeingHeld = true;
                selectedObject.GetComponent<Object>().plate.GetComponent<ObjectPlate>().placedObject = null;
            }

        }
    }

    private void ReloadTurret()
    {
        if (interactObject.tag == "Turret")
        {
            interactObject.GetComponent<SmartTurret>().ReloadAmmo(selectedObject.GetComponent<Ammo>().reloadObject());
            selectedObject.GetComponent<Ammo>().deleteObject();
            selectedObject = null;
        }
    }

    private void WhackTurret()
    {
        if (interactObject.tag == "Turret")
        {
            interactObject.GetComponent<SmartTurret>().HitByPlayer();
        }
    }

    private void WhackWorkbench()
    {
        if (interactObject.tag == "Workbench")
        {
            interactObject.GetComponent<Workbench>().Interact(this);
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
}
