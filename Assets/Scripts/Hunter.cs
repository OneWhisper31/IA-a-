using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AgentStates
{
    Idle,
    Patrol,
    Chase
}


public class Hunter : MonoBehaviour
{
    [SerializeField] float maxSpeed,energy,hunterViewRadius,maxForce,recalculateCooldown, killRadius;
    [SerializeField] Transform[] allWaypoints;
    private FiniteStateMachine _FSM;

    public Vector3 velocity { get { return _FSM.velocity; } }

    void Start()
    {
        _FSM = new FiniteStateMachine(energy,transform, maxSpeed, maxForce);
        _FSM.AddState(AgentStates.Idle, new IdleState(_FSM));
        _FSM.AddState(AgentStates.Patrol, new PatrolState(allWaypoints,_FSM, hunterViewRadius));
        _FSM.AddState(AgentStates.Chase, new ChaseState(_FSM, recalculateCooldown,hunterViewRadius));
        _FSM.ChangeState(AgentStates.Patrol);

    }

    void Update()
    {
        _FSM.Update();

        transform.position += velocity * Time.deltaTime;
        if(_FSM.currentKey!=AgentStates.Idle)
            transform.forward = velocity;

        KillHandler();

        CheckBounds();
    }
    void CheckBounds()//check bounds a teleports to the other side
    {
        transform.position = GameManager.instance.SetObjectBoundPosition(transform.position);
    }
    void KillHandler()//checks if the ship is near a fish
    {
        List<Vector3> fishsPos=new List<Vector3>();
        foreach (Fish fish in GameManager.instance.fishs)
        {
            if (fish == null) continue;
            if ((fish.transform.position - transform.position).magnitude <= killRadius)
            {
                fishsPos.Add(fish.transform.position);
                continue;
            }
        }
        foreach (Vector3 pos in fishsPos)
        {
            GameManager.instance.DestroyFish(pos);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, hunterViewRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, killRadius);
    }
}
