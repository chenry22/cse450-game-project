using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoughBumperTrigger : MonoBehaviour {
    [Header("UI")]
    public TossGameManager manager;
    private SpriteRenderer spr;
    public Color normalColor = new Color(0, 0, 255, 0.2f);
    public Color triggeredColor = new Color(255, 0, 0, 0.2f);
    public bool left = true; // as in, pointing left
    private bool canBeTossed = false;

    void Start() {
        spr = GetComponent<SpriteRenderer>();
        spr.color = normalColor;
    }

    void Update() {
        if (canBeTossed && ((left && Input.GetAxis("Horizontal") < 0) || (!left && Input.GetAxis("Horizontal") > 0))) {
            canBeTossed = false;
            spr.color = normalColor;
            manager.TossDough();
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (manager.IsGameActive() && collision.tag == "Dough") {
            spr.color = triggeredColor;
            canBeTossed = true;
        }
    }
    void OnTriggerExit2D(Collider2D collision) {
        if (manager.IsGameActive() && canBeTossed && collision.tag == "Dough") {
            Debug.Log("Exit collision (fail)");
            canBeTossed = false;
            spr.color = normalColor;
            manager.DropDough();
        }
    }
}
