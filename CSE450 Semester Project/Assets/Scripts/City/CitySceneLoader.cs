using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CitySceneLoader : MonoBehaviour {
    public TMP_Text instructText;
    public string sceneName; // MainKitchenScene 
    private bool triggered = false;

    public void LoadScene() {
        SceneManager.LoadScene(sceneName);
    }

    void Update() {
        if (triggered && Input.GetKeyDown(KeyCode.E)) {
            LoadScene();
        }
    }

    public void LoadBandRhythmScene() {
        SceneManager.LoadScene("BandRhythmGame");
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player") {
            triggered = true;
            instructText.text = "[E] to enter " + sceneName;
            instructText.gameObject.SetActive(true);
        }
    }
    void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag == "Player") {
            triggered = false;
            instructText.gameObject.SetActive(false);
        }
    }
}
