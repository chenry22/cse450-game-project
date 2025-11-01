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
    void Update()
    {
        if (txt.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
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
