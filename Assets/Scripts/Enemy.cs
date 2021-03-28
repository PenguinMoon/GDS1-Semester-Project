using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] int health = 2;

    [SerializeField] GameObject currencyPrefab;
    [SerializeField, Range(0, 10)] int amountOfCurrencyToDrop = 2;

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

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.collider.gameObject.tag)
        {
            case "Bullet":
                TakeDamage(1);
                break;
        }
    }

    private void TakeDamage(int amount)
    {
        health -= amount;

        if (health <= 0)
            Die();
    }

    private void Die()
    {
        for (int i = 0; i < amountOfCurrencyToDrop; i++)
            Instantiate(currencyPrefab, transform.position + (Random.onUnitSphere * 2f), Quaternion.identity);

        Destroy(gameObject);
    }
}
