using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureMove : MonoBehaviour
{
    public GameObject selectedCreature = null;
    public float speed = 3f;

    public bool updateSelectedCreature(GameObject selected) {
        if (selectedCreature == null)
        {
            selectedCreature = selected;
            return true;
        }
        else if (selected.GetInstanceID() == selectedCreature.GetInstanceID())
        {
            selectedCreature = null;
            return false;
        }
        else
        {
            selectedCreature.GetComponent<CreatureSelect>().DeselectCreature();
            selectedCreature = selected;
            return true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedCreature != null)
        {
            float h = speed * Input.GetAxis("Horizontal");
            float v = speed * Input.GetAxis("Vertical");
            var newPos = new Vector2(transform.position.x + h, transform.position.y + v);
            selectedCreature.GetComponent<Rigidbody2D>().velocity = new Vector2(h, v);
            // selectedCreature.GetComponent<Rigidbody2D>().MovePosition(newPos);
        }
    }
}
