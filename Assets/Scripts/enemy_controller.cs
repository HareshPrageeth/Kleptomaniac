using UnityEngine;

public class enemy_controller : MonoBehaviour
{
    public Transform[] waypoints;
    public float moveSpeed = 5.0f;
    private int currentWaypointIndex = 0;
    private bool goingForward = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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
}
