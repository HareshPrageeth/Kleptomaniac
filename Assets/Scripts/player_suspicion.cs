using UnityEngine;
using UnityEngine.UI;

public class player_suspicion : MonoBehaviour
{
    public float suspicionLevel = 0.0f;
    public float maxSuspicionLevel = 100.0f;
    public Slider suspicionSlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        suspicionSlider.maxValue = maxSuspicionLevel;
        suspicionSlider.value = suspicionLevel;
    }

    public void IncreaseSuspicion(float amount)
    {
        suspicionLevel += amount;
        if(suspicionLevel > maxSuspicionLevel)
        {
            suspicionLevel = maxSuspicionLevel;
        }
        suspicionSlider.value = suspicionLevel;
    }

    public void DecreaseSuspicion(float amount)
    {
        suspicionLevel -= amount;
        if(suspicionLevel < 0)
        {
            suspicionLevel = 0;
        }
        suspicionSlider.value = suspicionLevel;
    }
}
