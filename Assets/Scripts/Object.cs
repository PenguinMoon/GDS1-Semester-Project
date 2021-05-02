using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Object : MonoBehaviour
{

    public GameObject plate = null;
    public bool isBeingHeld = false;


    // Object Properties
    [SerializeField] public string objectName;
    [SerializeField] public Texture objectImage;
    [SerializeField] public int bitsPrice;
    [SerializeField] public int circuitsPrice;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
