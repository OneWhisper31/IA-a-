using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IState
{
    FiniteStateMachine _fsm;

    Fish currentFish;

    float recalculateCooldown,currentCooldown, hunterViewRadius;



    public ChaseState(FiniteStateMachine fsm,float recalculateCooldown,float hunterViewRadius)
    {
        _fsm = fsm;
        this.recalculateCooldown = recalculateCooldown;
        this.hunterViewRadius = hunterViewRadius;
    }

    public void OnEnter()
    {
        currentCooldown=recalculateCooldown;
        RecalculateTarget();
    }

    public void OnUpdate()
    {
        _fsm.energy -= Time.deltaTime;
        currentCooldown -= Time.deltaTime;
        if (_fsm.energy <= 0)//out of energy?
        {
            _fsm.ChangeState(AgentStates.Idle);
            return;
        }
        if (currentCooldown <= 0||currentFish==null)
            RecalculateTarget();
        if(currentFish != null)
            Persuit();
    }

    public void OnExit()
    {
        currentFish = null;
    }

    void Persuit()//chase fishes
    {
        Vector3 futurePos = currentFish.transform.position + currentFish.velocity; //* Time.deltaTime;

        Vector3 desired = futurePos - _fsm.transform.position;

        _fsm.AddForce(_fsm.CalculateSteering(desired));

    }
    void RecalculateTarget()//every x secs change to the closest fish
    {
        currentCooldown = recalculateCooldown;
        int fishCounter = 0;
        currentFish = null;

        foreach (Fish fish in GameManager.instance.fishs)
        {
            if (fish == null) break;
            var fishDesired = fish.transform.position - _fsm.transform.position;
            var currentFishDesired = Vector3.zero;

            if (fishDesired.magnitude <= hunterViewRadius)
            {
                if ((fishDesired.magnitude <= currentFishDesired.magnitude) || currentFish == null)
                {
                    currentFish = fish;
                    currentFishDesired = currentFish.transform.position - _fsm.transform.position;
                    fishCounter++;
                }
            }
        }
        if (fishCounter == 0)
        {
            _fsm.ChangeState(AgentStates.Patrol);
        }
    }
}
