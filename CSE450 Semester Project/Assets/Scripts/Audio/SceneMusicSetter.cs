using UnityEngine;

public class SceneMusicSetter : MonoBehaviour
{
    public bool useMainTheme = true;

    void Start()
    {
        if (MusicManager.Instance == null) return;

        if (useMainTheme)
            MusicManager.Instance.PlayTheme(MusicManager.Instance.mainThemeScenes);
        else
            MusicManager.Instance.PlayTheme(MusicManager.Instance.optionsScenes);
    }
}