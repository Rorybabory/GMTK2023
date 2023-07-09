using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CrunchHitman : MonoBehaviour {

    [SerializeField] private float fieldOfViewAngle, maxViewDist, desiredDistToRoomNode, newRoomWaitTime, roomPatrolDist;

    private Transform player;
    private NavMeshAgent agent;
    private List<GameObject> rooms;

    private GameObject currentRoom;
    private List<GameObject> uncheckedRooms;

    private enum State { headingToRoom, patrollingRoom, chasing, attacking }
    private void ChangeState(State newState) {
        stateDur = 0f;
        prevState = state;
        state = newState;
    }
    private State state, prevState;
    private float stateDur;

    private void Awake() {

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        rooms = new(GameObject.FindGameObjectsWithTag("RoomNode"));

        uncheckedRooms = new(rooms);
    }

    private void Update() {

        if (Input.GetKey(KeyCode.Space)) ChangeState(State.headingToRoom);

        Vector2 distToPlayer = player.position - transform.position;
        bool visible =
            Vector2.Angle(transform.right, distToPlayer) < fieldOfViewAngle / 2
            && Physics2D.Raycast(transform.position, distToPlayer, maxViewDist).collider.gameObject.CompareTag("Player");

        if (stateDur == 0) {
            stateDur = 0.0001f;

            switch (state) {

                case State.headingToRoom:

                    if (uncheckedRooms.Count == 0)
                        uncheckedRooms = new(rooms);

                    float dist = Mathf.Infinity;
                    GameObject close = null;
                    foreach (var room in uncheckedRooms) {

                        float newDist = (room.transform.position - transform.position).sqrMagnitude;

                        if (newDist < dist) {
                            close = room;
                            dist = newDist;
                        }
                    }

                    currentRoom = close;

                    break;

                case State.patrollingRoom:
                    uncheckedRooms.Remove(currentRoom);
                    break;

                case State.chasing:
                    break;
            }

            switch (prevState) {

            }
        }

        stateDur += Time.deltaTime;
        switch (state) {

            case State.headingToRoom:
                agent.SetDestination(currentRoom.transform.position);

                if (stateDur > newRoomWaitTime && agent.remainingDistance < desiredDistToRoomNode) {
                    //ChangeState(State.patrollingRoom);
                    uncheckedRooms.Remove(currentRoom);
                    ChangeState(State.headingToRoom);
                }
                else if (visible) ChangeState(State.chasing);
                break;

            case State.patrollingRoom:
                break;

            case State.chasing:
                agent.SetDestination(player.position);

                if (!visible) ChangeState(State.headingToRoom);
                break;
        }

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
