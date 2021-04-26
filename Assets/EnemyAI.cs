using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : StateMachine
{
    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private Transform goal;

    // Start is called before the first frame update
    void Start()
    {
        agent.destination = goal.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
