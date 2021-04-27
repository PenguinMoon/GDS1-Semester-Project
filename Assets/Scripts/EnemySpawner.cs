using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] bool randomizeParameters = false;

    [SerializeField] GameObject enemyPrefab;

    [SerializeField, Range(1, 10)] int _spawnDelay;
    [SerializeField, Range(1, 10)] int _maxNumInLane;

    private List<Transform> _enemies = new List<Transform>();

    [SerializeField]
    private List<Transform> _enemyGoal;
    

    private float _timeTilNextWave = 10f;
    public int _waveIndex = 0;
    public bool isFinished = false;

    private void Start()
    {
        StartCoroutine(RunSpawner());
    }

    IEnumerator RunSpawner()
    {
        yield return new WaitForSeconds(_timeTilNextWave);
        while (_waveIndex < 10)
        {
            yield return SpawnWave();

            yield return new WaitWhile(EnemyAlive);

            yield return new WaitForSeconds(_timeTilNextWave);
        }

        isFinished = true;
    }

    private IEnumerator SpawnWave()
    {
        _waveIndex++;
        for (int i = 0; i < _waveIndex; i++)
        {
            yield return new WaitWhile(LaneFull);
            SpawnEnemy();
            yield return new WaitForSeconds(_spawnDelay);
        }
    }

    private void SpawnEnemy()
    {
        _enemies.Add(Instantiate(enemyPrefab, transform.position, transform.rotation).transform);
        _enemies.Last().GetComponent<EnemyAI>().SetGoal(_enemyGoal.ToList());
        _enemies.Last().GetComponent<EnemyAI>().Begin();
    }

    private int EnemiesAlive()
    {
        _enemies = _enemies.Where(enemy => enemy != null).ToList();
        return _enemies.Count;
    }

    private bool LaneFull() => EnemiesAlive() >= _maxNumInLane;

    private bool EnemyAlive() => EnemiesAlive() > 0;

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position, Vector3.one * 0.5f);
    }

}
