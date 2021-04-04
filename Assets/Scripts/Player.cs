﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    Rigidbody rb;
    float movementSpeed = 7f;
    float rotationSpeed = 20f;

    bool isSprinting;

    private Vector3 movementInput = Vector3.zero;

    [SerializeField]
    private PlayerInput playerInput = null;

    [SerializeField]
    GameObject interactObject;

    Transform armsObject;

    public GameObject selectedObject;
    [SerializeField] Transform heldObjectPoint;

    public int currencyCount = 0;
    public Dictionary<string, int> inventory = new Dictionary<string, int>()
    {
        {"Bits", 0 },
        {"Circuits", 0 }
    };

    [SerializeField] HUDController hudController;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        armsObject = gameObject.transform.Find("Arms");
        isSprinting = false;
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
        if(interactObject)
        EnableOutlineObject();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
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

    void Sprint()
    {
        if (!isSprinting)
        {
            StartCoroutine(Sprinting());
        }
        else
        {
            StopCoroutine(Sprinting());
            movementSpeed = 7f;
            isSprinting = false;
            Debug.Log("Stop Sprinting");
        } 
    }

    IEnumerator Sprinting()
    {
        movementSpeed = 12f;
        isSprinting = true;
        Debug.Log("Sprinting");
        yield return new WaitForSeconds(5f);
        movementSpeed = 7f;
        isSprinting = false;
        Debug.Log("Stop Sprinting");
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
                    if(selectedObject != null && selectedObject.tag == "Turret")
                        PlaceSelectedObject();
                    break;
                case "ObjectPlate":
                    if ((selectedObject != null && selectedObject.tag == "Object") || (selectedObject != null && selectedObject.tag == "Turret"))
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

            if (selectedObject.GetComponent<Object>().plate)
                selectedObject.GetComponent<Object>().plate.GetComponent<ObjectPlate>().placedObject = null;
        }
    }

    private void ReloadTurret()
    {
        if (interactObject.tag == "Turret")
        {
            interactObject.GetComponent<Turret>().ReloadAmmo(selectedObject.GetComponent<Ammo>().reloadObject());
            selectedObject.GetComponent<Ammo>().deleteObject();
            selectedObject = null;
        }
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
