using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    float health = 0;
    [SerializeField] float maxHealth = 2f;

    [SerializeField] GameObject partToDrop;
    [SerializeField, Range(0, 100)] int chanceToDropPart;
    int amountOfCurrencyToDrop = 0;

    MeshRenderer rend;
    EnemyUI enemyUI;

    Vector3 currentDirection = Vector3.back;
    Rigidbody rb;

    private void Awake()
    {
        rend = GetComponent<MeshRenderer>();
        enemyUI = GetComponentInChildren<EnemyUI>();

        health = maxHealth;

        amountOfCurrencyToDrop = Random.Range(0, 100) < chanceToDropPart ? 1 : 0;
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
        for (int i = 0; i < amountOfCurrencyToDrop; i++)
            Instantiate(partToDrop, GetDropPosition(), Quaternion.identity);

        Destroy(gameObject);
    }
}
