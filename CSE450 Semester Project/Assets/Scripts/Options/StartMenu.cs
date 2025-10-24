using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class StartMenu : MonoBehaviour
{
#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void QuitGameWebGl();
#endif
    public void StartGame()
    {
        SceneManager.LoadScene("MainKitchenScene");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
        QuitGameWebGl();
#else
        Application.Quit();
#endif
    }
}
