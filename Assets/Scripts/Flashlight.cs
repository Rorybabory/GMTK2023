using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    private GameObject light;
    private GameObject player;
    private UnityEngine.Rendering.Universal.Light2D lightComp;
    // Start is called before the first frame update
    void Start()
    {
        light = transform.GetChild(1).gameObject;
        lightComp = light.GetComponent<UnityEngine.Rendering.Universal.Light2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.parent == null )
        {
            lightComp.intensity = 0.0f;
        }
        else
        {
            lightComp.intensity = 6.5f;
            Vector3 mpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector3 dir = (new Vector2(mpos.x, mpos.y) - new Vector2(this.transform.position.x, this.transform.position.y));
            transform.right = dir;

        }
    }
}
