using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] bool randomizeParameters = false;

    [Space]
    [Header("Enemies in order of rarity")]
    [SerializeField] List<GameObject> enemies;

    [SerializeField, Range(1, 5)] int _spawnDelay;
    [SerializeField, Range(1, 20)] int _maxNumInLane;

    private List<Transform> _enemies = new List<Transform>();

    [SerializeField]
    private List<Transform> _enemyGoal;

    [SerializeField] bool generateLane = false;
    [SerializeField] GameObject laneTile;
    List<GameObject> visualLane = new List<GameObject>();


    private float _timeTilNextWave = 20f;
    public int _waveIndex = 0;
    public bool isFinished = false;

    private void Awake()
    {
        GenerateLane();
    }

    IEnumerator RunSpawner()
    {
        yield return new WaitForSeconds(_timeTilNextWave);
        while (_waveIndex < 10)
        {
            //yield return SpawnWave();

            yield return new WaitWhile(EnemyAlive);

            yield return new WaitForSeconds(_timeTilNextWave);
        }

        isFinished = true;
    }

    //Returns how many enemies should spawn on given wave
    private int GetWaveEnemyNum(int wave)
    {
        return wave * 3;
    }

    //Called by the Wave Manager Script
    public IEnumerator SpawnWaveOfNum(int num, List<GameObject> options)
    {
        Debug.Log("Spawning " + num + " enemies");
        for (int i = 0; i < num; i++)
        {
            SpawnRandomEnemy(options);
            yield return new WaitForSeconds(1.5f);
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
        _enemies.Last().GetComponent<EnemyAI>().SetGoal(_enemyGoal.ToList());
        _enemies.Last().GetComponent<EnemyAI>().Begin();
        _enemies.Last().transform.SetParent(transform);
    }

    private void SpawnRandomEnemy(List<GameObject> options)
    {
        int rand = Random.Range(0, options.Count - 1);

        SpawnEnemy(options[rand]);
    }

    private GameObject GetEnemyToSpawn()
    {
        if (enemies.Count == 1)
            return enemies[0];
        else
        {
            int rand = Random.Range(0, 100);

            if (rand < 25)
                return enemies[1];
            else
                return enemies[0];
        }
    }

    public int EnemiesAlive()
    {
        _enemies = _enemies.Where(enemy => enemy != null).ToList();
        return _enemies.Count;
    }

    private bool LaneFull() => EnemiesAlive() >= _maxNumInLane;

    private bool EnemyAlive() => EnemiesAlive() > 0;

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

        generateLane = false;
    }

}
