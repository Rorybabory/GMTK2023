using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMechanics : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        hitman = FindObjectOfType<Hitman>();
        navmesh = hitman.GetComponent<NavMeshAgent>();
        hitmanRb = hitman.GetComponent<Rigidbody2D>();
    }

    //exposed Variables
    private float moveSpeed;
    [SerializeField] private float walkSpeed = 2;
    [SerializeField] private float sprintSpeed = 5;
    [SerializeField] private float pushbackStrength = 10;
    [SerializeField] private float pushbackDuration = 0.7f;
    [SerializeField] private KeyCode PushbackKey;
    [SerializeField] private KeyCode SprintKey;
    [SerializeField] private float pushbackRadius = 2;
    [SerializeField] private float pushbackCooldown = 3;
    
    public PheromoneManager PheromoneManager;
    public Pheromone PlayerTrailPheromone;

    //Variables
    private Rigidbody2D rb;
    private bool bIsSprinting = false;
    private bool bIsHiding = false;
    private Hitman hitman;
    private NavMeshAgent navmesh;
    private Rigidbody2D hitmanRb;
    private Vector2 prevDirection;
    private bool bCanPushBack = true;
    
    void ToggleHide() 
    {
        bIsHiding = !bIsHiding;
    }

    bool ShouldCreatePheromone()
    {
        return !bIsHiding && rb.velocity.magnitude > 0;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        rb.velocity = input * moveSpeed;
        if (input != Vector2.zero)
        {
            prevDirection = input;
        }
        //Sprinting
        SetSprint(Input.GetKey(SprintKey));
        if (Input.GetKeyDown(PushbackKey))
        {
            PushBack();
        }
        
    }


    void SetSprint(bool bStartSprint)
    {
        
        if (bStartSprint)
        {
            moveSpeed = sprintSpeed;
        }
        if (!bStartSprint)
        {
            moveSpeed = walkSpeed;
        }
    }
    private void FixedUpdate()
    {
        PheromoneManager.CreatePheromone(rb.transform.position, PlayerTrailPheromone);
    }

    void PushBack()
    {
        if ((hitmanRb.position - rb.position).magnitude <= pushbackRadius && bCanPushBack) {
            StartCoroutine(PushBackCoRoutine());
            bCanPushBack = false;
            new WaitForSeconds(pushbackCooldown);
        }
    }

    IEnumerator PushBackCoRoutine()
    {
        hitman.enabled = false;
        navmesh.enabled = false;
        hitmanRb.constraints = RigidbodyConstraints2D.FreezeRotation;
        hitmanRb.AddForce(prevDirection * pushbackStrength, ForceMode2D.Impulse);
        yield return new WaitForSeconds(pushbackDuration);
        hitman.enabled = true;
        navmesh.enabled = true;
        hitmanRb.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(pushbackCooldown);
        bCanPushBack = true;

    }
}
