using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective_Train_Platform : MonoBehaviour
{
    [SerializeField] GameObject Leader;

    float distanceBehind = 0;

    List<Vector3> path = new List<Vector3>();

    int tweenID = 0;

    private void Awake()
    {
        distanceBehind = Vector3.Distance(transform.position, Leader.transform.position);
    }

    private void Update()
    {
        if (path.Count > 0)
        {
            RotateToFacePath();

            if (Vector3.Distance(transform.position, Leader.transform.position) > distanceBehind)
                transform.position = Vector3.Lerp(transform.position, path[0], 4f * Time.deltaTime);

            if (Vector3.Distance(transform.position, path[0]) < 1f)
            {
                path.RemoveAt(0);
            }
        }
        else if (Vector3.Distance(transform.position, Leader.transform.position) > distanceBehind)
            AddPathPoint();

        
    }

    private void RotateToFacePath()
    {
        if (LeanTween.descr(tweenID) == null)
        {
            tweenID = LeanTween.rotate(gameObject, Quaternion.LookRotation(path[0] - transform.position).eulerAngles, 0.5f).id;
        }
    }

    private void AddPathPoint()
    {
        path.Add(Leader.transform.position);
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
        foreach (Vector3 p in path)
        {
            Gizmos.DrawCube(p, Vector3.one);
        }
    }
}
