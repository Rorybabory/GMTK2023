using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMechanics : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

 
    private float moveSpeed;
    [SerializeField] private float walkSpeed = 2;
    [SerializeField] private float sprintSpeed = 5;

    //Variables
    private Rigidbody2D rb;
    private bool bIsSprinting = false;
    private bool bIsHiding = false;

    void ToggleHide() 
    {
        bIsHiding = !bIsHiding;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        rb.velocity = input.normalized * moveSpeed;
        //Sprinting
        SetSprint(Input.GetKey(KeyCode.LeftShift));

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
}
