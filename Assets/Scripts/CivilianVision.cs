using UnityEngine;

public class CivilianVision : MonoBehaviour
{
    public float suspicionIncrease = 10f;
    private bool hasAlreadySpottedPlayer = false;
    private GameObject exclamationMarkIndicator;
    private AudioSource audioSource;
    public AudioClip alertSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        exclamationMarkIndicator = transform.Find("Exclamation").gameObject;
        if(exclamationMarkIndicator != null)
        {
            exclamationMarkIndicator.SetActive(false);
        }
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasAlreadySpottedPlayer)
        {
            player_controller playerController = collision.GetComponent<player_controller>();
            if(playerController != null)
            {
                if(playerController.heldItem != null)
                {
                    player_suspicion ps = collision.GetComponent<player_suspicion>();
                    if(ps != null)
                    {
                        ps.IncreaseSuspicion(suspicionIncrease);
                        hasAlreadySpottedPlayer = true;
                        Debug.Log("Player spotted by civilian, increasing suspicion by " + suspicionIncrease);
                        if(exclamationMarkIndicator != null)
                        {
                            exclamationMarkIndicator.SetActive(true);
                        }
                        if(audioSource != null && alertSound != null)
                        {
                            audioSource.PlayOneShot(alertSound);
                        }
                    }
                }

                if(playerController.heldItem == null)
                {
                    hasAlreadySpottedPlayer = false;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            hasAlreadySpottedPlayer = false;
            if(exclamationMarkIndicator != null)
            {
                exclamationMarkIndicator.SetActive(false);
            }
        }
    }
}
