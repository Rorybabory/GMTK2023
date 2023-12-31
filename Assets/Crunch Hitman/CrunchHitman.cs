using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CrunchHitman : MonoBehaviour {

    [Header("Sight")]
    [SerializeField] private float fieldOfViewAngle;
    [SerializeField] private float maxViewDist;

    [Header("Movement")]
    [SerializeField] private float turnSpeed;
    [SerializeField] private float aimSpeed, chasePlayerTime;

    [Header("Patrolling")]
    [SerializeField] private int pickNewPatrolCount;
    [SerializeField] private float roomPatrolDist, newPatrolWaitTime, patrolSpeed;

    [Header("Attacking")]
    [SerializeField] private float fireRate;
    [SerializeField] private float shootDelay;
    [SerializeField] private SoundEffect shootSound;

    private Transform player;
    private PlayerMechanics playerInfo;
    private NavMeshAgent agent;
    private Collider2D col;
    private HitmanAnimation anim;

    private const float waitForAgentDestinationUpdate = 0.1f;

    private bool visible;
    private List<GameObject> rooms;

    private GameObject currentRoom;
    private List<GameObject> uncheckedRooms;

    private float aimAngle;

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
        col = GetComponent<Collider2D>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerInfo = player.GetComponent<PlayerMechanics>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        rooms = new(GameObject.FindGameObjectsWithTag("RoomNode"));

        uncheckedRooms = new(rooms);
    }

    private void Start() {
        shootSound.Init(gameObject);
        StartCoroutine(UpdateCoroutine());
    }

    private IEnumerator UpdateCoroutine() {

        while (true) {

            agent.enabled = true;

            switch (state) {

                case State.headingToRoom:

                    if (uncheckedRooms.Count == 0)
                        uncheckedRooms = new(rooms);

                    // find closest
                    float dist = Mathf.Infinity;
                    GameObject close = uncheckedRooms.Count == 1 ? uncheckedRooms[0] : null;
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

                        timer = 0;
                        while (state == State.patrollingRoom) {
                            timer += Time.deltaTime;
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

                    timer = 0;
                    while (state == State.chasing) {
                        timer += Time.deltaTime;

                        agent.SetDestination(player.position);

                        if (timer > chasePlayerTime && !visible) ChangeState(State.headingToRoom);
                        yield return null;
                    }

                    break;

                case State.attacking: yield return Attack(); IEnumerator Attack() {

                        agent.enabled = false;

                        while (!anim.Shoot()) {
                            aimAngle = anim.AimGun(player.position);
                            yield return null;
                        }

                        yield return new WaitForSeconds(shootDelay);

                        while (state == State.attacking) {

                            shootSound.Play();
                            if (anim.Shoot()) {
                                UIManager.TriggerGameOver();
                                yield break;
                            }

                            timer = 0;
                            while (timer < fireRate) {
                                timer += Time.deltaTime;
                                aimAngle = anim.AimGun(player.position);

                                if (!visible) {
                                    ChangeState(State.chasing);
                                    yield break;
                                }

                                yield return null;
                            }
                        }

                }   break;
            }

            yield return null;
        }
    }

    private void Update() {

        if (Input.GetKey(KeyCode.Space)) ChangeState(State.headingToRoom);

        Vector2 distToPlayer = player.position - transform.position;

        col.enabled = false;
        visible =
            Vector2.Angle(transform.right, distToPlayer) < Mathf.Lerp(fieldOfViewAngle, 360, playerInfo.volume) / 2
            && Physics2D.Raycast(transform.position, distToPlayer, maxViewDist).collider.gameObject.CompareTag("Player")
            && !playerInfo.isHidden;
        col.enabled = true;

        if (visible && state != State.attacking) {
            StopCoroutine(UpdateCoroutine());
            ChangeState(State.attacking);
            StartCoroutine(UpdateCoroutine());
        }

        if (!(agent.destination.x == Mathf.Infinity || agent.destination.y == Mathf.Infinity) || state == State.attacking) {

            float target = state == State.attacking ? aimAngle : Vector2.SignedAngle(Vector2.right, agent.destination - transform.position),
                  angle = transform.eulerAngles.z,
                  delta = Mathf.DeltaAngle(angle, target),
                  speed = state == State.attacking ? aimSpeed : turnSpeed,
                  vel = Mathf.Sign(delta) * speed * Time.deltaTime;

            if (Mathf.Abs(vel) > Mathf.Abs(delta)) vel = Mathf.Clamp(vel, -delta, delta);

            transform.eulerAngles += Vector3.forward * vel;
        }
        Debug.DrawLine(transform.position, agent.destination, Color.green);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.TryGetComponent(out Door door))
            door.Used(null);
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.TryGetComponent(out Door door))
            door.Used(null);
    }

    private void OnDrawGizmos() {

        Gizmos.color = Color.red;
        float center = transform.eulerAngles.z, viewAngle = Mathf.Lerp(fieldOfViewAngle, 360, playerInfo == null ? 0 : playerInfo.volume);

        Vector2 ToVector2(float angle) => new(Mathf.Cos((center + angle) * Mathf.Deg2Rad), Mathf.Sin((center + angle) * Mathf.Deg2Rad));
        Gizmos.DrawRay(transform.position, ToVector2(viewAngle / 2) * 100);
        Gizmos.DrawRay(transform.position, ToVector2(viewAngle / -2f) * 100);
    }
}
