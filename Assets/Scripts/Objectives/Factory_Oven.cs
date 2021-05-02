using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory_Oven : MonoBehaviour
{
    [SerializeField] GameObject doorPivotPoint;
    [SerializeField] float openXRotation = -95;

    public void OpenDoor()
    {
        LeanTween.rotateLocal(doorPivotPoint, new Vector3(openXRotation, 0, 0), 2f);
    }
}
