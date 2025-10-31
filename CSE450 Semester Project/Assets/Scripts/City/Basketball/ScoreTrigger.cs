using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTrigger : MonoBehaviour {
    public BasketballGameController gameController;
    private float cooldownTime = 1f;
    private bool cooldown = false;

    private IEnumerator DoCooldown()
    {
        cooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        cooldown = false;
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (cooldown) { return; }

        var ball = col.GetComponent<BasketballController>();
        if (ball && col.transform.position.y > this.transform.position.y) {
            gameController.UpdateScore(1, ball.GetLastHeld());
            StartCoroutine(DoCooldown());
        }
    }
}
