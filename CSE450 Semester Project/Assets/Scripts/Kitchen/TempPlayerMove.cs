using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: For real implementation, movement should be disabled for however that works
//   rn this is just a placeholder script but it is being used

public class TempPlayerMove : MonoBehaviour {
    public Rigidbody2D rb;
    public float speed = 3f;
    private bool movementEnabled = true;

    // Update is called once per frame
    void Update() {
        if (movementEnabled) {
            float h = speed * Input.GetAxis("Horizontal");
            float v = speed * Input.GetAxis("Vertical");
            rb.velocity = new Vector2(h, v);
        }
        
        // special command to see pizza state
        if (Input.GetKeyDown(KeyCode.P)){
            var playerPie = this.GetComponentInChildren<PizzaObject>();
            if(playerPie != null) { Debug.Log(playerPie.ToString()); }
        }
    }

    public void ToggleMovement() {
        movementEnabled = !movementEnabled;
        rb.velocity = Vector2.zero;
    }
}
