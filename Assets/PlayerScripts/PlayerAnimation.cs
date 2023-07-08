using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimation : MonoBehaviour {

    [SerializeField] private float walkAnimSpeed, sprintAnimSpeed;
    [SerializeField] private SoundEffect stepSound;

    private Animator animator;
    private Rigidbody2D rb;

    private float walkSpeed = 2, sprintSpeed = 5;

    private enum Animation { running, idle }
    private readonly string[] animationNames = new[] {
        "Player_Running",
        "Player_Idle",
    };

    private Animation currentAnimation;

    public void StepSound() => stepSound.Play();

    private void Awake() {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        stepSound.Init(gameObject);
    }

    private void Update() {

        float speed = rb.velocity.magnitude;
        bool moving = speed > 0.1f;

        if (moving && !(rb.velocity.y != 0 && rb.velocity.x != 0)) {
            transform.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.right, rb.velocity);
        }

        Animation newAnimation = moving ? Animation.running : Animation.idle;

        if (newAnimation != currentAnimation) {
            currentAnimation = newAnimation;
            animator.Play(animationNames[(int)currentAnimation]);
        }

        animator.speed = speed >= sprintSpeed ? sprintAnimSpeed : walkAnimSpeed;
    }
}
