using UnityEngine;
using System.Collections;

public class enemy_controller : MonoBehaviour
{
    // Patrolling Variables
    public Transform[] waypoints;
    public float moveSpeed = 5.0f;
    private int currentWaypointIndex = 0;
    private bool goingForward = true;

    // Vision Variables
    public enum VisionState { PATROL, SUS, ALERT, CHASE } // Patrol = normal, Sus = better vision, Alert = stare, Chase = attack
    public VisionState currentVisionState = VisionState.PATROL;
    public Transform visionCone;   // Vision cone object
    public GameObject questionMarkIndicator; // Question mark above enemy head

    public float baseVisionRange = 0.5f;
    public float susVisionRange = 0.75f;
    public float alertVisionRange = 1.0f;

    private float thresholdToSus = 50.0f;
    private float thresholdToAlert = 75.0f;
    private float thresholdToChase = 100.0f;

    private Transform player;
    private bool playerInSight = false;
    private float playerSuspicionCached = 0.0f;

    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 lastMoveDir = Vector2.zero;
    public float stopDistance = 0.5f;

    public int damage = 1;
    public float attackCooldown = 1.0f;

    private float attackTimer = 0.0f;
    private PlayerHealth targetHealth;

    public LayerMask solidObjectsLayer;
    public float attackRange = 0.7f;

    private AudioSource audioSource;
    public AudioClip attackSound;

