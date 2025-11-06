using UnityEngine;
using UnityEngine.AI;

public class SimpleChaseAI : MonoBehaviour
{
    [Header("Refs")]
    public NavMeshAgent agent;
    public Transform player;

    [Header("Ranges")]
    public float sightRange = 12f;     // start chase if inside this
    public float killDistance = 1.2f;  // kill if closer than this
    public float giveUpDistance = 18f; // stop chasing if farther than this
    public float giveUpSeconds = 3f;   // or if not seen for this long

    [Header("Speeds")]
    public float patrolSpeed = 2.0f;
    public float chaseSpeed  = 5.0f;
    public float angularSpeed = 360f;

    [Header("Patrol")]
    public float walkPointRange = 10f;

    // optional: simple LOS (unchecked layers block vision)
    [Header("Line of Sight")]
    public bool useLineOfSight = false;
    public LayerMask visionBlockers = ~0; // set to "Default" & walls, exclude Player layer

    enum State { Patrol, Chase, Search }
    State state = State.Patrol;

    Vector3 walkPoint;
    bool walkPointSet;
    float lastSeenPlayerTime = -999f;

    float searchTime = 3f;
    float searchTimer = 0f;

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
        // snap to navmesh and start patrolling
        if (NavMesh.SamplePosition(transform.position, out var h, 5f, NavMesh.AllAreas))
            agent.Warp(h.position);
        EnterPatrol();
    }

    void Update()
    {
        if (!agent || !agent.isOnNavMesh || player == null) return;

        var toPlayer = player.position - transform.position;
        float dist = toPlayer.magnitude;

        bool canSeePlayer = dist <= sightRange;
        if (useLineOfSight && canSeePlayer)
        {
            // ray from ~eye height to player's center
            Vector3 from = transform.position + Vector3.up * 1.6f;
            Vector3 to   = player.position   + Vector3.up * 1.0f;
            if (Physics.Raycast(from, (to - from).normalized, out var hit, sightRange, visionBlockers))
                canSeePlayer = hit.transform == player;
        }

        if (canSeePlayer) lastSeenPlayerTime = Time.time;

        switch (state)
        {
            case State.Patrol:
                PatrolTick();
                if (canSeePlayer) EnterChase();
                break;

            case State.Chase:
                agent.speed = chaseSpeed;
                agent.SetDestination(player.position);

                if (!canSeePlayer)
                {
                    EnterSearch();
                }


                if (dist <= killDistance)
                {
                    KillPlayer();
                    return;
                }

                bool lostForTooLong = (Time.time - lastSeenPlayerTime) > giveUpSeconds;
                bool tooFar = dist > giveUpDistance;
                if (lostForTooLong || tooFar) EnterPatrol();
                break;

            
            case State.Search:
                SearchTick();
                if (canSeePlayer) EnterChase(); // if found player again
                break;    
        }
    }

    void EnterPatrol()
    {
        state = State.Patrol;
        agent.speed = patrolSpeed;
        walkPointSet = false; // force finding a new point
    }

    void EnterChase()
    {
        state = State.Chase;
        agent.speed = chaseSpeed;
    }
    
    void EnterSearch()
    {
        state = State.Search;
        agent.SetDestination(transform.position); // stop
        searchTimer = searchTime;
    }



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

    void SearchTick()
    {
        // rotate slowly to "look around"
        transform.Rotate(0f, 120f * Time.deltaTime, 0f);

        searchTimer -= Time.deltaTime;
        if (searchTimer <= 0f)
        {
            EnterPatrol();
        }
    }


    void SearchWalkPoint()
    {
        if (walkPointRange <= 0f) return;

        // one cheap try per frame; if it fails we try again next frame
        Vector2 r = Random.insideUnitCircle * walkPointRange;
        Vector3 candidate = transform.position + new Vector3(r.x, 0f, r.y);

        if (NavMesh.SamplePosition(candidate, out var hit, 2f, NavMesh.AllAreas))
        {
            // ensure the agent can actually reach it
            var path = new NavMeshPath();
            if (agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
            {
                walkPoint = hit.position;
                walkPointSet = true;
            }
        }
    }

    void KillPlayer()
    {
        // TODO: replace with your game-over logic
        Debug.Log("Player caught!");
        // Example: disable controller or reload scene
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;    Gizmos.DrawWireSphere(transform.position, killDistance);
        Gizmos.color = Color.cyan;   Gizmos.DrawWireSphere(transform.position, giveUpDistance);
    }
}
