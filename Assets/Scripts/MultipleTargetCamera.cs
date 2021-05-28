using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MultipleTargetCamera : MonoBehaviour
{
    List<Transform> playerPositions = new List<Transform>();

    [SerializeField] Transform followTarget;

    Vector3 camOffsetDirection = Vector3.zero;

    [SerializeField] float maxZoom = 25f;
    [SerializeField] float minZoom = 5f;

    public float extraZoom = 0f;

    private void Start()
    {
        camOffsetDirection = transform.position - followTarget.position;
        camOffsetDirection.Normalize();
    }
    private void LateUpdate()
    {
        if (playerPositions.Count > 0) {

            if (playerPositions.Count == 1)
                followTarget.position = playerPositions[0].position + (camOffsetDirection * extraZoom);
            else {
                var bounds = new Bounds(playerPositions[0].position, Vector3.zero);
                foreach (Transform player in playerPositions) {
                    bounds.Encapsulate(player.position);
                }

                Vector3 lookPosition = bounds.center;
                float distBetweenPlayers = Mathf.Max(bounds.size.x, bounds.size.z);

                float zoom = Mathf.Clamp(distBetweenPlayers, minZoom, maxZoom);

                Vector3 finalPos = lookPosition + (camOffsetDirection * (zoom + extraZoom));

                followTarget.position = finalPos;
            }
        }
    }

    public void NewPlayerSpawned() {
        playerPositions.Clear();

        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player")) {
            playerPositions.Add(g.transform);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawCube(followTarget.position, Vector3.one);
    }
}
