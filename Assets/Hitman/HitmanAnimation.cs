using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HitmanAnimation : MonoBehaviour {

    [SerializeField] private float walkAnimSpeed, sprintAnimSpeed;
    [SerializeField] private Transform gunTip;

    private Animator animator;
    private Hitman hitman;
    private NavMeshAgent agent;

    private enum Animation { running, idle, aiming, shooting }
    private readonly string[] animationNames = new[] {
        "Hitman_Running",
        "Hitman_Idle",
        "Hitman_Aiming",
        "Hitman_Shooting",
    };

    private Animation currentAnimation;
    private List<Animation> manualAnimations = new() {
        Animation.aiming,
        Animation.shooting,
    };

    private void Awake() {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        hitman = GetComponent<Hitman>();
    }

    private void Play(Animation animation) {
        animator.Play(animationNames[(int)animation]);
    }

    /// <summary> Call this to enter and exit the aiming animation state. </summary>
    /// <param name="aiming"> if true, enters aiming. if false, exits aiming. </param>
    public void AimGun(bool aiming, Vector2 target) {

        var newAnimation = aiming ? Animation.aiming : Animation.idle;
        if (newAnimation != currentAnimation) {
            currentAnimation = newAnimation;
            Play(currentAnimation);
        }

        transform.right = target - (Vector2)(transform.position + gunTip.up * gunTip.localPosition.y);
        Debug.DrawLine(gunTip.position, target, Color.red);
    }

    /// <summary> Plays the shooting animation, then returns to aiming</summary>
    public void Shoot() {
        Play(Animation.shooting);
    }

    private void Update() {

        if (manualAnimations.Contains(currentAnimation)) return;

        float speed = agent.velocity.magnitude; 
        bool moving = speed > 0.1f;

        if (moving && !(agent.velocity.y != 0 && agent.velocity.x != 0)) {
            transform.eulerAngles = Vector3.forward * Mathf.Round(Vector2.SignedAngle(Vector2.right, agent.velocity) * 90f) / 90f;
        }

        Animation newAnimation = moving ? Animation.running : Animation.idle;

        if (newAnimation != currentAnimation) {
            currentAnimation = newAnimation;
            Play(currentAnimation);
        }

        animator.speed = speed >= hitman.SprintSpeed ? sprintAnimSpeed : walkAnimSpeed;
    }
}
