using UnityEngine;

public class PauseController : MonoBehaviour
{
    public static bool gameIsPaused { get; private set; } = false;
    public GameObject pausePanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        Debug.Log("I made it here");
        gameIsPaused = !gameIsPaused;

        if (gameIsPaused)
        {
            // Pause the game
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
        }
        else
        {
            // Resume normal time
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
        } 

    }




    
}
