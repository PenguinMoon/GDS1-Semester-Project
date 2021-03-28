using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] bool infiniteAmmo = false;

    [SerializeField] float delayBetweenFiring = 0.5f;
    float currentFireDelay = 0f;
    [SerializeField] float range = 10f;
    [SerializeField]int maxAmmo = 20;
    int currentAmmo;

    [SerializeField]Transform firePoint;
    [SerializeField] LayerMask detectionMask;
    [SerializeField] GameObject bulletPrefab;

    public GameObject plate = null;

    private void Start()
    {
        currentAmmo = maxAmmo;
    }

    private void Update()
    {
        if (currentFireDelay > 0)
            currentFireDelay -= Time.deltaTime;

        SearchLane();
    }

    private void SearchLane()
    {
        RaycastHit hit;

        Debug.DrawRay(firePoint.position, firePoint.forward * range);

        if (Physics.Raycast(firePoint.position, Vector3.forward, out hit, range, detectionMask))
        {
            Fire();
        }
    }
    private void Fire()
    {
        if (currentAmmo > 0 && currentFireDelay <= 0f)
        {
            Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            currentFireDelay = delayBetweenFiring;

            if (!infiniteAmmo)
                currentAmmo--;
        }
    }
}
