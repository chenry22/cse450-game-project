using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenExit : MonoBehaviour {
    public GameObject txt;

    // Start is called before the first frame update
    void Start()
    {
        txt.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        if (txt.activeSelf && Input.GetKeyDown(KeyCode.E)) {
            string id = GameObject.FindWithTag("Player").GetComponent<CreatureStats>().id;
            foreach (GameObject registered in GameManager.instance.GetRegisteredCreatures()) {
                if (registered.GetComponent<CreatureStats>().id == id) {
                    GameManager.instance.playerCreature = registered;
                }
            }
            GameManager.instance.lastCity = null;
            SceneManager.LoadScene("City");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player") { return; }
        txt.SetActive(true);
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag != "Player") { return; }
        txt.SetActive(false);
    }
}
