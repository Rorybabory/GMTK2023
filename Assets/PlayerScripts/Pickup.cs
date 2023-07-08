using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private bool mousepressed = false;
    private GameObject held = null;

    private bool mousereset = false;
    private Vector2 dir = new Vector2(0.0f, 0.0f);
    [SerializeField] private float tossScale = 1.0f;

    private AudioSource source;
    [SerializeField] private AudioClip pickup;
    [SerializeField] private AudioClip drop;
    [SerializeField] private AudioClip toss;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 mpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dir = (new Vector2(mpos.x, mpos.y) - new Vector2(this.transform.position.x, this.transform.position.y));
        mpos.z = 0.0f;
        float dist = Vector3.Distance(this.transform.position, mpos);
        dir.Normalize();
        if (Input.GetMouseButton(1))
        {
            mousepressed = true;
            if (held != null && mousereset == true)
            {
                held.transform.parent = null;
                held.GetComponent<Collider2D>().enabled = true;
                held.transform.GetChild(0).GetComponent<Collider2D>().enabled = true;
                held.transform.position = this.transform.position + new Vector3(dir.x, dir.y, 0.0f);
                held.transform.localScale = held.transform.localScale * 1.25f;
                held.gameObject.GetComponent<Rigidbody2D>().velocity = dir * dist * tossScale;
                
                held = null;
                mousereset = false;
                source.clip = toss;
                source.Play();
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
                held.transform.localScale = held.transform.localScale * 1.25f;
                held = null;
                mousereset = false;
                source.clip = drop;
                source.Play();
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
            Debug.Log("Picking Up");
            other.transform.parent.gameObject.transform.position = new Vector3(0, 0, 0);
            
            
            other.transform.parent.gameObject.transform.SetParent(this.gameObject.transform, false);
            other.enabled = false;
            other.transform.parent.gameObject.GetComponent<Collider2D>().enabled = false;
            held = other.transform.parent.gameObject;
            mousereset = false;
            source.clip = pickup;
            source.Play();
        }
        

    }
}
