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

    private void UpdateAnimation(Animation newAnimation) {
        if (currentAnimation == newAnimation) return;

        currentAnimation = newAnimation;
        Play(currentAnimation);
    }

    public float AimGun(Vector2 target) {

        UpdateAnimation(Animation.aiming);

        return Vector2.SignedAngle(Vector2.right, target - (Vector2)(transform.position + gunTip.up * gunTip.localPosition.y));
    }

    public void StopAiming() {
        UpdateAnimation(Animation.idle);
    }

    /// <summary> Plays the shooting animation, then returns to aiming</summary>
    public bool Shoot() {
        Play(Animation.shooting);
        var hit = Physics2D.Raycast(gunTip.position, gunTip.right);
        return hit && hit.collider.gameObject.CompareTag("Player");
    }

    private void Update() {

        if (manualAnimations.Contains(currentAnimation)) return;

        float speed = agent.velocity.magnitude; 
        bool moving = speed > 0.1f;

        if (moving && !(agent.velocity.y != 0 && agent.velocity.x != 0)) {
            transform.eulerAngles = Vector3.forward * Mathf.Round(Vector2.SignedAngle(Vector2.right, agent.velocity) * 90f) / 90f;
        }

        var newAnimation = moving ? Animation.running : Animation.idle;
        UpdateAnimation(newAnimation);

        //animator.speed = speed >= hitman.SprintSpeed ? sprintAnimSpeed : walkAnimSpeed;
    }
}
