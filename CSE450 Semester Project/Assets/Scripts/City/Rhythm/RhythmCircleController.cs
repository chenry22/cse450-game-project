using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmCircleController : MonoBehaviour
{
    public Color triggerColor = Color.red;
    public Color normalColor = Color.white;
    public SpriteRenderer spr;
    private int collisions = 0;

    void Start() {
        spr = this.GetComponent<SpriteRenderer>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && collisions <= 0) {
            RhythmGameController.instance.RhyhtmFail();
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        collisions++;
        spr.color = triggerColor;
    }

    void OnTriggerExit2D(Collider2D collision) {
        collisions--;
        if (collisions <= 0) {
            spr.color = normalColor;
        }
    }
}
