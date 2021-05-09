using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    List<EnemySpawner> spawners = new List<EnemySpawner>();

    [Header("Index: Wave, Value: Amount of Enemies")]
    [SerializeField] List<int> waves;

    [SerializeField] float timeBetweenWaves = 15f;
    [HideInInspector] public int currentWave = 0;

    private void Awake()
    {
        if (spawners.Count > 0)
            spawners.Clear();

        foreach (EnemySpawner e in FindObjectsOfType<EnemySpawner>())
        {
            spawners.Add(e);
        }
    }

    private void Start()
    {
        StartCoroutine(RunWaves());
    }

    IEnumerator RunWaves()
    {
        //Wait before spawning the first wave
        yield return new WaitForSeconds(timeBetweenWaves);

        for (int i = 0; i < waves.Count-1; i++)
        {
            currentWave = i;
            //Spawn enemies at each spawner
            foreach (EnemySpawner spawner in spawners)
                StartCoroutine(spawner.SpawnWaveOfNum(waves[i]));

            //Wait while any enemy spawners have enemies alive
            yield return new WaitWhile(IsWaveInProgress);
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    private bool IsWaveInProgress()
    {
        foreach(EnemySpawner spawner in spawners)
        {
            if (spawner.HasEnemies())
                return true;
        }

        return false;
    }

    public int GetWavesRemaining()
    {
        return waves.Count - currentWave;
    }
}
