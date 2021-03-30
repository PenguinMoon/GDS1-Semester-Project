using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        hud.UpdateHP(health);
    }
}
