using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAnimator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LeanTween.moveY(this.gameObject, 2, 0.5f).setEaseInCubic().setLoopPingPong();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
