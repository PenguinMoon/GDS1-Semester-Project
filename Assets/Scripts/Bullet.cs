using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField] protected float maxLifeTime = 15f;
    float lifeTime = 0f;

    [SerializeField] protected float damageAmount = 1;
    [SerializeField] protected float speed = 15f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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

        if (!(collision.collider.gameObject.tag == "Player" && collision.collider.gameObject.tag == "Bullet"))
            Destroy(gameObject);
    }

    protected virtual void OnEnemyHit(GameObject enemy)
    {
        DealDamageTo(enemy.GetComponent<Enemy>());
    }

    protected void DealDamageTo(Enemy enemy)
    {
        enemy.TakeDamage(damageAmount);
    }
}
