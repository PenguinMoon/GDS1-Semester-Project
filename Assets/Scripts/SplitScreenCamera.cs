using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplitScreenCamera : MonoBehaviour
{
    [SerializeField] GameObject p1;
    [SerializeField] GameObject p2;
    [SerializeField] Camera p1Cam;
    [SerializeField] Camera p2Cam;
    [SerializeField] GameObject splitScreenBar;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(p1.transform.position, p2.transform.position) <= 12.5f)
        {
            p1Cam.rect = new Rect(0, 0, 1f, 1f);
            p2Cam.enabled = false;
            splitScreenBar.SetActive(false);
        }
        else
        {
            p1Cam.rect = new Rect(-0.5f, 0, 1f, 1f);
            p2Cam.enabled = true;
            splitScreenBar.SetActive(true);
        }
    }
}
