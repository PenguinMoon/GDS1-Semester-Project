using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObstacle : MonoBehaviour
{
    private Vector3 _startingPosition;
    // Start is called before the first frame update
    void Start()
    {
        _startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _startingPosition + new Vector3(Mathf.Sin(Time.time + 100) * 15, 0, 0);
    }
}
