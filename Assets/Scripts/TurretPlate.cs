using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretPlate : MonoBehaviour
{
    public GameObject placedTurret;

    private BoxCollider[] colls;

    private void Awake()
    {
        colls = GetComponents<BoxCollider>();
    }

    private void Update()
    {
        foreach (BoxCollider coll in colls)
        {
            coll.enabled = placedTurret == null;
        }
    }
}
