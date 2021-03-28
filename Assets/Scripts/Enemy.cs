using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;

    Vector3 currentDirection = Vector3.back;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();    
    }

    private void Update()
    {
        rb.velocity = currentDirection * moveSpeed;
    }
}
