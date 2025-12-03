using UnityEngine;

public class NPC_Movement : MonoBehaviour
{
    public Transform[] waypoints;
    public float moveSpeed = 2.0f;
    public float arriveThreshold = 0.1f;
    private int currentWaypointIndex = 0;
    private bool goingForward = true;

    public float minIdleTime = 1.0f;
    public float maxIdleTime = 4.0f;
    [Range(0f, 1f)]
    public float idleChance = 0.5f;   // 30% chance to idle at each waypoint
    private bool isIdle = false;
    private float idleTimer = 0.0f;

    public LayerMask solidObjectsLayer;

    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 lastMoveDir = Vector2.zero;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // No waypoints - just stand still
        if (waypoints == null || waypoints.Length == 0)
        {
            lastMoveDir = Vector2.zero;
            UpdateAnimator();
            return;
        }

        if (isIdle)
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0f)
            {
                // done idling, resume patrol
                isIdle = false;
            }
            else
            {
                // stay idle this frame
                lastMoveDir = Vector2.zero;
                UpdateAnimator();
                return;
            }
        }

        Patrol();
        UpdateAnimator();
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        return Physics2D.OverlapCircle(targetPos, 0.1f, solidObjectsLayer) == null;
    }

    private void Patrol()
    {
        if (waypoints.Length == 0) return;

        Vector3 waypointPos = waypoints[currentWaypointIndex].position;
        Vector3 toTarget = waypointPos - transform.position;
        lastMoveDir = new Vector2(toTarget.x, toTarget.y);

        // candidate next position
        Vector3 nextPos = Vector3.MoveTowards(transform.position, waypointPos, moveSpeed * Time.deltaTime);

        // move only if not hitting a solid tile
        if (IsWalkable(nextPos))
        {
            transform.position = nextPos;
        }
        else
        {
            // hit a wall - stop this frame
            lastMoveDir = Vector2.zero;
            return;
        }

        // reached waypoint?
        if (Vector3.Distance(transform.position, waypointPos) < arriveThreshold)
        {
            // roll for idle
            if (Random.value < idleChance)
            {
                isIdle = true;
                idleTimer = Random.Range(minIdleTime, maxIdleTime);
                lastMoveDir = Vector2.zero;
                return;
            }

            // no idle -> advance waypoint (ping-pong like your enemy)
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

    private void UpdateAnimator()
    {
        if (animator == null) return;

        bool isWalking = lastMoveDir.sqrMagnitude > 0.001f && !isIdle;
        animator.SetBool("isWalking", isWalking);

        int direction = animator.GetInteger("direction"); // keep last if not moving

        if (isWalking)
        {
            Vector2 moveDir = lastMoveDir.normalized;

            if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
            {
                // side
                direction = 2;

                // flip sprite left/right
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Sign(moveDir.x) * Mathf.Abs(scale.x);
                transform.localScale = scale;
            }
            else
            {
                // vertical: 0 down, 1 up
                direction = (moveDir.y > 0) ? 1 : 0;
            }

            animator.SetInteger("direction", direction);
        }
    }
}
