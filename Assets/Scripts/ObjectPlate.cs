using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlate : MonoBehaviour
{
    public GameObject placedObject;
    
    private BoxCollider[] colls;
    private void Awake()
    {
        colls = GetComponents<BoxCollider>();
    }

    private void Update()
    {
        foreach (BoxCollider coll in colls)
        {
            coll.enabled = placedObject == null;
        }
    }
}
