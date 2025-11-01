using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerRoomWallController : MonoBehaviour {
    private GameManager gameManager;

    void Start() {
        gameManager = GameManager.instance;
    }

    void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag == "Player") {
            if (collision.transform.position.x < transform.position.x) {
                gameManager.DeactivateBeginDayButton();
            } else {
                gameManager.ActivateBeginDayButton();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player") {
            gameManager.DeactivateBeginDayButton();
        }
    }
}
