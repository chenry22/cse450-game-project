using System.Collections;
using UnityEngine;

public class BasketballController : MonoBehaviour {
    private Rigidbody2D rb;
    private CircleCollider2D ballCol;
    private float originalY;
    private bool playerLastHeld = false;

    public float minBounceVelocity = 1f;
    public float bounceDecayRate = 1.7f;
    public float shootTime = 1f;
    public float gravityScale = 0.65f;

    private const float ballToAirPct = 0.08f; // how long into shot does it become un-blockable


    void Start() {
        rb = GetComponent<Rigidbody2D>();
        ballCol = GetComponent<CircleCollider2D>();
        rb.gravityScale = 0f;
    }

    void FixedUpdate() {
        if (transform.position.y < originalY) {
            if (rb.velocity.magnitude >= minBounceVelocity) {
                ballCol.isTrigger = false; // if bouncing, should be hitting now
                transform.position += Vector3.up * 0.05f;
                rb.velocity = rb.velocity / bounceDecayRate;
                rb.angularVelocity = rb.angularVelocity / bounceDecayRate;

                if (Random.Range(0, 1f) > .5f) {
                    rb.angularVelocity *= -1;
                }
                if (Random.Range(0, 1f) > .5f) {
                    rb.velocity *= -1;
                }
            } else {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.gravityScale = 0f;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        if(GetComponent<FixedJoint2D>() != null) { return; } // can't be owned twice
        if (col.gameObject.tag == "Player" || col.gameObject.tag == "Creature") {
            rb.gravityScale = 0f;
            originalY = -100f;
            FixedJoint2D joint = gameObject.AddComponent<FixedJoint2D>();
            joint.connectedBody = col.rigidbody;
            joint.enableCollision = false;
            transform.position = col.transform.position + (Vector3.right * 0.5f);
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.tag != "Player" && col.tag != "Creature") {
            ballCol.isTrigger = false;
        }
    }
    
    public bool GetLastHeld() { return playerLastHeld; }

    public bool ShootBall(Vector2 target) {
        var joint = this.GetComponent<FixedJoint2D>();
        if (joint == null) { return false; } // someone must have control of the ball.

        rb.gravityScale = gravityScale;
        originalY = Mathf.Min(-1.2f, this.transform.position.y);
        this.transform.position = joint.connectedBody.transform.position + new Vector3(0.8f, 1f);
        playerLastHeld = joint.connectedBody.tag == "Player";
        Destroy(joint);
        target += new Vector2(-.65f, -.1f);
        Vector2 diff = target - rb.position;
        float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);

        float vx = diff.x / shootTime;
        float vy = (diff.y + 0.5f * gravity * shootTime * shootTime) / shootTime;
        rb.velocity = new Vector2(vx, vy);

        // consider ball "in air" after some delay, else it can be blocked/stolen
        StartCoroutine(SendBallToAir(shootTime * ballToAirPct));
        return true;
    }
    
    // helper function making ball shooting w other players a little cleaner
    // basically by turning this into a trigger it will ignore collisions of the hoopers
    private IEnumerator SendBallToAir(float delay) {
        yield return new WaitForSeconds(delay);
        if (this.GetComponent<FixedJoint2D>() == null) {
            ballCol.isTrigger = true;
        }
    }
    
    public bool Dribble() {
        var joint = this.GetComponent<FixedJoint2D>();
        if (joint == null) { return false; } // someone must have control of the ball.

        var parent = joint.connectedBody.transform.position;
        if (transform.position.x > parent.x) {
            transform.position = parent - (Vector3.right * 0.5f);
        } else {
            transform.position = parent + (Vector3.right * 0.5f);
        }
        return true;
    }
    
    public bool AttachedTo(GameObject obj) {
        var joint = this.GetComponent<FixedJoint2D>();
        if (joint == null) { return false; } // someone must have control of the ball.
        return joint.connectedBody.gameObject == obj;
    }
}
