using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    float movementSpeed = 5f;

    [SerializeField]
    GameObject interactObject;

    public GameObject selectedTurret;
    [SerializeField] Transform heldObjectPoint;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.E))
            Interact();


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
        }
    }

    private void OnTriggerExit(Collider other)
    {
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
            }
    }

    private void PlaceSelectedTurret()
    {
        Debug.Log(selectedTurret);

        if (selectedTurret)
        {
            selectedTurret.transform.position = interactObject.transform.position;
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
}
