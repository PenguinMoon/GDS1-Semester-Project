using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret_Flamethrower : SmartTurret
{
    [SerializeField] ParticleSystem fireParticles;

    protected override void Update()
    {
        base.Update();
        if (isFiring && isActive)
            fireParticles.Play();
        else
            fireParticles.Stop();
    }
}
