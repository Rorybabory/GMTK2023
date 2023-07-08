using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private bool mousepressed = false;
    private GameObject held = null;
    private 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            mousepressed = true;
        }else
        {
            mousepressed = false;
        }
        if (held != null)
        {
            held.transform.localPosition = new Vector3(0, 0, 0);
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {

        if (mousepressed)
        {
            Debug.Log("Picking Up");
            other.gameObject.transform.position = new Vector3(0, 0, 0);
            
            
            other.gameObject.transform.SetParent(this.gameObject.transform, false);
            other.enabled = false;
            other.gameObject.GetComponent<Collider2D>().enabled = false;
            held = other.gameObject;
        }
        

    }
}
