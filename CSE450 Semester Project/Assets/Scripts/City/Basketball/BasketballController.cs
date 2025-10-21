using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketballController : MonoBehaviour {
    private Rigidbody2D rb;
    private float originalY;

    public float minBounceVelocity = 1f;
    public float bounceDecayRate = 1.7f;
    public float shootTime = 1f;


    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        if (transform.position.y < originalY) {
            if (rb.velocity.magnitude >= minBounceVelocity) {
                transform.position += Vector3.up * 0.05f;
                rb.velocity = -rb.velocity / bounceDecayRate;
                rb.angularVelocity = -rb.angularVelocity / bounceDecayRate;
            } else {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "Player") {
            originalY = -100f;
            FixedJoint2D joint = gameObject.AddComponent<FixedJoint2D>();
            joint.connectedBody = col.rigidbody;
            joint.enableCollision = false;
            transform.position = col.transform.position + (Vector3.right * 0.5f);
        }
    }
    
    public void ShootBall(Vector2 target) {
        var joint = this.GetComponent<FixedJoint2D>();
        if (joint == null) { return; } // someone must have control of the ball.

        originalY = Mathf.Min(-1.2f, this.transform.position.y);
        this.transform.position = joint.connectedBody.transform.position + new Vector3(0.8f, 1f);
        Destroy(joint);
        target += new Vector2(-0.6f, -0.1f);
        Vector2 diff = target - rb.position;
        float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);

        float vx = diff.x / shootTime;
        float vy = (diff.y + 0.5f * gravity * shootTime * shootTime) / shootTime;
        rb.velocity = new Vector2(vx, vy);
    }
    
    public bool AttachedTo(GameObject obj) {
        var joint = this.GetComponent<FixedJoint2D>();
        if (joint == null) { return false; } // someone must have control of the ball.
        return joint.connectedBody.gameObject == obj;
    }
}
