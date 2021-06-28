﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManagerV2 : MonoBehaviour
{
    List<EnemySpawner> spawners = new List<EnemySpawner>();

    [SerializeField] List<WaveDataV2> enemyWaves;

    [SerializeField] float timeBetweenWaves = 15f;
    [HideInInspector] public int currentWave = 0;
    [SerializeField] AudioClip gameWinClip;

    LevelLoader levelLoader;
    AudioSource audio;

    MultiplayerManager multiManager;

    public bool hasLevelStarted = false;    // Identifies if the level has started after intro cutscene delay

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
        multiManager = FindObjectOfType<MultiplayerManager>();
    }

    private void Start()
    {
        StartCoroutine(RunWaves());
    }

    IEnumerator RunWaves()
    {
        yield return new WaitForSeconds(10f);   // Delay for intro cutscene
        hasLevelStarted = true;

        for (int i = 0; i < enemyWaves.Count; i++)
        {
            yield return new WaitForSeconds(timeBetweenWaves);

            currentWave = i + 1;

            int numOfEnemiesToSpawn = enemyWaves[i].enemies.Count;

            //Scale waves to the amount of players - DISABLED
            //numOfEnemiesToSpawn *= MultiplayerManager.playerCount;

            //Spawn enemies at each spawner
            foreach (EnemySpawner spawner in spawners)
                StartCoroutine(spawner.SpawnWaveOfNum(numOfEnemiesToSpawn, enemyWaves[i].enemies));

            //Wait while any enemy spawners have enemies alive
            yield return new WaitWhile(IsWaveInProgress);
        }

        OnAllWavesCompleted();
    }

    public bool IsWaveInProgress()
    {
        foreach (EnemySpawner spawner in spawners)
            if (spawner.HasEnemies())
                return true;

        return false;
    }

    private void OnAllWavesCompleted()
    {
        Debug.LogWarning("Final score: " + MultiplayerManager.Score);
        StartCoroutine(LoadLevelAfterAudio());
    }

    IEnumerator LoadLevelAfterAudio()
    {
        audio.clip = gameWinClip;
        audio.Play();
        
        yield return new WaitForSeconds(gameWinClip.length / 2.5f);

        multiManager.EndLevel();
        levelLoader.LoadLevel("Game Win");
    }

    public int GetWavesRemaining()
    {
        return enemyWaves.Count - currentWave;
    }

    public string GetCurrentWave()
    {
        return currentWave.ToString() + " / " + enemyWaves.Count;
    }

    public float GetWaveDelay()
    {
        return timeBetweenWaves;
    }
}

[System.Serializable]
public struct WaveDataV2
{
    public List<GameObject> enemies;
}
