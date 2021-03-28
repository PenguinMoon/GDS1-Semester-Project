using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{

    [SerializeField]int maxAmmo = 20;
    int currentAmmo;

    [SerializeField] float rotationSpeed = 0.1f;

    [SerializeField]GameObject gunObject;
    int direction = 1;

    private void Update()
    {
        SearchSurrounds();
    }

    private void SearchSurrounds()
    {
        Vector3 rotation = gunObject.transform.rotation.eulerAngles;

        if (Mathf.RoundToInt(rotation.y) == 90 || Mathf.RoundToInt(rotation.y) == 270)
            direction *= -1;

        rotation += Vector3.up * rotationSpeed * direction;

        gunObject.transform.rotation = Quaternion.Euler(rotation);

        //Do a raycast out the front of the turret, using a layer mask with the 'Enemy' layer selected
        //If hit something, then lock on to it and fire at it
        //Or do a boxcast/spherecast for a wider view angle
    }
}
