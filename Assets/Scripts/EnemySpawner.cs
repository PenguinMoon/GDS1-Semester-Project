using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] bool randomizeParameters = false;

    [SerializeField] GameObject enemyPrefab;

    [SerializeField, Range(1, 10)] int spawnDelay;
    [SerializeField, Range(1, 10)] int maxNumInLane;

    private float timeRemaining = 0f;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    private void Start()
    {
        if (randomizeParameters)
        {
            spawnDelay = Random.Range(1, 10);
            maxNumInLane = Random.Range(1, 10);
        }
    }

    private void Update()
    {

        if (timeRemaining <= 0f)
            SpawnEnemy();

        else
            timeRemaining -= Time.deltaTime;
    }

    private void SpawnEnemy()
    {
        timeRemaining = spawnDelay;

        int enemyCount = 0;
        foreach (GameObject enemy in spawnedEnemies)
            if (enemy != null)
                enemyCount++;

        if (enemyCount < maxNumInLane)
            spawnedEnemies.Add(Instantiate(enemyPrefab, transform.position, Quaternion.identity));
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position, Vector3.one * 0.5f);
    }

}
