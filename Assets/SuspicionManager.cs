using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SuspicionManager : MonoBehaviour
{
    public Tilemap SearchAreas;

    public float[,] SusMap;

    // Start is called before the first frame update
    void Start()
    {
        SusMap = new float[SearchAreas.size.x, SearchAreas.size.y];

        Debug.Log($"XSize: {SearchAreas.size.x}");
        /*for (int y = 0; y < SearchAreas.size.y; y++)
        {
            SearchAreas.getti
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


