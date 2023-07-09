using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private bool mousepressed = false;
    private GameObject held = null;
    private GameObject light = null;
    private bool mousereset = false;
    private Vector2 dir = new Vector2(0.0f, 0.0f);
    [SerializeField] private float tossScale = 1.0f;

    [SerializeField] private SoundEffect pickup;
    [SerializeField] private SoundEffect drop;
    [SerializeField] private SoundEffect toss;
    // Start is called before the first frame update
    void Start()
    {
        pickup.Init(gameObject);
        drop.Init(gameObject);
        toss.Init(gameObject);
        /*        light = transform.GetChild(0).gameObject;*/
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 mpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        dir = (new Vector2(mpos.x, mpos.y) - new Vector2(this.transform.position.x, this.transform.position.y));
        mpos.z = 0.0f;
        float dist = Vector3.Distance(this.transform.position, mpos);
        dir.Normalize();
/*        light.transform.right = new Vector3(dir.y, -dir.x, 0.0f);*/

        if (Input.GetMouseButton(1))
        {
            mousepressed = true;
            if (held != null && mousereset == true)
            {
                held.transform.parent = null;
                held.GetComponent<Collider2D>().enabled = true;
                held.transform.GetChild(0).GetComponent<Collider2D>().enabled = true;
                held.transform.position = this.transform.position + new Vector3(dir.x, dir.y, 0.0f);
                held.gameObject.GetComponent<Rigidbody2D>().velocity = dir * dist * tossScale;
                
                held = null;
                mousereset = false;
                toss.Play();
            }
        }
        if (Input.GetMouseButton(0))
        {
            mousepressed = true;
            if (held != null && mousereset == true)
            {
                held.transform.parent = null;
                held.GetComponent<Collider2D>().enabled = true;
                held.transform.GetChild(0).GetComponent<Collider2D>().enabled = true;
                held.transform.position = this.transform.position + new Vector3(dir.x, dir.y, 0.0f);
                held = null;
                mousereset = false;
                drop.Play();
            }
        }
        else
        {
            mousereset = true;
            mousepressed = false;
        }
        if (held != null)
        {
            held.transform.localPosition = new Vector3(0, 0, 0);
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {

        if (mousepressed == true && mousereset == true)
        {
            if (other.transform.parent.gameObject.tag == "Pickup")
            {
                Debug.Log("Picking Up");
                other.transform.parent.gameObject.transform.position = new Vector3(0, 0, 0);


                other.transform.parent.gameObject.transform.SetParent(this.gameObject.transform, false);
                other.enabled = false;
                other.transform.parent.gameObject.GetComponent<Collider2D>().enabled = false;
                held = other.transform.parent.gameObject;
                mousereset = false;
                pickup.Play();
            }else if (other.transform.parent.gameObject.tag == "Interactable")
            {
                Interact interact = other.transform.parent.gameObject.GetComponent<Interact>();
                if (interact == null) {
                    Debug.Log("Interact not found\n");
                    return;

                }
                interact.Used(this.gameObject);
                mousereset = false;
            }
            
        }
        

    }
}
