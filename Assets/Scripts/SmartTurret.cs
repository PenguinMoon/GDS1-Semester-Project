using System.Collections;
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

    // Turret Vairables

    [SerializeField] float FOVAngle = 360f;
    float viewAngle = 0f;
    [SerializeField] float rotSpeed = 2f;
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
        if (currentFireDelay > 0)
            currentFireDelay -= Time.deltaTime;

        if (currentAmmo == 0)
        {
            //Sleep particle start
            isActive = false;
            ammoImage.color = Color.grey;
            particles[(int)ParticleEffects.Inactive].Play();
        }
        else if (currentAmmo == maxAmmo)
        {
            //Sleep particle end
            isActive = true;
            ammoImage.color = Color.green;
            particles[(int)ParticleEffects.Inactive].Stop();
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

    private void AimGun()
    {
        RaycastHit[] hits = Physics.SphereCastAll(baseTransform.position, range, transform.forward, 1f, enemyMask);

        List<Transform> foundEnemies = new List<Transform>();

        foreach (RaycastHit hit in hits)
        {
            //Commented this out as it was causing issues with the new enemy navigation ??????

            //Physics.Linecast(firePoint.position, hit.transform.position, out RaycastHit rayInfo);

            //Debug.Log(rayInfo.collider);

            //if (rayInfo.collider && rayInfo.collider.gameObject.tag == "Enemy")
            //{
            foundEnemies.Add(hit.transform);
            //}
        }


        Transform closestEnemy = GetClosestTransform(foundEnemies);

        isFiring = false;

        //Restrict view angle by half when turret is being held
        viewAngle = isBeingHeld ? FOVAngle * 0.5f : FOVAngle;

        if (viewAngle < 40f)
            viewAngle = 40f;

        if (closestEnemy && GetAngleToPos(closestEnemy.position) <= viewAngle)
        {
            Vector3 target = closestEnemy.position;

            if (Vector3.Distance(baseTransform.position, target) > 10f)
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
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        currentFireDelay = fireDelay;

        PlayParticle(ParticleEffects.Fire);

        currentAmmo--;
        UpdateAmmoUI();
    }

    private void TurnToFace(Vector3 position)
    {
        Quaternion targetRotation = Quaternion.LookRotation(position - gunTransform.position);
        gunTransform.rotation = targetRotation;//Quaternion.Lerp(gunTransform.rotation, targetRotation, Time.deltaTime * (isFiring ? rotSpeed * 3f: rotSpeed));
    }

    private float GetAngleToPos(Vector3 position)
    {
        Vector3 targetDir = position - baseTransform.position;
        float angle = Vector3.Angle(baseTransform.forward, targetDir);
        return angle;
    }

    private float GetFireAngleToPos(Vector3 position)
    {
        Vector3 targetDir = position - gunTransform.position;
        float angle = Vector3.Angle(gunTransform.forward, targetDir);
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
