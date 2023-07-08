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
    public static Hitman Instance;

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
    Vector2 pheromoneAverage = Vector2.zero;

    // ------------------- State Machine Stuff -------------------- //
    [HideInInspector] public FSM<HitmanStates> SM = new();

    private void Awake()
    {
        if (Instance != null) Debug.LogError("Cannot have multiple hitmen in the scene (This is intentional)");
        Instance = this; //this has to be in awake so that it can be used in start by other objects
    }

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

        PheromoneManager.CreatePheromone(Target.transform.position, PheromoneManager.Instance.FootstepPheromone);
        
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

    public void EvaluatePheromoneAverage()
    {
        Vector2 average = Vector2.zero;
        float totalStrength = 0;

        foreach (Pheromone attractor in PheromoneManager.Instance.Pheromones) //get the total amount of pheromone in the scene
        {
            totalStrength += attractor.strength;
        }

        foreach (Pheromone attractor in PheromoneManager.Instance.Pheromones)
        {
            average += (Vector2) attractor.transform.position * attractor.strength / totalStrength;
        }

        pheromoneAverage = average;
    }

    private void OnDrawGizmos()
    {
        EvaluatePheromoneAverage();
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(pheromoneAverage, 0.5f);
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
        //hitman.Agent.SetDestination(hitman.LastPlayerPos); //move to the last known position
    }

    public override void Update()
    {
        hitman.EvaluatePheromoneAverage();
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

    public override void Exit()
    {
        PheromoneManager.CreatePheromone(hitman.Target.position, PheromoneManager.Instance.ExitChasePheromones); //create strong pheromones where the player last was
    }
}