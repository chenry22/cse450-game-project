using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class StartMenu : MonoBehaviour
{
    // TODO: add this plugin
// #if UNITY_WEBGL
//     [DllImport("__Internal")]
//     private static extern void QuitGameWebGl();
// #endif
    public void StartGame()
    {
        SceneManager.LoadScene("MainKitchenScene");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
        Debug.Log("The application quits.");
        // QuitGameWebGl();
#else
        Application.Quit();
#endif
    }
}
