using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DrivingGameExit : MonoBehaviour
{
    public GameObject returnTxt;

    void Update() {
        if (returnTxt.activeSelf && Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("City");
        } 
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Player") {
            returnTxt.SetActive(true);
        }   
    }

    void OnTriggerExit2D(Collider2D collision) {
        if(collision.tag == "Player") {
            returnTxt.SetActive(false);
        }  
    }
}
