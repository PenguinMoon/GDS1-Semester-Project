using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyUI : MonoBehaviour
{

    public Slider healthBar;


    public void UpdateHealthBar(float health, float maxHealth)
    {
        healthBar.value = health / maxHealth;
    }
}
