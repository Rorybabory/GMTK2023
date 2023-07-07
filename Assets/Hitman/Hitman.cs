using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using StateMachine;

public enum HitmanStates
{
    Wander,
    Search,
    Chase,
    Stab,
    Shoot,
    Snipe
}


[RequireComponent(typeof(NavMeshAgent))]
public class Hitman : MonoBehaviour
{
    public Transform Target;
    public float FOV = 180;
    public float ViewDistance = 25f;
    public LayerMask SightMask;

    [HideInInspector] public NavMeshAgent Agent;

    // ------------------- State Machine Stuff -------------------- //
    private FSM SM = new();

    // Start is called before the first frame update
    void Start()
    {
        Agent = gameObject.GetComponent<NavMeshAgent>();
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;

        //initialize the state machine states
        SM.AddState((int) HitmanStates.Chase, new ChaseState(this));

        SM.SetCurrentState((int) HitmanStates.Chase);
    }

    void Update()
    {
        SM.Update();
    }

    /// <summary>
    /// Check if the hitman can see the player
    /// </summary>
    /// <returns></returns>
    public bool CheckLOS()
    {
        var dir = (Target.position - transform.position).normalized;        
        
        if (Vector2.Angle(transform.up, dir) <= FOV / 2)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, ViewDistance, SightMask);
            if (hit.transform == Target) return true;
        }

        return false;
    }
}

public class ChaseState : State
{
    protected Hitman hitman;

    public ChaseState(Hitman hitman)
    {
        this.hitman = hitman;
    }

    public override void Update()
    {
        if (hitman.CheckLOS())
        {
            hitman.Agent.isStopped = false;
            hitman.Agent.SetDestination(hitman.Target.position);
            
        }
        else
        {
            Debug.Log("Lost Sight");
            //hitman.Agent.SetDestination(hitman.transform.position);
            hitman.Agent.isStopped = true;
        }
    }
}
