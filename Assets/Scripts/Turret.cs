using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{

    [SerializeField]int maxAmmo = 20;
    int currentAmmo;

    [SerializeField]GameObject gunObject;

    private void Update()
    {
    }
}
