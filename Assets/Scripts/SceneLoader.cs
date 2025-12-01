using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private FadeScreen fadeScreen;

    public void SwitchScene(string scene)
    {
        StartCoroutine(fadeScreen.FadeOut(1f));
        SceneManager.LoadScene(scene);
    }
}
