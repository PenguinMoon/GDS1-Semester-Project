using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Turret : Object
{
    [SerializeField] bool infiniteAmmo = false;

    [SerializeField] float delayBetweenFiring = 0.5f;
    float currentFireDelay = 0f;
    [SerializeField] float range = 10f;
    [SerializeField]int maxAmmo = 20;
    int currentAmmo;
    [SerializeField] TextMeshProUGUI ammoText;

    [SerializeField]Transform firePoint;
    [SerializeField] LayerMask detectionMask;
    [SerializeField] GameObject bulletPrefab;

    private void Start()
    {
        currentAmmo = maxAmmo;
        ammoText.text = currentAmmo.ToString();
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

        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, range, detectionMask))
        {
            Fire();
        }
    }
    private void Fire()
    {
        if (currentAmmo > 0 && currentFireDelay <= 0f)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            currentFireDelay = delayBetweenFiring;

            if (!infiniteAmmo)
            {
                currentAmmo--;
                ammoText.text = currentAmmo.ToString();
            }
        }
    }

    public void ReloadAmmo(int ammo)
    {
        currentAmmo += ammo;
        ammoText.text = currentAmmo.ToString();
        if (currentAmmo > maxAmmo)
        {
            currentAmmo = maxAmmo;
            ammoText.text = currentAmmo.ToString();
        }
    }
}
