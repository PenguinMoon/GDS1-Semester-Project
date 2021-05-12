using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective_Bridge : MonoBehaviour
{
    [SerializeField] Transform[] pivotPoints;

    void Start() 
    {
        SetHalfRotationTo(90, 0.1f);
    }

    public void OnRepair() {
        SetHalfRotationTo(0, 2f);
    }

    void SetHalfRotationTo(float rot, float time) {
        foreach (Transform point in pivotPoints) {
            LeanTween.rotateX(point.gameObject, rot, time);
        }
    }
}
