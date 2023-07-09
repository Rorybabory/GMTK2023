using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hideable : Interact
{
    private bool insideFridge = false;
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
        
        insideFridge = !insideFridge;
        Debug.Log("Inside Fridge: " + insideFridge);
        player.GetComponent<PlayerMechanics>().isHidden = insideFridge;
        player.GetComponent<Renderer>().enabled = !insideFridge;
        player.transform.velocity = new Vector3(0, 0, 0);
    }
}
