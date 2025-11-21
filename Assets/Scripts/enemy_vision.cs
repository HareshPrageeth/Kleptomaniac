using UnityEngine;

public class enemy_vision : MonoBehaviour
{
    public enemy_controller enemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(!other.CompareTag("Player"))
        {
            return;
        }
        Debug.Log("Player in sight");
        player_suspicion ps = other.GetComponent<player_suspicion>();
        if(ps == null)
        {
            Debug.Log("Player suspicion component not found");
            return;
        }
        enemy.OnPlayerInSight(other.transform, ps.suspicionLevel);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(!other.CompareTag("Player"))
        {
            return;
        }
        enemy.OnPlayerOutOfSight();
    }
}
