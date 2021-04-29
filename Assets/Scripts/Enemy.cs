using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    float health = 0;
    [SerializeField] float maxHealth = 0;

    [SerializeField] GameObject bitsPrefab;
    [SerializeField] GameObject circuitsPrefab;
    [SerializeField, Range(0, 10)] int amountOfCurrencyToDrop = 2;

    bool willDropCircuits = false;
    [SerializeField] Material circuitDropMaterial;
    MeshRenderer rend;
    EnemyUI enemyUI;

    Vector3 currentDirection = Vector3.back;
    Rigidbody rb;

    private void Awake()
    {
        rend = GetComponent<MeshRenderer>();
        enemyUI = GetComponentInChildren<EnemyUI>();

        health = maxHealth;

        willDropCircuits = Random.Range(0, 100) < 10;

        if (willDropCircuits)
            rend.material = circuitDropMaterial;
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
        enemyUI.UpdateHealthBar(health, maxHealth);
        if (health <= 0)
            Die();
    }

    private Vector3 GetDropPosition()
    {
        Vector3 pos = transform.position + (Random.onUnitSphere / 2);
        pos.y = 1f;
        return pos;
    }

    private void Die()
    {
        if (willDropCircuits)
            Instantiate(circuitsPrefab, GetDropPosition(), Quaternion.identity);
        else
            for (int i = 0; i < amountOfCurrencyToDrop; i++)
                Instantiate(bitsPrefab, GetDropPosition(), Quaternion.identity);

        Destroy(gameObject);
    }
}
