using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player") {
            GameObject canvas = GameObject.Find("WinScreen");
            canvas.GetComponent<Canvas>().enabled = true;
            Time.timeScale = 0;
            Debug.Log("YOU WINNN!");
        }
        
    }
}
