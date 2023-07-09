using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMechanics : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //hitman = FindObjectOfType<Hitman>();
        //navmesh = hitman.GetComponent<NavMeshAgent>();
        //hitmanRb = hitman.GetComponent<Rigidbody2D>();
        isHidden = false;
        bIsHiding = false;
    }

    //exposed Variables
    private float moveSpeed;
    [SerializeField] private float walkSpeed = 2;
    [SerializeField] private float sneakSpeed = 1;
    [SerializeField] private float sprintSpeed = 5;
    [SerializeField] private float pushbackStrength = 10;
    [SerializeField] private float pushbackDuration = 0.7f;
    [SerializeField] private KeyCode PushbackKey;
    [SerializeField] private KeyCode SprintKey;
    [SerializeField] private KeyCode SneakKey;
    [SerializeField] private float pushbackRadius = 2;
    [SerializeField] private float pushbackCooldown = 3;
    
    public PheromoneManager PheromoneManager;
    public Pheromone PlayerTrailPheromone;

    public bool isHidden = false;

    public float volume = 0.0f;

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
        if (isHidden) { return; }
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        rb.velocity = input * moveSpeed;

        
        volume = Mathf.Clamp(volume, 0.0f, 1.0f);
        

        if (input != Vector2.zero)
        {
            prevDirection = input;
        }
        //Sprinting
        SetKey(Input.GetKey(SprintKey), Input.GetKey(SneakKey));
        if (Input.GetKeyDown(PushbackKey))
        {
            PushBack();
        }
    }

    void SetKey(bool bStartSprint, bool bStartSneak)
    {
        
        if (bStartSprint)
        {
            moveSpeed = sprintSpeed;
            volume = 1.0f;
        }
        else if (bStartSneak)
        {
            moveSpeed = sneakSpeed;
            volume = 0.0f;
        }else
        {
            moveSpeed = walkSpeed;
            volume = 0.5f;
        }
        if (rb.velocity.magnitude == 0.0f)
        {
            volume = 0.0f;
        }
    }

    //private void FixedUpdate()
    //{
    //    if(!isHidden) PheromoneManager.CreatePheromone(rb.transform.position, PlayerTrailPheromone);
    //}

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
