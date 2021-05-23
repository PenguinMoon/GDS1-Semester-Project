using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WaveManager : MonoBehaviour
{
    List<EnemySpawner> spawners = new List<EnemySpawner>();

    [SerializeField] List<WaveData> enemyWaves;

    [SerializeField] float timeBetweenWaves = 15f;
    [HideInInspector] public int currentWave = 0;
    [SerializeField] AudioClip gameWinClip;



    LevelLoader levelLoader;
    AudioSource audio;

    private void Awake()
    {
        if (spawners.Count > 0)
            spawners.Clear();

        foreach (EnemySpawner e in FindObjectsOfType<EnemySpawner>())
            spawners.Add(e);

        levelLoader = FindObjectOfType<LevelLoader>();
        if (!levelLoader)
            Debug.LogWarning("NO LEVEL LOADER IN THIS LEVEL - PLEASE LOAD FROM MAIN MENU");

        audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        StartCoroutine(RunWaves());
    }

    IEnumerator RunWaves()
    {
        yield return new WaitForSeconds(10f);   // Delay for intro cutscene

        for (int i = 0; i < enemyWaves.Count; i++)
        {
            yield return new WaitForSeconds(timeBetweenWaves);

            currentWave = i + 1;

            int numOfEnemiesToSpawn = enemyWaves[i].totalEnemies / spawners.Count;

            //Spawn enemies at each spawner
            foreach (EnemySpawner spawner in spawners)
                StartCoroutine(spawner.SpawnWaveOfNum(numOfEnemiesToSpawn, enemyWaves[i].enemies));

            //Wait while any enemy spawners have enemies alive
            yield return new WaitWhile(IsWaveInProgress);
        }

        OnAllWavesCompleted();
    }

    private bool IsWaveInProgress()
    {
        foreach (EnemySpawner spawner in spawners)
            if (spawner.HasEnemies())
                return true;

        return false;
    }

    private void OnAllWavesCompleted()
    {
        StartCoroutine(LoadLevelAfterAudio());
    }

    IEnumerator LoadLevelAfterAudio()
    {
        audio.clip = gameWinClip;
        audio.Play();

        yield return new WaitForSeconds(gameWinClip.length / 2.5f);

        levelLoader.LoadLevel("Game Win");
    }

    public int GetWavesRemaining()
    {
        return enemyWaves.Count - currentWave;
    }
}

[System.Serializable]
public struct WaveData
{
    public int totalEnemies;
    public List<GameObject> enemies;
}
