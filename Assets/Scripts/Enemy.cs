using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    float health = 0;
    float maxHealth = 2f;

    [SerializeField] GameObject bitsPrefab;
    [SerializeField] GameObject circuitsPrefab;
    int amountOfCurrencyToDrop = 0;

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

        willDropCircuits = Random.Range(0, 100) < 10;

        if (willDropCircuits)
        {
            rend.material = circuitDropMaterial;
            maxHealth *= 3f;
        }

        health = maxHealth;

        amountOfCurrencyToDrop = Random.Range(0, 100) < 40 ? 1 : 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "WorkshopWall":
                Destroy(gameObject);
                break;
        }
    }

    //Changed so player can walk through the area

    //private void OnCollisionEnter(Collision collision)
    //{
    //    switch (collision.collider.gameObject.tag)
    //    {
    //        case "WorkshopWall":
    //            Destroy(gameObject);
    //            break;
    //    }
    //}

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
