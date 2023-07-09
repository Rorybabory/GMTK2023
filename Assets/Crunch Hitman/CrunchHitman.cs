using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CrunchHitman : MonoBehaviour {

    [Header("Sight")]
    [SerializeField] private float fieldOfViewAngle;
    [SerializeField] private float maxViewDist;

    //[Header("Movement")]

    [Header("Patrolling")]
    [SerializeField] private int pickNewPatrolCount;
    [SerializeField] private float roomPatrolDist, newPatrolWaitTime, patrolSpeed;

    [Header("Attacking")]
    [SerializeField] private float firstShotWaitTime;
    [SerializeField] private float fireRate;

    private Transform player;
    private NavMeshAgent agent;
    private HitmanAnimation anim;

    private const float waitForAgentDestinationUpdate = 0.1f;

    private Vector2 distToPlayer;
    private bool visible;
    private List<GameObject> rooms;

    private GameObject currentRoom;
    private List<GameObject> uncheckedRooms;

    // state stuff
    private enum State { headingToRoom, patrollingRoom, chasing, attacking }
    private void ChangeState(State newState) {
        prevState = state;
        state = newState;
    }
    private State state, prevState;

    private bool reachedTarget => agent.remainingDistance < 0.1f;

    private void Awake() {

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<HitmanAnimation>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        rooms = new(GameObject.FindGameObjectsWithTag("RoomNode"));

        uncheckedRooms = new(rooms);
    }

    private void Start() {
        StartCoroutine(UpdateCoroutine());
    }

    private IEnumerator UpdateCoroutine() {

        while (true) {

            switch (state) {

                case State.headingToRoom:

                    if (uncheckedRooms.Count == 0)
                        uncheckedRooms = new(rooms);

                    // find closest
                    float dist = Mathf.Infinity;
                    GameObject close = null;
                    foreach (var room in uncheckedRooms) {

                        if (room == currentRoom) continue;

                        float newDist = (room.transform.position - transform.position).sqrMagnitude;

                        if (newDist < dist) {
                            close = room;
                            dist = newDist;
                        }
                    }

                    currentRoom = close;

                    float timer = 0;
                    while (state == State.headingToRoom) {
                        timer += Time.deltaTime;

                        agent.SetDestination(currentRoom.transform.position);

                        if (timer > waitForAgentDestinationUpdate && reachedTarget)
                            ChangeState(State.patrollingRoom);
                        yield return null;
                    }

                    break;

                case State.patrollingRoom:

                    uncheckedRooms.Remove(currentRoom);

                    int newPatrols = pickNewPatrolCount;
                    while (state == State.patrollingRoom) {

                        yield return new WaitForSeconds(newPatrolWaitTime);

                        agent.SetDestination((Vector2)currentRoom.transform.position + Random.insideUnitCircle * roomPatrolDist);
                        yield return waitForAgentDestinationUpdate;

                        while (state == State.patrollingRoom) {
                            if (reachedTarget) break;
                            yield return null;
                        }

                        newPatrols--;
                        if (newPatrols == 0) {
                            yield return new WaitForSeconds(newPatrolWaitTime);
                            ChangeState(State.headingToRoom);
                        }
                    }

                    break;

                case State.chasing:

                    while (state == State.chasing) {

                        agent.SetDestination(player.position);

                        if (!visible) ChangeState(State.headingToRoom);
                        yield return null;
                    }

                    break;

                case State.attacking:

                    bool firstShotTaken = false;

                    while (state == State.attacking) {
                        agent.enabled = false;

                        timer = firstShotTaken ? firstShotWaitTime : fireRate;
                        while (state == State.attacking) {
                            timer -= Time.deltaTime;
                            anim.AimGun(player.position);

                            if (timer < 0) break;
                            else if (!visible) ChangeState(State.chasing);
                            yield return null;
                        }

                        if (anim.Shoot()) {
                            print("you died");
                            UIManager.TriggerGameOver();
                        }
                    }

                    break;
            }

            yield return null;
        }
    }

    private void Update() {

        if (Input.GetKey(KeyCode.Space)) ChangeState(State.headingToRoom);

        distToPlayer = player.position - transform.position;
        visible =
            Vector2.Angle(transform.right, distToPlayer) < fieldOfViewAngle / 2
            && Physics2D.Raycast(transform.position, distToPlayer, maxViewDist).collider.gameObject.CompareTag("Player");

        if (visible) {
            StopCoroutine(UpdateCoroutine());
            ChangeState(State.attacking);
            StartCoroutine(UpdateCoroutine());
        }

        if (!(agent.destination.x == Mathf.Infinity || agent.destination.y == Mathf.Infinity))
            transform.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.right, agent.destination - transform.position);
        Debug.DrawLine(transform.position, agent.destination, Color.green);
    }


    private void OnDrawGizmos() {

        Gizmos.color = Color.red;
        float center = transform.eulerAngles.z;

        Vector2 ToVector2(float angle) => new(Mathf.Cos((center + angle) * Mathf.Deg2Rad), Mathf.Sin((center + angle) * Mathf.Deg2Rad));
        Gizmos.DrawRay(transform.position, ToVector2(fieldOfViewAngle / 2) * 100);
        Gizmos.DrawRay(transform.position, ToVector2(fieldOfViewAngle / -2f) * 100);
    }
}
