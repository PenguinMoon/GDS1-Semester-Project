using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective_Bridge : MonoBehaviour
{
    [SerializeField] Transform[] pivotPoints;
    [SerializeField] GameObject playerBlocker;

    void Start() 
    {
        SetHalfRotationTo(90, 0.1f);
    }

    public void OnRepair() {
        SetHalfRotationTo(0, 2f);
        playerBlocker.SetActive(false);
    }

    void SetHalfRotationTo(float rot, float time) {
        foreach (Transform point in pivotPoints) {
            LeanTween.rotateX(point.gameObject, rot, time);
        }
    }
}
