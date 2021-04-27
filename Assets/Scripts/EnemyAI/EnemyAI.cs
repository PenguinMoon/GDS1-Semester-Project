using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : StateMachine
{
    [SerializeField]
    public NavMeshAgent agent;

    [SerializeField]
    public List<Transform> goal;

    // Start is called before the first frame update
    public void Begin()
    {
        agent = GetComponent<NavMeshAgent>();
        SetState(new Start(this));
    }

    public void GoToGoal()
    {
        
        
    }
    private void Update()
    {
        Debug.Log(agent.remainingDistance);
    }

    public void SetGoal(List<Transform> _newGoal)
    {
        goal = _newGoal;
    }

    public bool GoalNotReach() =>agent.remainingDistance > 0.25f;
}
