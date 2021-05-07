using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : Bullet
{
    float explosionRadius = 5f;

    [SerializeField] GameObject explosionPrefab;

    protected override void OnEnemyHit(GameObject enemy)
    {
        //Explode and deal damage to all enemies within radius
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, explosionRadius, transform.forward, 1f);

        foreach (RaycastHit hit in hits)
        {
            Enemy foundEnemy = hit.collider.gameObject.GetComponent<Enemy>();

            if (foundEnemy)
            {
                foundEnemy.TakeDamage(damageAmount);
            }
        }

        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
