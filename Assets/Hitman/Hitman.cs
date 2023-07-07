using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using StateMachine;

public enum HitmanState
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
    public HitmanState state = HitmanState.Chase;

    private NavMeshAgent Agent;

    // Start is called before the first frame update
    void Start()
    {
        Agent = gameObject.GetComponent<NavMeshAgent>();
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case HitmanState.Wander:
                Wander();
                break;
            case HitmanState.Search:
                Search();
                break;
            case HitmanState.Chase:
                Chase();
                break;
            case HitmanState.Stab:
                Stab();
                break;
            case HitmanState.Shoot:
                Shoot();
                break;
            case HitmanState.Snipe:
                Snipe();
                break;
            default:
                break;
        }
    }

    void Wander()
    {
        throw new NotImplementedException();
    }
    
    private void Search()
    {
        throw new NotImplementedException();
    }
    
    private void Chase()
    {
        Agent.SetDestination(Target.position);
    }

    private void Stab()
    {
        throw new NotImplementedException();
    }

    private void Shoot()
    {
        throw new NotImplementedException();
    }

    private void Snipe()
    {
        throw new NotImplementedException();
    }

}
