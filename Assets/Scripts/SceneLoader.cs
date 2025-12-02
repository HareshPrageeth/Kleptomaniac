using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private FadeScreen fadeScreen;

    public void SwitchScene(string scene)
    {
        StartCoroutine(fadeScreen.FadeSwitch(1f, scene));
    }
}
