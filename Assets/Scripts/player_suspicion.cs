using UnityEngine;

public class player_suspicion : MonoBehaviour
{
    public float suspicionLevel = 0.0f;
    public float maxSuspicionLevel = 100.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseSuspicion(float amount)
    {
        suspicionLevel += amount;
        if(suspicionLevel > maxSuspicionLevel)
        {
            suspicionLevel = maxSuspicionLevel;
        }
    }

    public void DecreaseSuspicion(float amount)
    {
        suspicionLevel -= amount;
        if(suspicionLevel < 0)
        {
            suspicionLevel = 0;
        }
    }
}
