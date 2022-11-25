using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine
{
    public float maxEnergy, energy;
    float maxForce, maxSpeed;
    public Transform transform;
    Vector3 _velocity; public Vector3 velocity { get { return _velocity; } }

    //FSM

    IState _currentState;
    AgentStates _currentKey; public AgentStates currentKey { get { return _currentKey; } }

    Dictionary<AgentStates, IState> _allStates = new Dictionary<AgentStates, IState>();

    public FiniteStateMachine(float maxEnergy,Transform transform,float maxSpeed,float maxForce)
    {
        this.maxEnergy = maxEnergy;
        energy = maxEnergy;
        this.transform = transform;
        this.maxSpeed = maxSpeed;
        this.maxForce = maxForce;
    }


    public void Update()
    {
        _currentState.OnUpdate();
    }

    public void ChangeState(AgentStates state)
    {
        if (_currentState != null) _currentState.OnExit();

        _currentKey = state;
        _currentState = _allStates[state];
        _currentState.OnEnter();
    }

    public void AddState(AgentStates key, IState value)
    {
        if (!_allStates.ContainsKey(key)) _allStates.Add(key, value);
        else _allStates[key] = value;
    }
    public void AddForce(Vector3 force)
    {

        force.y = 0;
        _velocity = Vector3.ClampMagnitude(_velocity + force, maxSpeed);
    }
    public Vector3 CalculateSteering(Vector3 desired)
    {
        return Vector3.ClampMagnitude((desired.normalized * maxSpeed) - _velocity, maxForce);
    }
}
