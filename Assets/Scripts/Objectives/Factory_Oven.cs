using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory_Oven : MonoBehaviour
{
    [SerializeField] GameObject doorPivotPoint;
    [SerializeField] float openXRotation = -95;

    Collider[] doorChildren;

    private void Awake()
    {
        doorChildren = GetComponentsInChildren<Collider>();
    }

    public void OpenDoor()
    {
        SetChildrenCollision(false);
        LeanTween.rotateLocal(doorPivotPoint, new Vector3(openXRotation, 0, 0), 2f).setOnComplete(() => SetChildrenCollision(true));
    }

    private void SetChildrenCollision(bool isEnabled)
    {
        foreach (Collider col in doorChildren)
        {
            col.enabled = isEnabled;
        }
    }
}
