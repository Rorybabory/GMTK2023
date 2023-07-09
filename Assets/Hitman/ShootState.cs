using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

/// <summary>
/// Hitman Shoot State
/// </summary>
public class ShootState : State
{
    protected Hitman hitman;

    public ShootState(Hitman hitman)
    {
        this.hitman = hitman;
    }

    public override void Enter()
    {
        Debug.Log("Enter Shoot State");
    }

    public override void Update()
    {

    }

    public override void Exit()
    {
        Debug.Log("Exit Shoot State");
    }
}
