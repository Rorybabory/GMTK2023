using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SuspicionManager : MonoBehaviour
{
    public static SuspicionManager Instance;

    public Tilemap SearchAreas;

    [Tooltip("How much percent of the suspicion each cell should give to it's neighbors every tick.")]
    [Range(0f, 1f)]
    public float diffuse = 0.05f;
    [Tooltip("How much suspicion should each cell lose every tick.")]
    [Range(0, 1)]
    public float evaporation = 0.01f;
    public bool percentageEvaporation = false;

    [Header("Gizmos")]
    public bool DrawSusMap = false;
    public Color GizmoSusColor = Color.black;
    public float GizmoOpacityMultiplier = 0.5f;

    /// <summary>
    /// Map of the suspicion of each cell, null is an empty cell.
    /// </summary>
    public float?[,] SusMap;

    private void Awake()
    {
        if (Instance != null) Debug.LogError("Cannot have multiple Suspicion Managers in the scene.");
        else Instance = this;
    }

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
                    SusMap[x, y] = 0; //initialize suspicion map with no suspicion
                }
                else
                {
                    SusMap[x, y] = null;
                }                
            }
        }
    }

    private void FixedUpdate()
    {
        ComputeTick();
    }

    public void ComputeTick()
    {
        for (int y = SusMap.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < SusMap.GetLength(0); x++)
            {
                if (SusMap[x, y] != null)
                {
                    //step one is to diffuse to neighbors
                    //calculate diffuse amount
                    float amnt = SusMap[x, y].Value * diffuse / 4;

                    void DoDiffuse(int _x, int _y) //diffuse the value to another cell relative to this one.
                    {
                        if (CheckCell(x +_x, y + _y))
                        {
                            SusMap[x + _x, y + _y] += amnt;
                            SusMap[x, y] -= amnt;
                        }
                    }

                    DoDiffuse(1, 0);
                    DoDiffuse(-1, 0);
                    DoDiffuse(0, 1);
                    DoDiffuse(0, -1);
                    
                    //Step two is to evaporate
                    if (percentageEvaporation)
                    {
                        SusMap[x, y] *= 1 - evaporation;
                    }
                    else
                    {
                        SusMap[x, y] -= evaporation;
                    }
                    

                    if (SusMap[x, y] < 0) SusMap[x, y] = 0;
                }
            }
        }
    }

    bool CheckCell(int x, int y)
    {
        if (x < 0 || y < 0) return false;
        if (x >= SusMap.GetLength(0) || y >= SusMap.GetLength(1)) return false;
        if (SusMap[x, y] == null) return false;
        return true;
    }

    public Vector2Int WorldToSusMap(Vector2 position)
    {
        //var tilemapPos = SearchAreas.WorldToCell(position);
        return new Vector2Int((int)Mathf.Round(position.x + SearchAreas.localBounds.extents.x), (int)Mathf.Round(position.y + SearchAreas.localBounds.extents.y - 0.5f));
    }

    public void AddSus(int x, int y, float amount)
    {
        if (!CheckCell(x, y))
        {
            Debug.LogError($"Coordinate is not in bounds of the Suspicion Map. ({x}, {y})");
            return;
        }
        SusMap[x, y] += amount;
    }

    public void AddSusWorld(float x, float y, float amount)
    {
        var coord = WorldToSusMap(new(x, y));
        AddSus(coord.x, coord.y, amount);
    }

    private void OnDrawGizmos()
    {
        DrawGridGizmo();
    }

    private void DrawGridGizmo()
    {
        if (!Application.isPlaying || !DrawSusMap) return;
        for (int y = SusMap.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < SusMap.GetLength(0); x++)
            {
                if (SusMap[x, y] != null)
                {
                    Gizmos.color = new(GizmoSusColor.r, GizmoSusColor.g, GizmoSusColor.b, SusMap[x, y].Value * GizmoOpacityMultiplier);
                    Gizmos.DrawCube(new Vector3(x + 0.5f, y + 0.5f) + SearchAreas.localBounds.min, Vector2.one);
                }
            }
        }
    }
}

/*

// * --- Loop template --- * //

for (int y = SusMap.GetLength(1) - 1; y >= 0; y--)
{
    for (int x = 0; x < SusMap.GetLength(0); x++)
    {
        if (SusMap[x, y] != null)
        {
            
        }
    }
}
 */
