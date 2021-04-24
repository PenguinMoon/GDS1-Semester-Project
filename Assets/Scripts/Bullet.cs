using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody rb;

    float maxLifeTime = 15f;
    float lifeTime = 0f;

    protected int damageAmount = 1;
    protected float speed = 15f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Initialize();
    }

    protected virtual void Initialize()
    {

    }

    private void Update()
    {
        rb.velocity = transform.forward * speed;

        if (lifeTime > maxLifeTime)
            Destroy(gameObject);
        else
            lifeTime += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Enemy")
            OnEnemyHit(collision.gameObject);
        if (collision.collider.gameObject.tag == "Player")
            Destroy(gameObject);
    }

    protected virtual void OnEnemyHit(GameObject enemy)
    {
        DealDamageTo(enemy.GetComponent<Enemy>());
        Destroy(gameObject);
    }

    protected void DealDamageTo(Enemy enemy)
    {
        enemy.TakeDamage(damageAmount);
    }
}
