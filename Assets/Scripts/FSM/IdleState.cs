using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    
    FiniteStateMachine _fsm;

    public IdleState(FiniteStateMachine fsm)
    {
        _fsm = fsm;
    }

    public void OnEnter()
    {//normalize value
        _fsm.energy = 0;
    }

    public void OnUpdate()//restore energy
    {
        _fsm.AddForce(_fsm.CalculateSteering(Vector3.Lerp(-_fsm.velocity,Vector3.zero,0.5f)));
        _fsm.energy += Time.deltaTime*4;

        if (_fsm.energy >= _fsm.maxEnergy)
        {
            _fsm.ChangeState(AgentStates.Patrol);
        }
    }

    public void OnExit()
    {//normalize value
        _fsm.energy = _fsm.maxEnergy;
    }
}
