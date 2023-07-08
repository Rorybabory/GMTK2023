using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PheromoneManager : MonoBehaviour
{
    public static PheromoneManager Instance;
    
    public PlayerMechanics Player;

    public List<Pheromone> Pheromones = new();

    public Pheromone FootstepPheromone;
    public Pheromone PlayerTrailPheromone;
    public Pheromone ExitChasePheromones;

    void Awake()
    {
        if (Instance != null) Debug.LogError("Only one pheromone manager is allowed.");
        Instance = this;
    }

    private void FixedUpdate()
    {
        CreatePheromone(Player.transform.position, PlayerTrailPheromone);
    }

    public static Pheromone CreatePheromone(Vector2 Position, float strength, float range, float duration, AnimationCurve falloff = null)
    {
        if (falloff == null) falloff = AnimationCurve.Linear(0, 1, 1, 0);
        var pheromone = Instantiate(new Pheromone(), Position, Quaternion.identity);
        pheromone.m_Strength = strength;
        pheromone.Range = range;
        pheromone.Duration = duration;
        pheromone.Falloff = falloff;

        return pheromone;
    }

    public static Pheromone CreatePheromone(Vector2 position, Pheromone pheromone)
    {
        return Instantiate(pheromone, position, Quaternion.identity);
    }
}
