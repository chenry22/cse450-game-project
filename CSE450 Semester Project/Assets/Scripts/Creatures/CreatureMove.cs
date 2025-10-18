using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// creature movement *when controlled by player*
// Only one of these should exist, manages whole game/creature ecosystem

public class CreatureMove : MonoBehaviour {
    private const string playerTag = "Player"; // this is necessary for station interaction
    private const string creatureTag = "Creature";

    public GameObject selectedCreature = null;
    public bool movementEnabled = true;
    public float speed = 3f; // default speed
    private Camera mainCam;

    // this is where the camera should always be
    private Vector3 baseCamPosition = new Vector3(0, 0, -10);
    private Vector3 camVelocity = Vector3.zero;
    private float camMoveTime = 0.2f;

    private void Start() {
        mainCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update() {
        if (selectedCreature != null && movementEnabled) {
            float creatureSpeed = selectedCreature.GetComponent<CreatureStats>().speed;
            float h = creatureSpeed * Input.GetAxis("Horizontal");
            float v = creatureSpeed * Input.GetAxis("Vertical");
            selectedCreature.GetComponent<Rigidbody2D>().velocity = new Vector2(h, v);
        }

        if (mainCam.transform.localPosition != baseCamPosition)
        {
            // smooth transition to new creature assignment 
            mainCam.transform.localPosition = Vector3.SmoothDamp(mainCam.transform.localPosition, baseCamPosition, ref camVelocity, camMoveTime);
        }
    }

    public bool UpdateSelectedCreature(GameObject selected) {
        if (selected.GetInstanceID() == selectedCreature.GetInstanceID()) {
            return true;
        } else if (selectedCreature == null) {
            selectedCreature = selected;
            selected.gameObject.tag = playerTag;
            mainCam.transform.parent = selected.transform;
            GameObject.Find("CreatureHandler").GetComponent<CreatureAssign>().UnassignCreature(selected.GetComponent<CreatureSelect>());
            return true;
        } else {
            // SWAP CREATURE SELECTION
            selectedCreature.GetComponent<CreatureSelect>().DeselectCreature();
            selectedCreature.gameObject.tag = creatureTag; // reset so only one active player in scene...

            selectedCreature = selected;
            selected.gameObject.tag = playerTag;
            mainCam.transform.parent = selected.transform;
            GameObject.Find("CreatureHandler").GetComponent<CreatureAssign>().UnassignCreature(selected.GetComponent<CreatureSelect>());

            // Reset multiple interaction blocker 
            // (any active interactions should be ended since we are taking control of a new creature)
            StationInteract.interacting = null;
            return true;
        }
    }

    public bool IsSelectedCreature(GameObject creature) {
        return selectedCreature == creature;
    }

    public void ToggleMovement() {
        selectedCreature.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        movementEnabled = !movementEnabled;
    }
}