    public float maxChaseDistance = 10.0f;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(questionMarkIndicator != null)
        {
            questionMarkIndicator.SetActive(false);
        }
        SetConeScale(baseVisionRange);

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        switch(currentVisionState)
        {
            case VisionState.PATROL:
                Patrol();
                break;
            case VisionState.SUS:
                Patrol();
                break;
            case VisionState.ALERT:
                FacePlayer();
                lastMoveDir = Vector2.zero;
                break;
            case VisionState.CHASE:
                moveSpeed = 2.5f;
                ChasePlayer();
                break;
        }
        UpdateAnimator();
        HandleAttack();
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        return Physics2D.OverlapCircle(targetPos, 0.1f, solidObjectsLayer) == null;
    }

    public void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            return;
        }

        Vector3 waypointPos = waypoints[currentWaypointIndex].position;
        Vector3 toTarget = waypointPos - transform.position;
        lastMoveDir = new Vector2(toTarget.x, toTarget.y);

        // candidate next position
        Vector3 nextPos = Vector3.MoveTowards(transform.position, waypointPos, moveSpeed * Time.deltaTime);

        //only move if not hitting a solid tile
        if (IsWalkable(nextPos))
        {
            transform.position = nextPos;
        }
        else
        {
            // hit a wall - stop or flip path
            lastMoveDir = Vector2.zero;
            return;
        }

        if (Vector3.Distance(transform.position, waypointPos) < 0.1f)
        {
            if (goingForward)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypoints.Length)
                {
                    currentWaypointIndex = waypoints.Length - 2;
                    goingForward = false;
                }
            }
            else
            {
                currentWaypointIndex--;
                if (currentWaypointIndex < 0)
                {
                    currentWaypointIndex = 1;
                    goingForward = true;
                }
            }
        }
    }

    public void FacePlayer()
    {
        if(player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            if(direction.x != 0)
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Sign(direction.x) * Mathf.Abs(scale.x);
                transform.localScale = scale;
                if (questionMarkIndicator != null)
                {
                    Vector3 qmScale = questionMarkIndicator.transform.localScale;
                    qmScale.x = Mathf.Abs(qmScale.x);
                    questionMarkIndicator.transform.localScale = qmScale;
                }
            }
            UpdateVisionConeDirection(direction);
        }
    }

    public void ChasePlayer()
    {
        if (player == null)
        {
            return;
        }

        Vector3 currentPos = transform.position;
        Vector3 toTarget = player.position - currentPos;
        float distance = toTarget.magnitude;

        if (distance > maxChaseDistance)
        {
            currentVisionState = VisionState.PATROL;
            playerInSight = false;
            player = null;
            SetConeScale(baseVisionRange);
            if (questionMarkIndicator != null)
            {
                questionMarkIndicator.SetActive(false);
            }
            lastMoveDir = Vector2.zero;
            return;
        }

        if (distance < stopDistance)
        {
            lastMoveDir = Vector2.zero;
            return;
        }

        // Decide primary + secondary direction
        Vector2 dir = toTarget.normalized;
        Vector2 primaryDir;
        Vector2 secondaryDir;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            // try horizontal first, then vertical
            primaryDir   = new Vector2(Mathf.Sign(dir.x), 0f);
            secondaryDir = new Vector2(0f, Mathf.Sign(dir.y));
        }
        else
        {
            // try vertical first, then horizontal
            primaryDir   = new Vector2(0f, Mathf.Sign(dir.y));
            secondaryDir = new Vector2(Mathf.Sign(dir.x), 0f);
        }

        // Try moving in the primary direction
        Vector3 primaryStep = currentPos + (Vector3)(primaryDir * moveSpeed * Time.deltaTime);

        if (IsWalkable(primaryStep))
        {
            lastMoveDir = primaryDir;
            transform.position = primaryStep;
            return;
        }

        // If blocked, try the secondary direction (slide along wall)
        Vector3 secondaryStep = currentPos + (Vector3)(secondaryDir * moveSpeed * Time.deltaTime);

        if (IsWalkable(secondaryStep))
        {
            lastMoveDir = secondaryDir;
            transform.position = secondaryStep;
        }
        else
        {
            // Both directions blocked - stuck for this frame
            lastMoveDir = Vector2.zero;
        }
    }

    public void OnPlayerInSight(Transform playerTransform, float playerSuspicion)
    {
        player = playerTransform;
        playerInSight = true;
        playerSuspicionCached = playerSuspicion;
        UpdateVisionState();
    }

    public void OnPlayerOutOfSight()
    {
        playerInSight = false;
        if(currentVisionState != VisionState.CHASE)
        {
            currentVisionState = VisionState.PATROL;
            SetConeScale(baseVisionRange);
            if(questionMarkIndicator != null)
            {
                questionMarkIndicator.SetActive(false);
            }
        }
    }

    private void UpdateVisionState()
    {
        float suspicion = playerSuspicionCached;
        if(suspicion >= thresholdToChase)
        {
            Debug.Log("Chasing Player");
            currentVisionState = VisionState.CHASE;
            SetConeScale(alertVisionRange);
            if(questionMarkIndicator != null)
            {
                questionMarkIndicator.SetActive(false);
            }
            return;
        }

        if(suspicion >= thresholdToAlert)
        {
            if(playerInSight)
            {
                Debug.Log("Alerted to Player");
                currentVisionState = VisionState.ALERT;
                SetConeScale(alertVisionRange);
                if(questionMarkIndicator != null)
                {
                    questionMarkIndicator.SetActive(true);
                }
            }
            else
            {
                currentVisionState = VisionState.PATROL;
                SetConeScale(baseVisionRange);
            }
            return;
        }

        if(suspicion >= thresholdToSus)
        {
            Debug.Log("Suspicious of Player");
            currentVisionState = VisionState.SUS;
            SetConeScale(susVisionRange);
            if(questionMarkIndicator != null)
            {
                questionMarkIndicator.SetActive(true);
            }
            return;
        }

        currentVisionState = VisionState.PATROL;
        SetConeScale(baseVisionRange);
        if(questionMarkIndicator != null)
        {
            questionMarkIndicator.SetActive(false);
        }
    }

    private void SetConeScale(float range)
    {
        if(visionCone != null)
        {
            Vector3 scale = visionCone.localScale;
            scale.x = range;
            visionCone.localScale = scale;
        }
    }

    private void UpdateAnimator()
    {
        if (animator == null) 
        {
            return;
        }

        bool isMoving = lastMoveDir.sqrMagnitude > 0.001f;
        animator.SetBool("isMoving", isMoving);

        int direction = animator.GetInteger("direction"); // keep last direction if not moving

        if (isMoving)
        {
            Vector2 moveDir = lastMoveDir.normalized;

            if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
            {
                direction = 2; // side
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Sign(moveDir.x) * Mathf.Abs(scale.x);
                transform.localScale = scale;
            }
            else
            {
                direction = (moveDir.y > 0) ? 1 : 0; // back / forward
            }

            animator.SetInteger("direction", direction);
            UpdateVisionConeDirection(moveDir);
        }
    }


    private void UpdateVisionConeDirection(Vector2 moveDir)
    {
        if(visionCone != null)
        {
            if(lastMoveDir.sqrMagnitude < 0.001f)
            {
                return;
            }
           float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            visionCone.rotation = Quaternion.Euler(0, 0, angle + 90);
        }
    }

    private void HandleAttack()
    {
        // Always tick down the timer
        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;

        if (player == null)
            return;

        targetHealth = player.GetComponent<PlayerHealth>();
        float dist = Vector2.Distance(transform.position, player.position);

        bool canAttack = currentVisionState == VisionState.CHASE && dist <= attackRange;

        if (canAttack && attackTimer <= 0f)
        {
            // Trigger animation
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }


            targetHealth.TakeDamage(damage);
            audioSource.PlayOneShot(attackSound);

            // Reset cooldown
            attackTimer = attackCooldown;
        }
    }
}
