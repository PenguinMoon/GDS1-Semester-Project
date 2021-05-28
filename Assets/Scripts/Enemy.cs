using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    float health = 0;
    [SerializeField] float maxHealth = 2f;

    [SerializeField] GameObject partToDrop;
    [SerializeField, Range(0, 100)] int chanceToDropPart;
    int amountOfCurrencyToDrop = 1;


    public int damageToDeal = 1;

    [SerializeField] ParticleSystem deathParticle;

    MeshRenderer rend;
    EnemyUI enemyUI;

    Vector3 currentDirection = Vector3.back;
    Rigidbody rb;

    private void Awake()
    {
        rend = GetComponent<MeshRenderer>();
        enemyUI = GetComponentInChildren<EnemyUI>();

        health = maxHealth;

        amountOfCurrencyToDrop = 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "WorkshopWall":
                Instantiate(deathParticle, transform.position, Quaternion.identity);
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
        Instantiate(partToDrop, GetDropPosition(), Quaternion.identity);

        Instantiate(deathParticle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
