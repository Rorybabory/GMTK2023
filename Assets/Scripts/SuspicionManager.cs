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

    [Tooltip("How much above average (in percent) suspicion has to be in order to prioritize it.")]
    public float SignificanceThreshold = 0.2f;

    [Header("Gizmos")]
    public bool DrawSusMap = false;
    public Color GizmoSusColor = Color.black;
    public Color GizmoSigColor = Color.magenta;
    public float GizmoOpacityMultiplier = 0.5f;

    /// <summary>
    /// Map of the suspicion of each cell, null is an empty cell.
    /// </summary>
    public float?[,] SusMap;
    [Tooltip("List of cell coordinates that are above the Significance threshold.")]
    public List<Vector2Int> SigPoints;

    [Header("Readonly")]
    public float Average = 0;

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
        CalculateAverage();
        CalculateSignificantCells();
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
                    //calculate diffuse amount based on the number of neighbors it has
                    int adjacent = 0;
                    if (CheckCell(x, y + 1)) adjacent++;
                    if (CheckCell(x, y - 1)) adjacent++;
                    if (CheckCell(x + 1, y)) adjacent++;
                    if (CheckCell(x - 1, y)) adjacent++;

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

    public void CalculateAverage()
    {
        int count = 0;
        float amount = 0;
        for (int y = SusMap.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < SusMap.GetLength(0); x++)
            {
                if (SusMap[x, y] != null)
                {
                    amount += SusMap[x, y].Value;
                    count++;
                }
            }
        }

        amount /= count;
        Average = amount;
    }

    public void CalculateSignificantCells()
    {
        SigPoints = new();
        for (int y = SusMap.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < SusMap.GetLength(0); x++)
            {
                if (SusMap[x, y] != null)
                {
                    if (SusMap[x, y] > Average * (1 + SignificanceThreshold)) //if the cell is significant
                    {
                        SigPoints.Add(new(x, y));
                    }
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
        var tilemapPos = SearchAreas.WorldToCell(position);
        Vector2Int value = (Vector2Int)tilemapPos - new Vector2Int((int)SearchAreas.localBounds.min.x, (int)SearchAreas.localBounds.min.y); //new Vector2Int((int)Mathf.Round(position.x + SearchAreas.localBounds.extents.x), (int)Mathf.Round(position.y + SearchAreas.localBounds.extents.y - 0.5f)) - (Vector2Int)SearchAreas.origin;

        return value;
    }

    public Vector2 SusMapToWorld(int x, int y)
    {
        return SearchAreas.CellToWorld(new Vector3Int(x, y) + new Vector3Int((int)SearchAreas.localBounds.min.x, (int)SearchAreas.localBounds.min.y));
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
        DrawSusMapGizmo();
    }

    private void DrawSusMapGizmo()
    {
        if (!Application.isPlaying || !DrawSusMap) return;
        for (int y = SusMap.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < SusMap.GetLength(0); x++)
            {
                if (SusMap[x, y] != null)
                {
                    if (SigPoints.Contains(new(x, y)))
                    {
                        Gizmos.color = new(GizmoSigColor.r, GizmoSigColor.g, GizmoSigColor.b, SusMap[x, y].Value * GizmoOpacityMultiplier);
                    }
                    else
                    {
                        Gizmos.color = new(GizmoSusColor.r, GizmoSusColor.g, GizmoSusColor.b, SusMap[x, y].Value * GizmoOpacityMultiplier);
                    }

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
