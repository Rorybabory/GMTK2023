using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using StateMachine;
using GGUtil;

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

    public float TurnLerp = 0.1f;

    [HideInInspector] public NavMeshAgent Agent;

    public bool HasLOS { get; private set; }
    public Vector2 PlayerDir { get; private set; }

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
        CalculateStuff();
        
        SM.Update();
    }

    void CalculateStuff()
    {
        PlayerDir = (Target.position - transform.position).normalized;
        HasLOS = CheckLOS();
    }


    /// <summary>
    /// Check if the hitman can see the player
    /// </summary>
    /// <returns></returns>
    public bool CheckLOS()
    {        
        if (Vector2.Angle(transform.up, PlayerDir) <= FOV / 2)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, PlayerDir, ViewDistance, SightMask);
            if (hit.transform == Target) return true;
        }

        return false;
    }

    public void LookAtPlayer()
    {
        Quaternion rot = GGMath.LookRotation2D(transform.position, (Vector2)transform.position - PlayerDir, 90); //desired rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, TurnLerp * Time.deltaTime);
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
        if (hitman.HasLOS)
        {
            hitman.Agent.isStopped = false;
            hitman.Agent.SetDestination(hitman.Target.position);
            hitman.LookAtPlayer();
        }
        else
        {
            Debug.Log("Lost Sight");
            hitman.Agent.isStopped = true;
        }
    }
}
