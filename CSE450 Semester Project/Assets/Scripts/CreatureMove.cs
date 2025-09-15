using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureMove : MonoBehaviour
{
    public GameObject selectedCreature = null;
    public float speed = 3f; // default speed

    public bool updateSelectedCreature(GameObject selected)
    {
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
            float creatureSpeed = selectedCreature.GetComponent<CreatureStats>().speed;
            float h = creatureSpeed * Input.GetAxis("Horizontal");
            float v = creatureSpeed * Input.GetAxis("Vertical");
            selectedCreature.GetComponent<Rigidbody2D>().velocity = new Vector2(h, v);
            // selectedCreature.GetComponent<Rigidbody2D>().MovePosition(newPos);
        }
    }
}
