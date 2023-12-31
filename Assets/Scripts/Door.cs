using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interact
{
    private bool open = false;
    [SerializeField] private bool locked = false;
    [SerializeField] private Transform pivot;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Used(GameObject player)
    {
        if (locked && player != null) { return; }
        
        
        open = !open;
        
        if (open)
        {
            this.transform.RotateAround(pivot.position, Vector3.forward, 270);
        }else
        {
            this.transform.RotateAround(pivot.position, Vector3.forward, -270);
        }
    }
}
