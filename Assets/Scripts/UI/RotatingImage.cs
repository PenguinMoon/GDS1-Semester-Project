using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingImage : MonoBehaviour
{
    [SerializeField] float rotSpeed = 100f;
    [SerializeField] Vector3 dir = Vector3.back;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(dir * Time.unscaledDeltaTime * rotSpeed);
    }
}
