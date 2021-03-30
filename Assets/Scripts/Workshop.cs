using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Workshop : MonoBehaviour
{
    [SerializeField] int health = 100;
    [SerializeField] HUDController hud;

    // Start is called before the first frame update
    void Start()
    {
        hud.SetMaxHP(health);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        hud.UpdateHP(health);
    }
}
