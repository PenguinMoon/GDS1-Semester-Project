using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody rb;

    float maxLifeTime = 15f;
    float lifeTime = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        rb.velocity = transform.forward * 15f;

        if (lifeTime > maxLifeTime)
            Destroy(gameObject);
        else
            lifeTime += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Enemy")
            Destroy(gameObject);
        if (collision.collider.gameObject.tag == "Player")
            Destroy(gameObject);
    }
}
