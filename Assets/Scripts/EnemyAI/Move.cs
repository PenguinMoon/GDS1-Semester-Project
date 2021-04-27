using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : State
{
    public Move(EnemyAI _enemyAI) : base(_enemyAI) {}

    public override IEnumerator Begin()
    {
        Debug.Log("Moving");
        _enemyAI.GoToGoal();
        _enemyAI.GoToGoal();
        _enemyAI.agent.destination = _enemyAI.goal[0].position;
        _enemyAI.goal.RemoveAt(0);
        yield return new WaitForSeconds(1);
        yield return new WaitWhile(_enemyAI.GoalNotReach);
        if(_enemyAI.goal.Count > 0)
        {
            _enemyAI.SetState(new Move(_enemyAI));
        }
    }
}
