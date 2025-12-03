using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenExit : MonoBehaviour {
    public GameObject txt;
    private GameObject colliding = null;

    // Start is called before the first frame update
    void Start() {
        txt.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.instance.GetDay() <= 0 || GameObject.Find(DayManager.dayManagerObjName).GetComponent<DayManager>().DayIsActive()) { return; }
        if (colliding != null && colliding.tag == "Player" && Input.GetKeyDown(KeyCode.E)) {
            string name = GameObject.FindWithTag("Player").GetComponent<CreatureSelect>().GetName();
            string buddyName = GameObject.FindWithTag("Partner")?.GetComponent<CreatureSelect>().GetName() ?? "";
            GameManager.instance.buddyCreature = null;
            
            for(int i = 0; i < GameManager.instance.GetRegisteredCreatures().Count; i++) {
                var registered = GameManager.instance.GetRegisteredCreatures()[i];
                if (registered.GetComponent<CreatureSelect>().GetName() == name) {
                    GameManager.instance.playerCreature = registered;
                }
                if (registered.GetComponent<CreatureSelect>().GetName() == buddyName) {
                    GameManager.instance.buddyCreature = registered;
                }
            }
            GameManager.instance.lastCity = null;
            SceneManager.LoadScene("City");
            colliding = null;
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag != "Player") { return; }
        if (GameManager.instance.GetDay() <= 0 || GameObject.Find(DayManager.dayManagerObjName).GetComponent<DayManager>().DayIsActive()) {
            txt.GetComponent<TMP_Text>().text = "You have work to do.";
        } else {
            txt.GetComponent<TMP_Text>().text = "[E] to exit kitchen";
        }
        colliding = collision.gameObject;
        txt.SetActive(true);
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag != "Player") { return; }
        colliding = null;
        txt.SetActive(false);
    }
}
