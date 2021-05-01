using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory_Oven : MonoBehaviour
{
    [SerializeField] Transform doorPivotPoint;

    public void OpenDoor()
    {
        StartCoroutine(IncrementOpenDoor());
    }

    IEnumerator IncrementOpenDoor()
    {
        while (doorPivotPoint.localRotation.x > -95)
        {
            Vector3 rot = doorPivotPoint.localRotation.eulerAngles;
            rot.x -= 1;
            doorPivotPoint.localRotation = Quaternion.Euler(rot);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
