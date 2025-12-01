using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDriver : MonoBehaviour {
    public Transform[] waypoints;
    public float moveSpeed = 5f;
    public float rotateSpeed = 720f; // degrees per second

    private Rigidbody2D rb;
    private int currentIndex = 0;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        if (waypoints == null || currentIndex >= waypoints.Length || currentIndex < 0) {
            return;
        }
        Vector2 target = waypoints[currentIndex].position;
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, moveSpeed * Time.fixedDeltaTime);
        Vector2 direction = (newPos - rb.position).normalized;
        rb.MovePosition(newPos);

        if (direction.sqrMagnitude > 0.0001f) {
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            float newAngle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, rotateSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(newAngle);
        }

        float distance = Vector2.Distance(rb.position, target);
        if (distance < 0.1f) {
            currentIndex++;
            if (currentIndex >= waypoints.Length) {
                currentIndex = 0; // Loop path
            }
        }
    }
}
