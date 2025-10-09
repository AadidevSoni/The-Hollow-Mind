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

    [Header("Stun")]
    public bool isStunned = false;
    public float stunEndTime = 0f;

    [Header("Search/Wander")]
    public float searchDuration = 5f;
    public float searchTurnSpeed = 120f;
    public float wanderRadius = 10f;
    public float wanderInterval = 5f;

    [Header("Search Behavior")]
    public float searchSpeed = 7f;

    [Header("Movement")]
    public float moveSpeed = 3.5f;

    [Header("Rotation")]
    public float rotationSpeed = 5f;

    [Header("Animation")]
    public float speedMultiplier = 1f;

    [Header("Teleport")]
    public float unreachableDuration = 2f;
    public float teleportPause = 2.5f;
    public float teleportOffset = 2f;

    [Header("Flashlight")]
    public Light playerFlashlight;
    public float fleeSpeed = 10f;
    public float fleeMargin = 2f;
    public float fleeUpdateInterval = 0.4f;

    private bool isFleeing = false;
    private float nextFleeUpdateTime = 0f;

    private NavMeshAgent agent;
    private Animator animator;
    private float nextAttackTime = 0f;

    private Vector3 lastKnownPosition = Vector3.zero;
    private float searchEndTime = -999f;
    private bool isSearching = false;
    private float wanderTimer;
    private bool isAttacking = false;
    private bool isTeleporting = false;
    private float unreachableTimer = 0f;
    private float nextSearchPointTime = 0f;

    [Header("Combat")]
    public Collider handHitbox;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        if (eyePoint == null) eyePoint = transform;

        agent.speed = moveSpeed;
        wanderTimer = wanderInterval;
    }

    private void Update()
    {

        bool shouldFlee = false;
        if (playerFlashlight != null && player != null)
        {
            bool flashlightOn = playerFlashlight.enabled;
            float distToPlayer = Vector3.Distance(transform.position, player.position);

            bool inFlashlightCone = false;
            if (flashlightOn)
            {
                Vector3 toDemonFromLight = transform.position - playerFlashlight.transform.position;
                float angle = Vector3.Angle(playerFlashlight.transform.forward, toDemonFromLight);
                float distanceFromLight = toDemonFromLight.magnitude;

                if (angle <= playerFlashlight.spotAngle * 0.5f && distanceFromLight <= playerFlashlight.range)
                {
                    if (Physics.Raycast(playerFlashlight.transform.position, toDemonFromLight.normalized, out RaycastHit hit, distanceFromLight + 0.05f))
                    {
                        if (hit.collider != null && (hit.collider.transform.IsChildOf(transform) || hit.collider.gameObject == this.gameObject))
                            inFlashlightCone = true;
                    }
                }
            }

            shouldFlee = (playerFlashlight.enabled && Vector3.Distance(transform.position, player.position) <= sightRange) || inFlashlightCone;

            if (shouldFlee && !isFleeing) StartFlee();
            else if (!shouldFlee && isFleeing) StopFlee();
        }

        if (isFleeing)
        {
            if (Time.time >= nextFleeUpdateTime && player != null)
            {
                Vector3 awayDir = (transform.position - player.position).normalized;
                float desiredDistance = sightRange + fleeMargin;

                Vector3 fleeTarget = transform.position + awayDir * desiredDistance;

                if (NavMesh.SamplePosition(fleeTarget, out NavMeshHit hit, desiredDistance * 1.2f, NavMesh.AllAreas))
                {
                    agent.isStopped = false;
                    agent.speed = fleeSpeed;
                    agent.SetDestination(hit.position);
                }
                else
                {
                    Vector3 fallback = transform.position + awayDir * Mathf.Max(6f, desiredDistance * 0.6f) + Random.insideUnitSphere * 2f;
                    if (NavMesh.SamplePosition(fallback, out NavMeshHit hit2, 6f, NavMesh.AllAreas))
                    {
                        agent.isStopped = false;
                        agent.speed = fleeSpeed;
                        agent.SetDestination(hit2.position);
                    }
                }

                nextFleeUpdateTime = Time.time + fleeUpdateInterval;
            }

            UpdateAnimatorSpeed(agent.velocity.magnitude);
            return;
        }

        agent.speed = moveSpeed;

        if (isStunned)
        {
            if (Time.time >= stunEndTime) Unstun();
            else
            {
                agent.isStopped = true;
                UpdateAnimatorSpeed(0f);
                return;
            }
        }

        if (player == null) return;

        float distToPlayer2 = Vector3.Distance(transform.position, player.position);
        bool playerDetected = distToPlayer2 <= autoSenseRadius || CanSeePlayer();

        if (!isAttacking && distToPlayer2 <= attackRange && Time.time >= nextAttackTime && playerDetected)
            StartCoroutine(DoAttack());

        if (!isAttacking && !isTeleporting)
        {
            if (playerDetected)
            {
                lastKnownPosition = player.position;
                isSearching = false;
                WanderReset();
                FacePlayerSmooth();

                Vector3 playerGroundPos = new Vector3(player.position.x, transform.position.y, player.position.z);
                NavMeshPath path = new NavMeshPath();
                agent.CalculatePath(playerGroundPos, path);

                bool pathBlocked = path.status != NavMeshPathStatus.PathComplete;
                float yDiff = player.position.y - transform.position.y;
                bool verticalTooHigh = Mathf.Abs(yDiff) > 2f;

                if (pathBlocked || verticalTooHigh)
                {
                    unreachableTimer += Time.deltaTime;
                    if (unreachableTimer >= unreachableDuration && distToPlayer2 <= autoSenseRadius)
                        StartCoroutine(TeleportNearPlayer());
                }
                else
                {
                    unreachableTimer = 0f;
                    if (distToPlayer2 > attackRange)
                    {
                        agent.isStopped = false;
                        agent.SetDestination(player.position);
                    }
                    else agent.isStopped = true;
                }
            }
            else if (lastKnownPosition != Vector3.zero && !isSearching)
            {
                agent.isStopped = false;
                agent.speed = searchSpeed;
                agent.SetDestination(lastKnownPosition);

                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                    StartSearch();
            }
            else if (isSearching)
            {
                agent.speed = searchSpeed;
                GlobalSearch();
            }
            else
            {
                agent.speed = moveSpeed;
                Wander();
            }
        }

        UpdateAnimatorSpeed(agent.velocity.magnitude);
    }

    private void StartFlee()
    {
        isFleeing = true;
        agent.speed = fleeSpeed;
        agent.isStopped = false;
        nextFleeUpdateTime = 0f;

        if (player != null)
        {
            Vector3 awayDir = (transform.position - player.position).normalized;
            float desiredDistance = sightRange + fleeMargin;
            Vector3 fleeTarget = transform.position + awayDir * desiredDistance;

            if (NavMesh.SamplePosition(fleeTarget, out NavMeshHit hit, desiredDistance * 1.2f, NavMesh.AllAreas))
                agent.SetDestination(hit.position);
            else
            {
                Vector3 fallback = transform.position + awayDir * Mathf.Max(6f, desiredDistance * 0.6f) + Random.insideUnitSphere * 2f;
                if (NavMesh.SamplePosition(fallback, out NavMeshHit hit2, 6f, NavMesh.AllAreas))
                    agent.SetDestination(hit2.position);
            }
        }
    }

    private void StopFlee()
    {
        isFleeing = false;
        agent.speed = moveSpeed;
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

    #region Movement
    private void FacePlayerSmooth()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

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
        agent.speed = searchSpeed;
        PickNextSearchPoint();
        agent.isStopped = false;
    }

    private void GlobalSearch()
    {
        if (player == null)
        {
            StopSearch();
            return;
        }

        if (CanSeePlayer() || Vector3.Distance(transform.position, player.position) <= autoSenseRadius)
        {
            StopSearch();
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            PickNextSearchPoint();

        agent.isStopped = false;
    }

    private void PickNextSearchPoint()
    {
        Vector3 randomPoint;
        NavMeshPath path = new NavMeshPath();
        int attempts = 0;

        do
        {
            randomPoint = RandomNavSphere(transform.position, 100f, -1);
            agent.CalculatePath(randomPoint, path);
            attempts++;
            if (attempts > 10) break;
        }
        while (path.status != NavMeshPathStatus.PathComplete);

        agent.SetDestination(randomPoint);
    }

    private void StopSearch()
    {
        isSearching = false;
        agent.speed = moveSpeed;
    }

    private void WanderReset() => wanderTimer = wanderInterval;

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist + origin;
        if (NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask))
            return navHit.position;
        return origin;
    }
    #endregion

    #region Attack
    private IEnumerator DoAttack()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;
        agent.isStopped = true;

        if (player != null) FacePlayerSmooth();

        DemonHandCollider hand = null;
        if (handHitbox != null)
        {
            hand = handHitbox.GetComponent<DemonHandCollider>();
            if (hand != null) hand.ResetDamageFlag();
            handHitbox.enabled = true;
        }

        animator.SetTrigger("Attack");
        yield return null;

        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            if (player != null) FacePlayerSmooth();
            UpdateAnimatorSpeed(0f);
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
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(25);
            }
        }
    }
    #endregion

    #region Teleport
    private IEnumerator TeleportNearPlayer()
    {
        if (isTeleporting) yield break;

        isTeleporting = true;
        agent.isStopped = true;
        UpdateAnimatorSpeed(0f);

        if (animator != null)
        {
            animator.SetTrigger("Teleport");
            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(teleportPause);

        if (player != null)
        {
            Camera cam = Camera.main;
            Vector3 front = cam.transform.forward;
            Vector3 spawnPos = cam.transform.position + front * teleportOffset;
            spawnPos.y = cam.transform.position.y - 1.0f;

            transform.position = spawnPos;
            transform.LookAt(player);
            agent.Warp(spawnPos);
        }

        unreachableTimer = 0f;
        isTeleporting = false;
        agent.isStopped = false;

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

    #region Animator
    private void UpdateAnimatorSpeed(float speed)
    {
        animator.SetFloat("Speed", speed * speedMultiplier);
    }
    #endregion
}
