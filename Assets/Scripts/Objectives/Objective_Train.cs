using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective_Train : MonoBehaviour
{
    [SerializeField] List<Transform> waypoints;

    int nextWaypointIndex = 0;

    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float timeToRotate = 0.5f;
    [SerializeField] float waypointTolerance = 1f;

    int tweenID = 0;

    private bool isRepaired = false;

    private void Update()
    {
        if (isRepaired)
        {
            float distance = Vector3.Distance(transform.position, waypoints[nextWaypointIndex].position);

            if (distance < waypointTolerance)
            {
                nextWaypointIndex = nextWaypointIndex < waypoints.Count - 1 ? nextWaypointIndex + 1 : 0;
                RotateInDir();
            }

            MoveForward();
        }
    }

    public void RepairTrain()
    {
        isRepaired = true;
    }

    private void MoveForward()
    {
        Vector3 towardsTarget = waypoints[nextWaypointIndex].position - transform.position;

        transform.position += (towardsTarget.normalized * movementSpeed) * Time.deltaTime;
    }

    private void RotateInDir()
    {
        if (LeanTween.descr(tweenID) == null)
        {
            tweenID = LeanTween.rotate(gameObject, Quaternion.LookRotation(waypoints[nextWaypointIndex].position - transform.position).eulerAngles, timeToRotate).id;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.transform.parent = transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.transform.parent = null;
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < waypoints.Count; i++)
        {
            int toIndex = i + 1;

            if (i == waypoints.Count - 1)
                toIndex = 0;

            Gizmos.DrawLine(waypoints[i].position, waypoints[toIndex].position);

            Gizmos.DrawWireSphere(waypoints[i].position, waypointTolerance);
        }
    }
}
