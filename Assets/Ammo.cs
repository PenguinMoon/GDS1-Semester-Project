using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{

    public int ammoValue = 20;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int reloadObject()
    {
        return ammoValue;
    }

    public void deleteObject()
    {
        Destroy(gameObject);
    }

}
