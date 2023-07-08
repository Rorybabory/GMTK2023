using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using StateMachine;
using GGUtil;

public enum HitmanStates
{
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

    public float TurnLerp = 4;

    [HideInInspector] public NavMeshAgent Agent;

    public bool HasLOS { get; private set; }
    public Vector2 PlayerDir { get; private set; }
    [HideInInspector] public Vector2 LastPlayerPos;

    Vector2 nzAgentVelocity = Vector2.one; //non-zero agent velocity, the 

    // ------------------- State Machine Stuff -------------------- //
    [HideInInspector] public FSM<HitmanStates> SM = new();

    // Start is called before the first frame update
    void Start()
    {
        Agent = gameObject.GetComponent<NavMeshAgent>();
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;

        //initialize the state machine states
        SM.AddState(HitmanStates.Search, new SearchState(this));
        SM.AddState(HitmanStates.Chase, new ChaseState(this));

        SM.SetCurrentState(HitmanStates.Chase);
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
        if(HasLOS) LastPlayerPos = Target.position;

        //This section stores the non-zero velocity
        if (Agent.velocity.sqrMagnitude > 0.01) nzAgentVelocity = Agent.velocity;
        else if (Agent.velocity.sqrMagnitude > 0.005) nzAgentVelocity = Agent.velocity.normalized * 0.01f;
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

    public void LookAtPosition(Vector2 Position)
    {
        Quaternion rot = GGMath.LookRotation2D(transform.position, (Vector2)transform.position - (Position - (Vector2)transform.position).normalized, 90); //desired rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, TurnLerp * Time.deltaTime);
    }

    
    public void LookMovementDir()
    {
        LookAtPosition((Vector2)transform.position + nzAgentVelocity.normalized);
    }
}

public class SearchState : State
{
    protected Hitman hitman;
    
    public SearchState(Hitman hitman)
    {
        this.hitman = hitman;
    }

    public override void Enter()
    {
        Debug.Log("Enter Search State");
        hitman.Agent.isStopped = false;
        hitman.Agent.SetDestination(hitman.LastPlayerPos); //move to the last known position
    }

    public override void Update()
    {
        hitman.LookMovementDir();
        if (hitman.HasLOS) hitman.SM.SetCurrentState(HitmanStates.Chase);
    }
}

public class ChaseState : State
{
    protected Hitman hitman;

    public ChaseState(Hitman hitman)
    {
        this.hitman = hitman;
    }

    public override void Enter()
    {
        hitman.Agent.isStopped = false;
    }

    public override void Update()
    {
        if (!hitman.HasLOS) //if can't see the player then search for him
        {
            hitman.SM.SetCurrentState(HitmanStates.Search);
            return;
        }

        hitman.Agent.SetDestination(hitman.Target.position);
        hitman.LookAtPosition(hitman.Target.position);
    }
}