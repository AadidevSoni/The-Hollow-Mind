using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class DemonAI : MonoBehaviour
{
    [Header("Detection")]
    public float sightRange = 15f;
    public float fov = 120f;
    public float autoSenseRadius = 20f;

    [Header("Combat")]
    public float attackRange = 2f;
    public float attackCooldown = 1.3f;
    public int damage = 20;

    [Header("Refs")]
    public Transform eyePoint;
    public Transform player;
    public Animator animator;

    [Header("Stun")]
    public bool isStunned = false;
    public float stunEndTime = 0f;

    [Header("Search/Wander")]
    public float searchDuration = 5f;
    public float searchTurnSpeed = 120f;
    public float wanderRadius = 10f;
    public float wanderInterval = 5f;

    [Header("Movement")]
    public float moveSpeed = 3.5f;
    public float speedMultiplier = 1f;

    [Header("Jump")]
    public float jumpHeight = 2f;
    public float jumpDuration = 0.7f;

    [Header("Summon")]
    public float summonRadius = 20f;
    public float summonHeight = 3f;
    public float summonDuration = 1f;

    private NavMeshAgent agent;
    private float nextAttackTime = 0f;

    private Vector3 lastKnownPosition = Vector3.zero;
    private float searchEndTime = -999f;
    private bool isSearching = false;
    private float wanderTimer;
    private bool isAttacking = false;
    private bool isJumping = false;
    private bool isSummoned = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (animator == null) Debug.LogError("Animator not found in children!");

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        if (eyePoint == null) eyePoint = transform;

        agent.speed = moveSpeed;
        wanderTimer = wanderInterval;

        // Hide demon initially
        gameObject.SetActive(false);
    }

    private void Update()
    {
        // F key toggles summon/hide
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isSummoned)
            {
                // Enable demon first
                gameObject.SetActive(true);
                isSummoned = true;

                StartCoroutine(SummonDemon());
            }
            else
            {
                HideDemon();
            }
        }

        if (!isSummoned) return; // AI only runs when summoned

        agent.speed = moveSpeed;

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

        float distToPlayer = Vector3.Distance(transform.position, player.position);
        bool playerDetected = distToPlayer <= autoSenseRadius || CanSeePlayer();

        if (!isAttacking && distToPlayer <= attackRange && Time.time >= nextAttackTime && playerDetected)
            StartCoroutine(DoAttack());

        if (!isAttacking && !isJumping)
        {
            if (playerDetected)
            {
                lastKnownPosition = player.position;
                isSearching = false;
                WanderReset();

                Vector3 playerGroundPos = new Vector3(player.position.x, transform.position.y, player.position.z);
                NavMeshPath path = new NavMeshPath();
                agent.CalculatePath(playerGroundPos, path);

                bool pathBlocked = path.status != NavMeshPathStatus.PathComplete;
                float yDiff = player.position.y - transform.position.y;
                bool verticalTooHigh = Mathf.Abs(yDiff) > 2f;
                bool shouldJump = pathBlocked && verticalTooHigh;

                if (shouldJump && !isJumping)
                    StartCoroutine(JumpToPlayer(player.position));
                else
                {
                    agent.isStopped = false;
                    agent.SetDestination(player.position);
                }
            }
            else if (lastKnownPosition != Vector3.zero && !isSearching)
            {
                agent.isStopped = false;
                agent.SetDestination(lastKnownPosition);

                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                    StartSearch();
            }
            else if (isSearching)
            {
                SearchAround();
            }
            else
            {
                Wander();
            }
        }

        if (!isAttacking && !isJumping)
            UpdateAnimatorSpeed();
        else
            animator.SetFloat("Speed", 0f);
    }

    #region Detection
    private bool CanSeePlayer()
    {
        Vector3 toPlayer = player.position - eyePoint.position;
        if (toPlayer.magnitude > sightRange) return false;
        float angle = Vector3.Angle(eyePoint.forward, toPlayer);
        if (angle > fov * 0.5f) return false;

        if (Physics.Raycast(eyePoint.position, toPlayer.normalized, out RaycastHit hit, sightRange))
            return hit.collider.CompareTag("Player");

        return false;
    }
    #endregion

    #region Attack
    private IEnumerator DoAttack()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;

        agent.isStopped = true;
        animator.SetTrigger("Attack");

        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
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
        if (dist <= attackRange + 0.5f)
        {
            var ph = player.GetComponent<PlayerHealth>();
            if (ph != null) ph.TakeDamage(damage);
        }
    }
    #endregion

    #region Jump
    private IEnumerator JumpToPlayer(Vector3 target)
    {
        isJumping = true;
        agent.isStopped = true;

        Vector3 startPos = transform.position;
        Vector3 endPos = target;
        float elapsed = 0f;

        Vector3 dir = (target - transform.position).normalized;
        transform.forward = new Vector3(dir.x, 0f, dir.z);

        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / jumpDuration;
            Vector3 pos = Vector3.Lerp(startPos, endPos, t);
            pos.y += jumpHeight * 4 * t * (1 - t);
            transform.position = pos;
            animator.SetFloat("Speed", 0f);
            yield return null;
        }

        transform.position = endPos;
        agent.Warp(endPos);
        agent.isStopped = false;
        isJumping = false;
    }
    #endregion

    #region Wander/Search
    private void Wander()
    {
        wanderTimer += Time.deltaTime;
        if (wanderTimer >= wanderInterval)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            wanderTimer = 0;
        }
        agent.isStopped = false;
    }

    private void StartSearch()
    {
        isSearching = true;
        searchEndTime = Time.time + searchDuration;
        agent.isStopped = true;
        animator.SetFloat("Speed", 0f);
    }

    private void SearchAround()
    {
        if (Time.time >= searchEndTime)
        {
            isSearching = false;
            lastKnownPosition = Vector3.zero;
            return;
        }
        transform.Rotate(Vector3.up, searchTurnSpeed * Time.deltaTime);
    }

    private void WanderReset() => wanderTimer = wanderInterval;

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist + origin;
        if (NavMesh.SamplePosition(randDirection, out NavMeshHit hit, dist, layermask))
            return hit.position;
        return origin;
    }
    #endregion

    #region Animator
    private void UpdateAnimatorSpeed()
    {
        if (animator == null || agent == null) return;
        animator.SetFloat("Speed", agent.velocity.magnitude * speedMultiplier);
    }
    #endregion

    #region Stun
    public void Stun(float duration)
    {
        isStunned = true;
        stunEndTime = Time.time + duration;
        animator.SetBool("Stunned", true);
        agent.isStopped = true;
    }

    private void Unstun()
    {
        isStunned = false;
        animator.SetBool("Stunned", false);
        agent.isStopped = false;
    }
    #endregion

    #region Summon/Hide
    private IEnumerator SummonDemon()
    {
        // Pick random point around player
        Vector2 circle = Random.insideUnitCircle * summonRadius;
        Vector3 spawnPos = new Vector3(circle.x, 0, circle.y) + player.position;

        if (NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, summonRadius, NavMesh.AllAreas))
            spawnPos = hit.position;

        // Start underground
        transform.position = spawnPos - new Vector3(0, summonHeight, 0);

        // Play summon animation
        animator.SetTrigger("Summon");

        float timer = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = spawnPos;

        while (timer < summonDuration)
        {
            timer += Time.deltaTime;
            float t = timer / summonDuration;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        transform.position = endPos;
        agent.isStopped = false;
    }

    private void HideDemon()
    {
        isSummoned = false;
        gameObject.SetActive(false);
    }
    #endregion
}
