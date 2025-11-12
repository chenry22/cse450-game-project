using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentController : MonoBehaviour {
    [Header("Gameplay")]
    public Transform leftHoop;
    public Transform rightHoop;
    public BasketballController ball;
    public GameObject player;
    public GameObject opponent;

    [Header("Opponent")]
    public const float actionTickRate = 0.4f;
    public Transform[] targetPositions;
    // TODO: private Transform target = null;

    private float timer = 0f;
    private float opponentSpeed;

    // Start is called before the first frame update
    void Start() {
        player = GameObject.FindWithTag("Player");
        opponent = GameObject.FindWithTag("Creature");
        opponentSpeed = opponent.GetComponent<CreatureStats>().speed;
    }

    void FixedUpdate() {
        if (timer < actionTickRate) {
            timer += Time.deltaTime;
            return;
        }

        timer = 0f;
        if (ball.AttachedTo(player)) {
            // each frame, the opponent should try to be somewhere between the player and the hoop
            // TODO: for now just assume right hoop
            var diff = player.transform.position - rightHoop.position;
            var target = player.transform.position - (Random.Range(0.01f, 0.6f) * diff);
            var dir = target - opponent.transform.position;
            opponent.GetComponent<Rigidbody2D>().velocity = dir.normalized * opponentSpeed;
        } else if (ball.AttachedTo(opponent)) {
            // if open, percentage to shoot ball
            // else move closer to basketb and/or away from player (try to get open)
            ball.ShootBall(rightHoop.transform.position);
        } else if (ball.GetComponent<Rigidbody2D>().velocity.magnitude < 0.05f) {
            // if ball is stopped, move towards it to try and grab it
            var dir = ball.transform.position - opponent.transform.position;
            opponent.GetComponent<Rigidbody2D>().velocity = dir.normalized * opponentSpeed;
        }
    }
}
