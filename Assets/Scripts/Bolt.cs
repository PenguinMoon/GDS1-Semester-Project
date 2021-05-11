using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bolt : MonoBehaviour
{
    //float lifeTime = 0f;
    float maxLifeTime = 30f;

    // How long until the material colour starts flashing e.g. starts flashing after 15secs
    // NOTE: Time should always be LOWER than maxLifeTime, or else it doesn't flash at all
    float startFlashTime = 25f;

    Material _material; // The material on the gameobject
    Color origColour;   // Material's original colour

    private void Start()
    {
        _material = GetComponent<Renderer>().material;
        origColour = _material.color;

        StartCoroutine(DisappearFlash());
        Destroy(gameObject, maxLifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        //if (lifeTime >= maxLifeTime)
        //    Destroy(gameObject);
        //else
        //    lifeTime += Time.deltaTime;

        transform.Rotate(Vector3.left);
        transform.Rotate(Vector3.up * 0.5f);
    }

    // Makes the material's colour change between the original colour and white
    // when the currency is about to disappear
    IEnumerator DisappearFlash()
    {
        yield return new WaitForSeconds(startFlashTime);
        while (enabled)
        {
            _material.color = Color.white;
            yield return new WaitForSeconds(0.2f);  // Change the float to change delay between flashes
            _material.color = origColour;
            yield return new WaitForSeconds(0.2f);  // Change the float to change delay between flashes
        }
    }

    private void OnDestroy()
    {
        // Destroys the material to prevent too many duplicates of the material.
        // (When you change the material on a renderer, it creates a duplicate
        // to not affect the material on other gameobjects. You have to destroy it
        // to not have a lot of materials that will affect performance.)
        Destroy(_material);
    }
}
