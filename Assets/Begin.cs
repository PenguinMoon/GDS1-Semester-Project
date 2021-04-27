using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Start : State
{
    public Start(EnemyAI _enemyAI) : base(_enemyAI) { }

    public override IEnumerator Begin()
    {
        Debug.Log("Enemy: " + _enemyAI.ToString() + " has started moving.");
        _enemyAI.GoToGoal();
        yield return null;
    }
}
