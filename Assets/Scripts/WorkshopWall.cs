using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkshopWall : MonoBehaviour
{
    Workshop parent;

    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.GetComponent<Workshop>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
            parent.TakeDamage(1);
    }
}
