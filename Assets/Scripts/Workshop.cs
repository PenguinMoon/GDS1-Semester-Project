using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Workshop : MonoBehaviour
{
    public int health = 100;
    [HideInInspector] public int startingHP;
    [SerializeField] HUDController hud;
    MultiplayerManager multiManager;

    LevelLoader levelLoader;

    private void Awake()
    {
        levelLoader = FindObjectOfType<LevelLoader>();
        if (!levelLoader)
            Debug.LogWarning("NO LEVEL LOADER IN THIS LEVEL - PLEASE LOAD FROM MAIN MENU");

        multiManager = FindObjectOfType<MultiplayerManager>();
        startingHP = health;
    }

    // Start is called before the first frame update
    void Start()
    {
        hud.SetMaxHP(health);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        hud.UpdateHP(health);

        if (health <= 0 && !levelLoader.isLoadingLevel)
        {
            multiManager.EndLevel(false);
            //levelLoader.LoadLevel("Game Over");
        }
    }
}
