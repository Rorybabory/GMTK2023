using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Door d = collider.gameObject.GetComponent<Door>();
        if (d != null) {
            d.Used(null);
            Destroy(this.gameObject);
        }
        
    }
}
