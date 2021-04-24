using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float health = 2;

    [SerializeField] GameObject bitsPrefab;
    [SerializeField] GameObject circuitsPrefab;
    [SerializeField, Range(0, 10)] int amountOfCurrencyToDrop = 2;

    bool willDropCircuits = false;
    [SerializeField] Material circuitDropMaterial;
    MeshRenderer rend;

    Vector3 currentDirection = Vector3.back;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<MeshRenderer>();

        willDropCircuits = Random.Range(0, 100) < 10;

        if (willDropCircuits)
            rend.material = circuitDropMaterial;
    }

    private void Update()
    {
        rb.velocity = currentDirection * moveSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.collider.gameObject.tag)
        {
            case "WorkshopWall":
                Destroy(this.gameObject);
                break;
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0)
            Die();
    }

    private void Die()
    {
        if (willDropCircuits)
            Instantiate(circuitsPrefab, transform.position + (Random.onUnitSphere / 2), Quaternion.identity);
        else
            for (int i = 0; i < amountOfCurrencyToDrop; i++)
                Instantiate(bitsPrefab, transform.position + (Random.onUnitSphere / 2), Quaternion.identity);

        Destroy(gameObject);
    }
}
