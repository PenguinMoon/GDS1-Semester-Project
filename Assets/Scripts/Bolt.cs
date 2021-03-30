using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bolt : MonoBehaviour
{
    float lifeTime = 0f;
    float maxLifeTime = 20f;
    // Update is called once per frame
    void Update()
    {
        if (lifeTime >= maxLifeTime)
            Destroy(gameObject);
        else
            lifeTime += Time.deltaTime;

        transform.Rotate(Vector3.left);
        transform.Rotate(Vector3.up * 0.5f);
    }
}
