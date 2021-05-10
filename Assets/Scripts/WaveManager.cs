using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    List<EnemySpawner> spawners = new List<EnemySpawner>();

    [Header("Index: Wave, Value: Amount of Enemies")]
    [SerializeField] List<int> waves;

    [SerializeField] float timeBetweenWaves = 15f;
    [HideInInspector] public int currentWave = 0;

    LevelLoader levelLoader;

    private void Awake()
    {
        if (spawners.Count > 0)
            spawners.Clear();

        foreach (EnemySpawner e in FindObjectsOfType<EnemySpawner>())
            spawners.Add(e);

        levelLoader = FindObjectOfType<LevelLoader>();
        if (!levelLoader)
            Debug.LogWarning("NO LEVEL LOADER IN THIS LEVEL - PLEASE LOAD FROM MAIN MENU");
    }

    private void Start()
    {
        StartCoroutine(RunWaves());
    }

    IEnumerator RunWaves()
    {
        for (int i = 0; i < waves.Count; i++)
        {
            yield return new WaitForSeconds(timeBetweenWaves);

            currentWave = i+1;
            //Spawn enemies at each spawner
            foreach (EnemySpawner spawner in spawners)
                StartCoroutine(spawner.SpawnWaveOfNum(waves[i]));

            //Wait while any enemy spawners have enemies alive
            yield return new WaitWhile(IsWaveInProgress);
        }

        OnAllWavesCompleted();
    }

    private bool IsWaveInProgress()
    {
        foreach(EnemySpawner spawner in spawners)
            if (spawner.HasEnemies())
                return true;

        return false;
    }

    private void OnAllWavesCompleted()
    {
        levelLoader.LoadLevel("Game Win");
    }

    public int GetWavesRemaining()
    {
        return waves.Count - currentWave;
    }
}
