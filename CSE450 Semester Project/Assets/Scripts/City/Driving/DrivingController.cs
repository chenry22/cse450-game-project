using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DrivingController : MonoBehaviour
{
    public enum State {
        Park, Neutral, Drive, Reverse
    }

    private SpriteRenderer spr;
    private Rigidbody2D rb;
    public TMP_Text stateTxt;
    public TMP_Text speedTxt;

    public Sprite leftBlink;
    public Sprite normal;
    public Sprite rightBlink;

    [Header("Car Controls")]
    public float acceleration = 10f;
    public float turnRate = 3.5f;
    public float drift = 0.2f; // 0 as none, 1 as full drift
    public float maxSpeed = 10f;
   
    float accelInput = 0;
    float steerInput = 0;
    float rotationAngle = 0;
    State drive = State.Park;
    bool leftBlinking = false;
    bool rightBlinking = false;

    void Start() {
        spr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        steerInput = Input.GetAxis("Horizontal");
        accelInput = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            drive = State.Park;
            stateTxt.text = "PARK";
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            drive = State.Drive;
            stateTxt.text = "DRIVE";
        } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            drive = State.Reverse;
            stateTxt.text = "REVERSE";
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            StopAllCoroutines();
            spr.sprite = normal;
            rightBlinking = false;
            if (!leftBlinking) {
                leftBlinking = true;
                StartCoroutine("LeftBlink");
            } else {
                leftBlinking = false;
            }
        } else if (Input.GetKeyDown(KeyCode.E)) {
            StopAllCoroutines();
            spr.sprite = normal;
            leftBlinking = false;
            if (!rightBlinking) {
                rightBlinking = true;
                StartCoroutine("RightBlink");
            } else {
                rightBlinking = false;
            }
        }

        speedTxt.text = ((int)(rb.velocity.magnitude * 4.5)) + " MPH";
    }

    public IEnumerator LeftBlink() {
        while(true) {
            spr.sprite = leftBlink;
            yield return new WaitForSeconds(0.5f);
            spr.sprite = normal;
            yield return new WaitForSeconds(.7f);
        }
    }
    public IEnumerator RightBlink() {
        while(true) {
            spr.sprite = rightBlink;
            yield return new WaitForSeconds(0.5f);
            spr.sprite = normal;
            yield return new WaitForSeconds(.7f);
        }
    }

    void FixedUpdate() {
        var minSpeedBeforeTurn = Mathf.Clamp01(rb.velocity.magnitude / 8);
        switch(drive) {
            case State.Park:
                rb.velocity = Vector2.zero;
                break;
            case State.Drive:
                if (Vector2.Angle(transform.up, rb.velocity) > 90) {
                    rb.velocity = Vector2.zero;
                } else if (rb.velocity.magnitude < maxSpeed && accelInput > 0) {
                    rb.AddForce(transform.up * acceleration * accelInput, ForceMode2D.Force);
                } else if (rb.velocity.magnitude > 0 && accelInput < 0) {
                    rb.AddForce(transform.up * acceleration * accelInput * 2.5f, ForceMode2D.Force);
                }

                rotationAngle -= steerInput * turnRate * minSpeedBeforeTurn;
                rb.MoveRotation(rotationAngle);
                break;
            case State.Reverse:
                if (Vector2.Angle(-transform.up, rb.velocity) > 90) {
                    rb.velocity = Vector2.zero;
                } else if (rb.velocity.magnitude < maxSpeed / 2f && accelInput > 0) {
                    rb.AddForce(-transform.up * acceleration * accelInput, ForceMode2D.Force);
                } else if (rb.velocity.magnitude > 0 && accelInput < 0) {
                    rb.AddForce(-transform.up * acceleration * accelInput * 2.5f, ForceMode2D.Force);
                }

                rotationAngle += steerInput * turnRate * minSpeedBeforeTurn;
                rb.MoveRotation(rotationAngle);
                break;
        }

        // eliminate drift
        var forward = transform.up * Vector2.Dot(rb.velocity, transform.up);
        var orthogonal = transform.right * Vector2.Dot(rb.velocity, transform.right);
        rb.velocity = forward + orthogonal * drift;
    }
}
