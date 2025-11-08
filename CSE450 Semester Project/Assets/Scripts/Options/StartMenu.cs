using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public Toggle skipTutorial;
    private

    void Start() {
        skipTutorial.isOn = PlayerPrefs.GetInt("SkipTutorial", 0) == 1;
        skipTutorial.onValueChanged.AddListener(delegate {
                ToggleTutorial();
            });
    }

    // TODO: add this plugin
    // #if UNITY_WEBGL
    //     [DllImport("__Internal")]
    //     private static extern void QuitGameWebGl();
    // #endif
    public void StartGame() {
        if (PlayerPrefs.GetInt("SkipTutorial", 0) == 1) {
            SceneManager.LoadScene("MainKitchenScene");
        } else {
            SceneManager.LoadScene("TutorialScene");
        }
    }
    
    
    public void ToggleTutorial() {
        if (PlayerPrefs.GetInt("SkipTutorial", 0) == 1) {
            PlayerPrefs.SetInt("SkipTutorial", 0);
        } else {
            PlayerPrefs.SetInt("SkipTutorial", 1);
        }
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
