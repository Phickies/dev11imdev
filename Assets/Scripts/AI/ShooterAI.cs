
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder;

public class ShooterAI : MonoBehaviour
{
    [Header("Refs")]
    public NavMeshAgent agent;
    public Transform player;
    public Transform muzzle;                 // where bullets spawn (empty child in front)
    public Projectile projectilePrefab;      // assign the projectile prefab

    [Header("Ranges")]
    public float sightRange = 18f;        // start chasing if within this
    public float shootRange = 12f;        // start shooting if within this
    public float holdDistance = 8f;         // desired distance to keep from player
    public float giveUpDistance = 22f;       // stop chasing if beyond this
    public float giveUpSeconds = 3f;        // or if not seen for this long

    [Header("Speeds")]
    public float patrolSpeed = 2.0f;
    public float chaseSpeed = 3.5f;
    public float angularSpeed = 360f;

    [Header("Patrol")]
    public float walkPointRange = 10f;

    [Header("Shooting")]
    public float fireCooldown = 0.6f;
    public bool useLineOfSight = true;
    public LayerMask visionBlockers;         // walls, Default… (exclude Player layer)

    enum State { Patrol, Chase, Shoot }
    State state = State.Patrol;

    Vector3 walkPoint;
    bool walkPointSet;
    float lastSeen = -999f;
    float fireTimer = 0f;

    void Awake()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
        agent.updateRotation = true;
        agent.angularSpeed = angularSpeed;
    }

    void Start()
    {
        if (NavMesh.SamplePosition(transform.position, out var h, 5f, NavMesh.AllAreas))
            agent.Warp(h.position);
        EnterPatrol();
    }

    void Update()
    {
        if (!agent || !agent.isOnNavMesh || player == null) return;

        fireTimer -= Time.deltaTime;

        Vector3 toPlayer = player.position - transform.position;
        float dist = toPlayer.magnitude;

        bool inSight = dist <= sightRange;
        bool canShoot = dist <= shootRange;

        if (useLineOfSight && (inSight || canShoot))
        {
            Vector3 from = transform.position + Vector3.up * 1.6f;
            Vector3 to = player.position + Vector3.up * 1.0f;
            if (Physics.Raycast(from, (to - from).normalized, out var hit, sightRange, visionBlockers))
                inSight = canShoot = (hit.transform == player);
        }

        if (inSight) lastSeen = Time.time;

        switch (state)
        {
            case State.Patrol:
                PatrolTick();
                if (inSight) EnterChase();
                break;

            case State.Chase:
                agent.speed = chaseSpeed;

                // move toward holdDistance
                if (dist > holdDistance) agent.SetDestination(player.position);
                else agent.SetDestination(transform.position); // stop when close enough

                Face(player.position);

                bool lost = (Time.time - lastSeen) > giveUpSeconds || dist > giveUpDistance;
                if (lost) EnterPatrol();
                else if (canShoot) EnterShoot();
                break;

            case State.Shoot:
                agent.SetDestination(transform.position); // stop to shoot
                Face(player.position);

                if (canShoot) TryFire();
                else EnterChase(); // player moved out of shoot range

                bool lost2 = (Time.time - lastSeen) > giveUpSeconds || dist > giveUpDistance;
                if (lost2) EnterPatrol();
                break;
        }
    }

    void Face(Vector3 worldPos)
    {
        Vector3 dir = (worldPos - transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) return;
        Quaternion look = Quaternion.LookRotation(dir.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 10f);
    }

    void TryFire()
    {
        if (projectilePrefab == null || muzzle == null) return;
        if (fireTimer > 0f) return;

        Vector3 dir = (player.position + Vector3.up * 1.0f) - muzzle.position;
        var proj = Instantiate(projectilePrefab, muzzle.position, Quaternion.LookRotation(dir));
        proj.Launch(dir);

        fireTimer = fireCooldown;
    }

    // --- States ---
    void EnterPatrol() { state = State.Patrol; agent.speed = patrolSpeed; walkPointSet = false; }
    void EnterChase() { state = State.Chase; agent.speed = chaseSpeed; }
    void EnterShoot() { state = State.Shoot; agent.speed = 0f; }

    void PatrolTick()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
            if ((transform.position - walkPoint).sqrMagnitude < 1f)
                walkPointSet = false;
        }
    }

    void SearchWalkPoint()
    {
        if (walkPointRange <= 0f) return;
        Vector2 r = Random.insideUnitCircle * walkPointRange;
        Vector3 candidate = transform.position + new Vector3(r.x, 0f, r.y);

        if (NavMesh.SamplePosition(candidate, out var hit, 2f, NavMesh.AllAreas))
        {
            var path = new NavMeshPath();
            if (agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
            {
                walkPoint = hit.position;
                walkPointSet = true;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.cyan; Gizmos.DrawWireSphere(transform.position, shootRange);
        Gizmos.color = Color.gray; Gizmos.DrawWireSphere(transform.position, holdDistance);
        Gizmos.color = Color.blue; Gizmos.DrawWireSphere(transform.position, giveUpDistance);
    }
}
