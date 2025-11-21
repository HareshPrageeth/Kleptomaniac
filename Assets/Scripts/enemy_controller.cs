using UnityEngine;

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

    public float baseVisionRange = 0.05f;
    public float susVisionRange = 0.075f;
    public float alertVisionRange = 0.1f;

    private float thresholdToSus = 50.0f;
    private float thresholdToAlert = 75.0f;
    private float thresholdToChase = 100.0f;

    private Transform player;
    private bool playerInSight = false;
    private float playerSuspicionCached = 0.0f;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(questionMarkIndicator != null)
        {
            questionMarkIndicator.SetActive(false);
        }
        SetConeScale(baseVisionRange);
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
                break;
            case VisionState.CHASE:
                ChasePlayer();
                break;
        }
    }

    public void Patrol()
    {
        // Move along the waypoints and reverse back on the path after completing it
        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex].position, moveSpeed * Time.deltaTime);
        if(Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.1f)
        {
            if(goingForward)
            {
                currentWaypointIndex++;
                if(currentWaypointIndex >= waypoints.Length)
                {
                    currentWaypointIndex = waypoints.Length - 2;
                    goingForward = false;
                }
            }
            else
            {
                currentWaypointIndex--;
                if(currentWaypointIndex < 0)
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
            }
        }
    }

    public void ChasePlayer()
    {
        if(player != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            FacePlayer();
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
}
