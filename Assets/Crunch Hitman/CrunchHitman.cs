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

    private const float waitForAgentDestinationUpdate = 0.1f;

    private Vector2 distToPlayer;
    private bool visible;

    private Transform player;
    private NavMeshAgent agent;
    private List<GameObject> rooms;

    private GameObject currentRoom;
    private List<GameObject> uncheckedRooms;

    // state stuff
    private enum State { headingToRoom, patrollingRoom, chasing, attacking }
    private void ChangeState(State newState) {
        stateDur = 0f;
        prevState = state;
        state = newState;
    }
    private State state, prevState;
    private float stateDur;

    private bool reachedTarget => agent.remainingDistance < 0.1f;

    private void Awake() {

        agent = GetComponent<NavMeshAgent>();
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

            stateDur = 0f;
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

                    while (state == State.headingToRoom) {
                        stateDur += Time.deltaTime;

                        agent.SetDestination(currentRoom.transform.position);

                        if (stateDur > waitForAgentDestinationUpdate && reachedTarget) {
                            ChangeState(State.patrollingRoom);
                            //uncheckedRooms.Remove(currentRoom);
                            //ChangeState(State.headingToRoom);
                        }
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
                        if (newPatrols == 0) ChangeState(State.headingToRoom);
                    }

                    break;

                case State.chasing:

                    while (state == State.chasing) {
                        stateDur += Time.deltaTime;

                        agent.SetDestination(player.position);

                        if (!visible) ChangeState(State.headingToRoom);
                        yield return null;
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

        if (visible) ChangeState(State.chasing);

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
