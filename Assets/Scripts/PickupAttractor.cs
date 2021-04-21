using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAttractor : MonoBehaviour
{
    bool isMoving = false;
    GameObject target;

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
            transform.parent.position = Vector3.MoveTowards(transform.parent.position, target.transform.position, 10f * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            target = other.gameObject;
            isMoving = true;
        }
    }
}
