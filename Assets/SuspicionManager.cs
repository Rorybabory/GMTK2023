using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SuspicionManager : MonoBehaviour
{
    public Tilemap SearchAreas;

    /// <summary>
    /// Map of the suspicion of each cell, null is an empty cell.
    /// </summary>
    public float?[,] SusMap;

    // Start is called before the first frame update
    void Start()
    {
        SusMap = new float?[SearchAreas.size.x, SearchAreas.size.y];

        Debug.Log($"XSize: {SearchAreas.size.x}, Min Coordinate: {SearchAreas.localBounds.min}");

        

        for (int y = 0; y < SearchAreas.size.y; y++)
        {
            for (int x = 0; x < SearchAreas.size.x; x++)
            {
                if (SearchAreas.GetTile(new(x + (int)SearchAreas.localBounds.min.x, y + (int)SearchAreas.localBounds.min.y)))
                {
                    SusMap[x, y] = 0;
                }
                else
                {
                    SusMap[x, y] = null;
                }
                
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


