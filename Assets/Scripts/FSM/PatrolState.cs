using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IState
{
    private int _currentWaypoint = 0;
    float hunterViewRadius;
    private FiniteStateMachine _fsm;

    Transform[] allWaypoints;

    public PatrolState(Transform[] allWaypoints, FiniteStateMachine fsm,float hunterViewRadius)
    {
        _fsm = fsm;
        this.allWaypoints = allWaypoints;
        this.hunterViewRadius = hunterViewRadius;

    }

    public void OnEnter()
    {
        Patrol();
    }

    public void OnUpdate()
    {
        _fsm.energy -= Time.deltaTime;
        if (_fsm.energy<=0)
        {
            _fsm.ChangeState(AgentStates.Idle);
            return;
        }
        foreach (Fish fish in GameManager.instance.fishs)
        {
            if (fish == null) break;
            if ((fish.transform.position - _fsm.transform.position).magnitude <= hunterViewRadius)
            {
                _fsm.ChangeState(AgentStates.Chase);
                return;
            }
        }

        Patrol();
        
    }

    void Patrol()
    {
        Transform nextWaypoint = allWaypoints[_currentWaypoint];
        Vector3 dir = nextWaypoint.position - _fsm.transform.position;

        _fsm.AddForce(_fsm.CalculateSteering(dir));

        if (dir.magnitude <= hunterViewRadius/2)
        {
            _currentWaypoint++;
            if (_currentWaypoint > allWaypoints.Length - 1)
            {
                _currentWaypoint = 0;
            }
        }

        _fsm.AddForce(_fsm.CalculateSteering(dir));
    }

    public void OnExit()
    {
    }
}
