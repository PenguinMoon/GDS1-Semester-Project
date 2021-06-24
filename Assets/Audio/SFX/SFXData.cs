using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerSFXData", menuName = "Player SFX Data", order = 51)]
public class SFXData : ScriptableObject
{
    [SerializeField] AudioClip towerPlace;
    [SerializeField] AudioClip towerPickup;
    [SerializeField] AudioClip footstep;
    [SerializeField] AudioClip whack;
    [SerializeField] AudioClip currencyPickup;

    public AudioClip TowerPlace
    {
        get
        {
            return towerPlace;
        }
    }

    public AudioClip TowerPickup
    {
        get
        {
            return towerPickup;
        }
    }

    public AudioClip Footstep
    {
        get
        {
            return footstep;
        }
    }

    public AudioClip Whack
    {
        get
        {
            return whack;
        }
    }

    public AudioClip CurrencyPickup
    {
        get
        {
            return currencyPickup;
        }
    }
}
