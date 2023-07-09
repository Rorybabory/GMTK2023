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

    private float shootTimer;
    private HitmanAnimation animation;
    private bool takenFirstShot;

    public ShootState(Hitman hitman)
    {
        this.hitman = hitman;
    }

    public override void Enter()
    {
        Debug.Log("Enter Shoot State");

        if (animation == null) animation = hitman.GetComponent<HitmanAnimation>();
        shootTimer = 0f;

        hitman.Agent.enabled = false;
        takenFirstShot = false;
    }

    public override void Update()
    {
        PlayerMechanics pm = hitman.Target.gameObject.GetComponent<PlayerMechanics>();

        if (pm != null)
        {
            if (pm.isHidden)
            {
                hitman.SM.SetCurrentState(HitmanStates.Search);
                return;
            }
        }
        bool outOfRange = Vector2.Distance(hitman.transform.position, hitman.Target.transform.position) > hitman.ShootRange,
             outOfSight = !hitman.HasLOS;

        if (outOfRange || outOfSight) {
            hitman.SM.SetCurrentState(HitmanStates.Chase);
            return;
        }

        animation.AimGun(hitman.Target.position);

        shootTimer += Time.deltaTime;

        float shootTime = takenFirstShot ? hitman.FireRate : hitman.FirstShotWaitTime;
        if (shootTimer > shootTime) {
            shootTimer = 0f;
            takenFirstShot = true;

            animation.Shoot();

            // todo: other shooting logic
        }
    }

    public override void Exit()
    {
        hitman.Agent.enabled = true;
        animation.StopAiming();

        Debug.Log("Exit Shoot State");
    }
}
