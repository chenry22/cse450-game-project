using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhyhtmArrowController : MonoBehaviour
{
    private bool active = false;
    public float liveTime = 0.5f;

    private IEnumerator DestroySelf() {
        yield return new WaitForSeconds(liveTime);
        if (active) {
            RhythmGameController.instance.RhyhtmFail();
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.isTrigger) { return; } // ignore other arrows...
        active = true;
        StartCoroutine(DestroySelf());
    }
    void OnTriggerExit2D(Collider2D collision) {
        if (!active) { return; }
        RhythmGameController.instance.RhyhtmFail();
        Destroy(this.gameObject);
    }

    void Update() {
        if (active && Input.GetKeyDown(KeyCode.Space)) {
            RhythmGameController.instance.RhythmScore(this.transform);
            active = false;
            Destroy(this.gameObject);
        }
    }
}
