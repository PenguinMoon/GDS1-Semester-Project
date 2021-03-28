using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    float movementSpeed = 5f;

    float rotationSpeed = 20f;

    [SerializeField]
    GameObject interactObject;

    Transform armsObject;

    public GameObject selectedTurret;
    [SerializeField] Transform heldObjectPoint;

    public int currencyCount = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        armsObject = gameObject.transform.Find("Arms");
    }

    void Update()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        // Check if there is movement input before rotating
        if (input != Vector3.zero)
        {
            Quaternion lookDirection = Quaternion.LookRotation(input);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookDirection, rotationSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.E))
            Interact();


        if (selectedTurret == null)
        {
            armsObject.localPosition = new Vector3(0.5f, 0, 0.125f);
        }
        else
        {
            armsObject.localPosition = new Vector3(0.5f, 0.9f, 0.125f);
        }

        rb.velocity = input * movementSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Turret":
            case "TurretPlate":
                interactObject = other.gameObject;
                break;
            case "Workbench":
                interactObject = other.gameObject;
                InteractWithWorkbench();
                break;
            case "Currency":
                PickupCurrency(other.gameObject);
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (interactObject && interactObject.tag == "Workbench")
            interactObject.GetComponent<Workbench>().StopInteract();

        interactObject = null;
    }

    private void Interact()
    {
        if (interactObject)
            switch (interactObject.tag)
            {
                case "TurretPlate":
                    PlaceSelectedTurret();
                    break;
                case "Turret":
                    PickupTurret();
                    break;
                case "Workbench":
                    InteractWithWorkbench();
                    break;

            }
    }

    private void InteractWithWorkbench()
    {
        interactObject.GetComponent<Workbench>().Interact(this);
    }

    private void PickupCurrency(GameObject coin)
    {
        currencyCount++;

        Destroy(coin);
    }

    private void PlaceSelectedTurret()
    {
        Debug.Log(selectedTurret);

        if (selectedTurret)
        {
            selectedTurret.transform.position = interactObject.transform.position;
            selectedTurret.transform.rotation = interactObject.transform.rotation;
            selectedTurret.transform.SetParent(interactObject.transform);



            interactObject.GetComponent<TurretPlate>().placedTurret = selectedTurret;
            selectedTurret.GetComponent<Turret>().plate = interactObject;

            selectedTurret = null;
        }
    }

    private void PickupTurret()
    {
        if (!selectedTurret)
        {
            interactObject.transform.position = heldObjectPoint.position;
            interactObject.transform.SetParent(heldObjectPoint);
            selectedTurret = interactObject;

            if (selectedTurret.GetComponent<Turret>().plate)
                selectedTurret.GetComponent<Turret>().plate.GetComponent<TurretPlate>().placedTurret = null;
        }
    }

    public void ReceiveTurret(GameObject turret, int cost)
    {
        if (!selectedTurret)
        {
            selectedTurret = Instantiate(turret, heldObjectPoint.position, heldObjectPoint.rotation);
            selectedTurret.transform.SetParent(heldObjectPoint);

            currencyCount -= cost;
        }
    }
}
