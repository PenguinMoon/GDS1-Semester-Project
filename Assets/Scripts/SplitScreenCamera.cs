using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class SplitScreenCamera : MonoBehaviour
{
    [SerializeField] GameObject p1;
    [SerializeField] GameObject p2;
    [SerializeField] Camera p1Cam;
    [SerializeField] Camera p2Cam;
    [SerializeField] CinemachineVirtualCameraBase centreCam;
    [SerializeField] GameObject splitScreenBar;
    bool isCentreCamOn;

    // Start is called before the first frame update
    void Start()
    {
        isCentreCamOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(p1.transform.position, p2.transform.position) <= 12.5f)
        {
            p1Cam.enabled = false;
            p2Cam.enabled = false;
            centreCam.enabled = true;
            isCentreCamOn = true;
            splitScreenBar.SetActive(false);
        }
        else
        {
            centreCam.enabled = false;
            isCentreCamOn = false;
            p1Cam.enabled = true;
            p2Cam.enabled = true;
            splitScreenBar.SetActive(true);
        }
    }

    private void FixedUpdate()
    {
        if (isCentreCamOn)
            centreCam.transform.position = new Vector3((p1Cam.transform.position.x + p2.transform.position.x) / 2, p1Cam.transform.position.y, p1Cam.transform.position.z);
    }
}
