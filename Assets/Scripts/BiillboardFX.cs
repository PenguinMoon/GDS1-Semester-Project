using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiillboardFX : MonoBehaviour
{
    Transform camTransform;
    Quaternion originRotate;
    Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
    }
}
