using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class DemonAI : MonoBehaviour
{
    [Header("Detection")]
    public float sightRange = 15f;
    public float fov = 120f;
    public float hearingRange = 8f;

    [Header("Combat")]
    public float attackRange = 2f;
    public float attackCooldown = 1.3f;
    public int damage = 20;

    [Header("Refs")]
    public Transform eyePoint;
    public LayerMask obstructionMask;
    public Transform player;

    [Header("Stun")]
    public bool isStunned = false;
    public float stunEndTime = 0f;

    [Header("Search")]
    public float searchDuration = 5f;
    public float searchTurnSpeed = 120f;

    [Header("Wander")]
    public float wanderRadius = 10f;
    public float wanderInterval = 5f;

    NavMeshAgent agent;
    Animator animator;
    float nextAttackTime = 0f;

    [HideInInspector] public Vector3 lastHeardPosition;
    [HideInInspector] public float lastHeardTime = -999f;
    public float heardMemoryTime = 3f;

    Vector3 lastKnownPosition = Vector3.zero;
    float searchEndTime = -999f;
    bool isSearching = false;

    float wanderTimer;

    bool isAttacking = false;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
            Debug.LogError("Animator not found!"); // ← finds animator in child

        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        if (eyePoint == null) eyePoint = transform;

        wanderTimer = wanderInterval;
    }

    void Update()
    {
        UpdateAnimatorSpeed();

        if (isStunned)
        {
            if (Time.time >= stunEndTime) Unstun();
            else
            {
                agent.isStopped = true;
                animator.SetFloat("Speed", 0f);
                return;
            }
        }

        if (player == null) return;

        bool canSee = CanSeePlayer();
        bool canHear = (Time.time - lastHeardTime <= heardMemoryTime) &&
                       Vector3.Distance(transform.position, lastHeardPosition) <= hearingRange;

        if (canSee)
        {
            lastKnownPosition = player.position;
            isSearching = false;
            WanderReset();
            EngagePlayer(player.position);
        }
        else if (canHear)
        {
            lastKnownPosition = lastHeardPosition;
            isSearching = false;
            WanderReset();
            agent.isStopped = false;
            agent.SetDestination(lastHeardPosition);
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
        else if (lastKnownPosition != Vector3.zero && !isSearching)
        {
            // head to last known position
            agent.isStopped = false;
            agent.SetDestination(lastKnownPosition);
            animator.SetFloat("Speed", agent.velocity.magnitude);

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                StartSearch();
            }
        }
        else if (isSearching)
        {
            SearchAround();
        }
        else
        {
            Wander(); // <-- NEW
        }

        float dist = Vector3.Distance(transform.position, player.position);
        if (!isAttacking && dist <= attackRange && Time.time >= nextAttackTime && canSee)
        {
            StartCoroutine(DoAttack());
        }
    }

    bool CanSeePlayer()
    {
        Vector3 toPlayer = player.position - eyePoint.position;
        float dist = toPlayer.magnitude;
        if (dist > sightRange) return false;

        float angle = Vector3.Angle(eyePoint.forward, toPlayer);
        if (angle > fov * 0.5f) return false;

        if (Physics.Raycast(eyePoint.position, toPlayer.normalized, out RaycastHit hit, sightRange, ~0, QueryTriggerInteraction.Ignore))
        {
            return hit.collider.CompareTag("Player");
        }
        return false;
    }

    void EngagePlayer(Vector3 targetPos)
    {
        agent.isStopped = false;
        agent.SetDestination(targetPos);
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    IEnumerator DoAttack()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;

        agent.isStopped = true;
        animator.SetTrigger("Attack");

        // Keep updating speed to 0 so it blends nicely during attack
        float attackDuration = 0.8f; // match your animation length
        float timer = 0f;
        while (timer < attackDuration)
        {
            timer += Time.deltaTime;
            animator.SetFloat("Speed", 0f);
            yield return null;
        }

        agent.isStopped = false;
        isAttacking = false;
    }


    public void OnAttackHit()
    {
        if (player == null) return;
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= attackRange + 0.5f && CanSeePlayer())
        {
            var ph = player.GetComponent<PlayerHealth>();
            if (ph != null) ph.TakeDamage(damage);
            else Debug.LogWarning("PlayerHealth not found on Player.");
        }
    }

    public void ReceiveNoise(Vector3 pos, float loudness = 1f)
    {
        lastHeardPosition = pos;
        lastHeardTime = Time.time;
    }

    public void Stun(float duration)
    {
        isStunned = true;
        stunEndTime = Time.time + duration;
        animator.SetBool("Stunned", true);
        agent.isStopped = true;
    }

    void Unstun()
    {
        isStunned = false;
        animator.SetBool("Stunned", false);
        agent.isStopped = false;
    }

    void StartSearch()
    {
        isSearching = true;
        searchEndTime = Time.time + searchDuration;
        agent.isStopped = true;
        animator.SetFloat("Speed", 0f);
    }

    void SearchAround()
    {
        if (Time.time >= searchEndTime)
        {
            isSearching = false;
            lastKnownPosition = Vector3.zero; // forget
            return;
        }
        transform.Rotate(Vector3.up, searchTurnSpeed * Time.deltaTime);
    }

    // ----------------- NEW WANDER -----------------
    void Wander()
    {
        wanderTimer += Time.deltaTime;

        if (wanderTimer >= wanderInterval)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            wanderTimer = 0;
        }

        agent.isStopped = false;
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void WanderReset()
    {
        wanderTimer = wanderInterval; // so it won’t instantly wander after chase/hear
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        if (NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask))
        {
            return navHit.position;
        }
        return origin;
    }

    void UpdateAnimatorSpeed()
    {
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);
    }
    // --------------------------------------------- 

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, hearingRange);
        Vector3 fwd = (eyePoint ? eyePoint.forward : transform.forward);
        Gizmos.color = Color.red;
        Gizmos.DrawRay((eyePoint ? eyePoint.position : transform.position), Quaternion.AngleAxis(fov / 2, Vector3.up) * fwd * sightRange);
        Gizmos.DrawRay((eyePoint ? eyePoint.position : transform.position), Quaternion.AngleAxis(-fov / 2, Vector3.up) * fwd * sightRange);
    }
}
