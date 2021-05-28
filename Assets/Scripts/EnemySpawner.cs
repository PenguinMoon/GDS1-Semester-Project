using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private List<Transform> _enemies = new List<Transform>();

    private ParticleSystem _enemySpawnParticle;

    [SerializeField]
    private List<Transform> _enemyGoal;

    [SerializeField] GameObject laneTile;
    List<GameObject> visualLane = new List<GameObject>();

    private void Awake()
    {
        GenerateLane();
        _enemySpawnParticle = GetComponentInChildren<ParticleSystem>();
    }

    //Called by the Wave Manager Script
    public IEnumerator SpawnWaveOfNum(int num, List<GameObject> options)
    {
        for (int i = 0; i < num; i++)
        {
            SpawnRandomEnemy(options);
            yield return new WaitForSeconds(1.7f);
        }
    }

    //Called by the Wave Manager Script
    public bool HasEnemies()
    {
        return EnemiesAlive() > 0;
    }

    private void SpawnEnemy(GameObject enemy)
    {
        _enemies.Add(Instantiate(enemy, transform.position, transform.rotation).transform);
        _enemySpawnParticle.Play();
        _enemies.Last().GetComponent<EnemyAI>().SetGoal(_enemyGoal.ToList());
        _enemies.Last().GetComponent<EnemyAI>().Begin();
        _enemies.Last().transform.SetParent(transform);
    }

    private void SpawnRandomEnemy(List<GameObject> options)
    {
        int rand = Random.Range(0, options.Count);

        SpawnEnemy(options[rand]);
    }

    private int EnemiesAlive()
    {
        _enemies = _enemies.Where(enemy => enemy != null).ToList();

        return _enemies.Count;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position, Vector3.one);

        Gizmos.color = Color.green;
        for (int i = 0; i < _enemyGoal.Count; i++)
        {
            if (i == 0)
            {
                Gizmos.DrawLine(transform.position, _enemyGoal[i].position);
            }

            if (i < _enemyGoal.Count - 1)
                Gizmos.DrawLine(_enemyGoal[i].position, _enemyGoal[i + 1].position);
        }
    }

    public void GenerateLane()
    {
        transform.rotation = Quaternion.LookRotation(_enemyGoal[0].position - transform.position);

        foreach (GameObject g in visualLane)
            if (g != null)
                Destroy(g);

        for (int i = 0; i < _enemyGoal.Count; i++)
        {
            Transform previousValue;

            if (i == 0)
                previousValue = transform;
            else
                previousValue = _enemyGoal[i - 1];


            Vector3 pos = (previousValue.position + _enemyGoal[i].position) / 2;

            Physics.Raycast(pos, Vector3.down, out RaycastHit hit);
            if (hit.collider)
                pos.y = hit.point.y;

            Quaternion rot = Quaternion.LookRotation(_enemyGoal[i].position - previousValue.position);
            float dist = Vector3.Distance(previousValue.position, _enemyGoal[i].position);

            GameObject tile = Instantiate(laneTile, pos, rot);
            tile.transform.localScale = new Vector3(2, 0.05f, dist + 1);
            tile.transform.SetParent(transform);

            visualLane.Add(tile);
        }
    }

}
