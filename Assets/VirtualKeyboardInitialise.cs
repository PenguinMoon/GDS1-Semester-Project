using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VirtualKeyboardInitialise : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        if (InputSystem.GetDevice<VirtualKeyboardDevice>() == null)
        {
            InputSystem.AddDevice<VirtualKeyboardDevice>();
        }

        var dvc = InputSystem.GetDevice<VirtualKeyboardDevice>();
        InputSystem.EnableDevice(dvc);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
