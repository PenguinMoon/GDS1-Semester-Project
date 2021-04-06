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

    [SerializeField] Transform firePoint;
    [SerializeField] ParticleSystem fireParticle;
    [SerializeField] ParticleSystem sleepParticle;
    [SerializeField] ParticleSystem hitParticle;
    [SerializeField] LayerMask detectionMask;
    [SerializeField] GameObject bulletPrefab;

    bool isActive;
    [SerializeField] float delayBetweenReloading = 1.5f;
    float currentReloadDelay = 0f;

    private void Start()
    {
        currentAmmo = maxAmmo;
        ammoText.text = currentAmmo.ToString();
    }

    private void Update()
    {
        if (isActive)
        {
            if (currentFireDelay > 0)
                currentFireDelay -= Time.deltaTime;

            SearchLane();

            if (currentAmmo == 0)
            {
                sleepParticle.Play();
                isActive = false;
            }

        }
        else
        {
            if (currentReloadDelay > 0)
                currentReloadDelay -= Time.deltaTime;

            if (currentReloadDelay <= 0f)
            {
                ReloadAmmo(1);
                currentReloadDelay = delayBetweenReloading;
            }
            

            if (currentAmmo == maxAmmo)
            {
                sleepParticle.Stop();
                isActive = true;
            }

        }

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
            fireParticle.Play();
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

    public void HitByPlayer()
    {
        ReloadAmmo(Mathf.FloorToInt(maxAmmo/10));
        hitParticle.Play();
    }
}
