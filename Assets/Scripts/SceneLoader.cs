using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private FadeScreen fadeScreen;

    public static SceneLoader Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SwitchScene(string scene)
    {
        StartCoroutine(fadeScreen.FadeSwitch(1f, scene));
    }
}
