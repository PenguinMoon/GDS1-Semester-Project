using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartTurret : MonoBehaviour
{
    [SerializeField] Transform gunTransform;
    [SerializeField] LayerMask enemyMask;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;

    [SerializeField] float FOVAngle = 360f;
    [SerializeField] float rotSpeed = 2f;
    [SerializeField] float range = 15f;

    float currentFireDelay = 0f;
    float fireDelay = 0.5f;

    bool isFiring = false;

    Vector3 debugTargetPos = Vector3.zero;

    private void Update()
    {
        if (currentFireDelay > 0)
            currentFireDelay -= Time.deltaTime;

        AimGun();
    }

    private void AimGun()
    {
        RaycastHit[] hits = Physics.SphereCastAll(gunTransform.position, range, transform.forward, range, enemyMask);

        List<Transform> foundEnemies = new List<Transform>();

        foreach (RaycastHit hit in hits)
            foundEnemies.Add(hit.transform);

        Transform closestEnemy = GetClosestTransform(foundEnemies);

        isFiring = false;

        if (closestEnemy && GetAngleToPos(closestEnemy.position) <= FOVAngle)
        {
            Vector3 target = closestEnemy.position - closestEnemy.forward;
            debugTargetPos = target;

            TurnToFace(target);

            if (GetAngleToPos(target) <= 5f && currentFireDelay <= 0f)
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
    }

    private void TurnToFace(Vector3 position)
    {
        Quaternion targetRotation = Quaternion.LookRotation(position - gunTransform.position);
        gunTransform.rotation = Quaternion.Lerp(gunTransform.rotation, targetRotation, Time.deltaTime * (isFiring ? rotSpeed * 1.5f : rotSpeed));
    }

    private float GetAngleToPos(Vector3 position)
    {
        Vector3 targetDir = position - gunTransform.position;
        return  Vector3.Angle(targetDir, gunTransform.forward);
    }

    private Transform GetClosestTransform(List<Transform> transforms)
    {
        float dist = float.MaxValue;
        Transform closestTransform = null;

        foreach (Transform t in transforms)
        {
            float d = Vector3.Distance(gunTransform.position, t.position);

            if (d < dist)
            {
                dist = d;
                closestTransform = t;
            }
        }

        return closestTransform;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(gunTransform.position, range);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(debugTargetPos, 0.3f);
    }
}
