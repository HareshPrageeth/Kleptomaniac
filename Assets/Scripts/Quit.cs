using UnityEngine;

public class Quit : MonoBehaviour
{
    public void OnExitButton()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }
}
