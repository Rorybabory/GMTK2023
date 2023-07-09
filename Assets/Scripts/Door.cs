using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interact
{
    private bool open = false;
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
        open = !open;
        
        if (open)
        {
            this.transform.position += new Vector3(-0.5f, 1f, 0);
            this.transform.Rotate(new Vector3(0, 0, 270));
        }else
        {
            this.transform.position -= new Vector3(-0.5f, 1f, 0);
            this.transform.Rotate(new Vector3(0,0,-270));
        }
    }
}
