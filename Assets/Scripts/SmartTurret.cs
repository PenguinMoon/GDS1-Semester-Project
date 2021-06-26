using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmartTurret : Object
{
    [SerializeField] Transform gunTransform;
    [SerializeField] Transform baseTransform;
    [SerializeField] LayerMask enemyMask;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] Animator animator;
    [SerializeField] public GameObject nextUpgradeTurret;

    // Turret Vairables

    [HideInInspector] public bool repaired = true;
    [SerializeField] bool hasInfiniteAmmo = false;
    float FOVAngle = 360f;
    float viewAngle = 0f;
    [SerializeField] float range = 15f;
    [SerializeField] float fireDelay = 0.5f;
    [SerializeField] int maxAmmo = 30;
    int currentAmmo = 0;
    [SerializeField] float delayBetweenReloading = 1.5f;
    float currentReloadDelay = 0f;

    // Turret UI

    [SerializeField] Image ammoImage;
    protected bool isActive;
    [SerializeField] Image FOVIndicator;
    float indicatorTimer = 0f;
    float maxIndicatorTime = 5f;

    [SerializeField] List<ParticleSystem> particles;
    enum ParticleEffects
    {
        Hit,
        Inactive,
        Fire
    }

    float currentFireDelay = 0f;

    protected bool isFiring = false;

    Vector3 debugTargetPos = Vector3.zero;

    [Header("Audio Stuff")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip fireClip;

    private void Start()
    {
        currentAmmo = maxAmmo;

        // Half the FOV angle meaning that a full circle is 180degrees in either direction from forward
        FOVAngle *= 0.5f;

        UpdateFOVUI();
        UpdateAmmoUI();
    }

    protected virtual void Update()
    {
        if (repaired)
        {
            if (currentFireDelay > 0)
                currentFireDelay -= Time.deltaTime;

            if (currentAmmo == 0)
            {
                //Sleep particle start
                isActive = false;
                ammoImage.color = Color.red;
                particles[(int)ParticleEffects.Inactive].Play();
                animator.SetBool("Inactive", true);
            }
            else if (currentAmmo == maxAmmo)
            {
                //Sleep particle end
                isActive = true;
                ammoImage.color = Color.green;
                particles[(int)ParticleEffects.Inactive].Stop();
                animator.SetBool("Inactive", false);
            }

            if (isActive)
                AimGun();
            else
            {
                if (currentReloadDelay > 0)
                    currentReloadDelay -= Time.deltaTime;

                if (currentReloadDelay <= 0f)
                {
                    ReloadAmmo(1);
                    currentReloadDelay = delayBetweenReloading;
                }
            }

            UpdateFOVUI();
        }
    }

    private void AimGun()
    {
        Collider[] colls = Physics.OverlapSphere(baseTransform.position, range, enemyMask);

        List<Transform> foundEnemies = new List<Transform>();

        foreach (Collider coll in colls)
        {
            Debug.DrawLine(firePoint.position, coll.transform.position, Color.blue);
            foundEnemies.Add(coll.transform);
        }


        Transform closestEnemy = GetClosestTransform(foundEnemies);

        isFiring = false;

        //Restrict view angle by half when turret is being held
        viewAngle = isBeingHeld ? FOVAngle * 0.5f : FOVAngle;

        if (closestEnemy)
        {
            if (isBeingHeld && !(GetAngleToPos(closestEnemy.position) <= viewAngle))
                return;

            Vector3 target = closestEnemy.position;

            if (Vector3.Distance(baseTransform.position, target) > 5f)
                target += closestEnemy.forward;

            debugTargetPos = target;

            TurnToFace(target);

            if (currentFireDelay <= 0f && isActive)
            {
                Fire();
            }
        }
    }

    private void Fire()
    {
        isFiring = true;
        animator.SetTrigger("Fire");
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        currentFireDelay = fireDelay;

        audioSource.PlayOneShot(fireClip);
        PlayParticle(ParticleEffects.Fire);

        if (!hasInfiniteAmmo)
            currentAmmo--;

        UpdateAmmoUI();


    }

    private void TurnToFace(Vector3 position)
    {
        Quaternion targetRotation = Quaternion.LookRotation(position - gunTransform.position);
        //Quaternion.euler performs an error correcting with aiming. The aiming bone is off by 90 degrees in the model and this is not fixable as of yet.
        gunTransform.rotation = targetRotation *  Quaternion.Euler(0, 0, 0);//Quaternion.Lerp(gunTransform.rotation, targetRotation, Time.deltaTime * (isFiring ? rotSpeed * 3f: rotSpeed));
    }

    private float GetAngleToPos(Vector3 position)
    {
        Vector3 targetDir = position - baseTransform.position;
        float angle = Vector3.Angle(baseTransform.forward, targetDir);
        return angle;
    }

    private Transform GetClosestTransform(List<Transform> transforms)
    {
        float dist = float.MaxValue;
        Transform closestTransform = null;

        foreach (Transform t in transforms)
        {
            float d = Vector3.Distance(baseTransform.position, t.position);

            if (d < dist)
            {
                dist = d;
                closestTransform = t;
            }
        }

        return closestTransform;
    }

    public void ReloadAmmo(int ammo)
    {
        currentAmmo += ammo;

        if (currentAmmo > maxAmmo)
            currentAmmo = maxAmmo; 

        UpdateAmmoUI();
    }

    public void UpgradeTurret()
    {
        GameObject upgradedTurret = Instantiate(nextUpgradeTurret, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);
        upgradedTurret.GetComponent<SmartTurret>().plate = this.plate;
        Destroy(this.gameObject);
    }


    public void HitByPlayer()
    {
        ReloadAmmo(Mathf.FloorToInt(maxAmmo / 10));
        PlayParticle(ParticleEffects.Hit);
    }

    private void UpdateAmmoUI()
    {
        ammoImage.fillAmount = ((float)currentAmmo / maxAmmo);
    }

    private void UpdateFOVUI()
    {
        //Set FOV Indicator fill amount and rotation correctly
        float totalAngle = viewAngle * 2f;

        float fillPercent = totalAngle / 360f;
        float imgRotation = (totalAngle * -0.5f + 180) + transform.rotation.eulerAngles.y;

        //Incrementally perform the fillPercent change
        if (Mathf.Abs(FOVIndicator.fillAmount - fillPercent) > 0.05f)
        {
            int dir = FOVIndicator.fillAmount > fillPercent ? -1 : 1;

            FOVIndicator.fillAmount += 0.5f * dir * Time.deltaTime;
        }
        else
            FOVIndicator.fillAmount = fillPercent;


        FOVIndicator.rectTransform.eulerAngles = new Vector3(90, imgRotation, 0);

        //Set FOV Indicator size correctly
        float indicatorRadius = range * 2.1f;

        FOVIndicator.rectTransform.sizeDelta = new Vector2(indicatorRadius, indicatorRadius);

        //Set color alpha correctly
        if (isBeingHeld)
            indicatorTimer = maxIndicatorTime;
        else
            indicatorTimer -= Time.deltaTime;

        FOVIndicator.color = new Color(1, 1, 1, (indicatorTimer / maxIndicatorTime) * 0.5f);
    }

    private void PlayParticle(ParticleEffects effect)
    {
        particles[(int)effect].Play();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(debugTargetPos, 0.3f);
    }
}
