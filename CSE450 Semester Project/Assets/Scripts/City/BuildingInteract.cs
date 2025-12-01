using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInteract : MonoBehaviour
{
    void Start() {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.tag == "Player") {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        if (col.tag == "Player"){
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
