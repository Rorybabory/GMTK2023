using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pheromone : MonoBehaviour
{
    [Tooltip("How strong should the pheromones attract the hitman. (The \"loudness\")")]
    public float m_Strength = 10;
    [Min(0)] public float Range = 15;
    [Tooltip("Strength of the fadeout effect over distance")]
    public AnimationCurve Falloff = AnimationCurve.Linear(0, 1, 1, 0);
    [Min(0)] public float Duration = 1;

    public LayerMask BlockingMask;
    public float ObscurityDampening = 0.5f;

    public bool Suspicion = true;
    public float SuspicionAmount = 5;

    [Header("Read-Only")]
    [Tooltip("The strength that the hitman actually reads.")]
    public float strength;

    private float fadeSpeed;

    // Start is called before the first frame update
    void Start()
    {
        fadeSpeed = m_Strength / Duration;
        PheromoneManager.Instance.Pheromones.Add(this);
        if(Suspicion) SuspicionManager.Instance.AddSusWorld(transform.position.x, transform.position.y, SuspicionAmount); //add suspicion impulse
    }

    // Using fixed update because it should have consistent deltaTime
    void FixedUpdate()
    {
        if (Duration <= 0)
        {
            Destroy(gameObject);
        }

        m_Strength -= fadeSpeed * Time.fixedDeltaTime;
        strength = m_Strength;
        strength *= Falloff.Evaluate(Vector2.Distance(transform.position, Hitman.Instance.transform.position) / Range); //evaluate the actual strength based on distance

        RaycastHit2D hit = Physics2D.Linecast(transform.position, Hitman.Instance.transform.position, BlockingMask);
        if (hit.transform != null) strength *= ObscurityDampening; //lower the intensity of the pheromones if they are behind a wall

        Duration -= Time.fixedDeltaTime;
    }

    private void Update()
    {
        
    }

    private void OnDestroy()
    {
        PheromoneManager.Instance.Pheromones.Remove(this);
    }
}
