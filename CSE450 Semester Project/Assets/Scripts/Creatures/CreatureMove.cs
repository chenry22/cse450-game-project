using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// creature movement *when controlled by player*
// Only one of these should exist, manages whole game/creature ecosystem

public class CreatureMove : MonoBehaviour {
    private const string playerTag = "Player"; // this is necessary for station interaction
    private const string creatureTag = "Creature";
    private const string buddyTag = "Partner";

    public GameObject selectedCreature = null;
    public bool movementEnabled = true;
    private Camera mainCam;

    // this is where the camera should always be
    private Vector3 baseCamPosition = new Vector3(0, 0, -10);
    private Vector3 camVelocity = Vector3.zero;
    private float camMoveTime = 0.2f;

    public GameObject staminaUI;
    public RectTransform staminaFill;

    // buddy system
    public GameObject buddyCreature = null;
    public float buddyCloseEnoughDist = 2.2f; // TODO: decide on right range for this...

    private void Start() {
        mainCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update() {
        if (selectedCreature != null && selectedCreature.GetComponent<CreatureStats>() != null) {
            var stats = selectedCreature.GetComponent<CreatureStats>();
            if (movementEnabled) {
                float creatureSpeed = stats.speed;
                float h = creatureSpeed * Input.GetAxis("Horizontal");
                float v = creatureSpeed * Input.GetAxis("Vertical");
                selectedCreature.GetComponent<Rigidbody2D>().velocity = new Vector2(h, v);
            }
            
            if (mainCam.transform.localPosition != baseCamPosition) {
                // smooth transition to new creature assignment 
                mainCam.transform.localPosition = Vector3.SmoothDamp(mainCam.transform.localPosition, baseCamPosition, ref camVelocity, camMoveTime);
            }

            if (staminaUI != null && staminaFill != null){
                float pct = stats.stamina / stats.maxStamina;
                pct = Mathf.Clamp01(pct);

                staminaFill.localScale = new Vector3(pct, 1f, 1f);
                staminaUI.SetActive(pct >= 1f);
            }
        } else {
            if (staminaUI != null) staminaUI.SetActive(false);
            // if (lowStaminaUI != null) lowStaminaUI.SetActive(false);
        }
    }

    void FixedUpdate() {
        if (selectedCreature != null && buddyCreature != null) {
            var targetPos = selectedCreature.transform.position;
            var dir = targetPos - buddyCreature.transform.position;
            var rb = buddyCreature.GetComponent<Rigidbody2D>();

            if (dir.magnitude > buddyCloseEnoughDist) {
                float speed = buddyCreature.GetComponent<CreatureStats>().speed;
                rb.velocity = dir.normalized * speed;
            }
        }
    }

    public bool UpdateSelectedCreature(GameObject selected) {
        if (selected.GetInstanceID() == selectedCreature.GetInstanceID()) {
            return true;
        } else if (selectedCreature == null) {
            selectedCreature = selected;
            selected.tag = playerTag;
            if (mainCam != null) {
                mainCam.transform.parent = selected.transform;
            } else {
                GameObject.FindWithTag("MainCamera").transform.SetParent(selected.transform);
            }

            var creatureAssign = GameObject.Find("CreatureHandler");
            if (creatureAssign == null) {
                Debug.LogWarning("Creature Handler is null. (this is ok if not in kitchen scene)");
            } else {
                creatureAssign.GetComponent<CreatureAssign>().UnassignCreature(selected.GetComponent<CreatureSelect>());
            }
            movementEnabled = true;
            return true;
        } else {
            // SWAP CREATURE SELECTION
            selectedCreature.GetComponent<CreatureSelect>().DeselectCreature();
            if (buddyCreature != null && selected.GetInstanceID() == buddyCreature.GetInstanceID()) {
                // if swapping to buddy, keep buddy selection
                UpdateBuddyCreature(selectedCreature);
            } else {
                // reset so only one active player in scene...
                selectedCreature.tag = creatureTag;
            }

            selectedCreature = selected;
            selected.tag = playerTag;
            mainCam.transform.parent = selected.transform;
            GameObject.Find("CreatureHandler").GetComponent<CreatureAssign>().UnassignCreature(selected.GetComponent<CreatureSelect>());

            // Reset multiple interaction blocker 
            // (any active interactions should be ended since we are taking control of a new creature)
            StationInteract.interacting = null;
            movementEnabled = true;
            return true;
        }
    }

    public bool UpdateBuddyCreature(GameObject buddy) {
        if (buddyCreature != null && buddy == buddyCreature) {
            // unset 
            buddy.tag = creatureTag;
            buddy.GetComponent<CreatureSelect>().SetTextNormal();
            buddyCreature = null;
            return true;
        } else {
            if (buddyCreature != null) {
                buddyCreature.tag = creatureTag;
            }
            buddy.tag = buddyTag;
            buddyCreature = buddy;
            buddy.GetComponent<CreatureSelect>().SetTextBuddy();
            return true;
        }
    }
    public void UnassignBuddyCreature() {
        if (buddyCreature != null) {
            buddyCreature.tag = creatureTag;
            buddyCreature = null;
        }
    }

    public bool IsSelectedCreature(GameObject creature) {
        return selectedCreature == creature;
    }

    public void ToggleMovement() {
        if (selectedCreature == null) { return; }
        selectedCreature.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        movementEnabled = !movementEnabled;
    }
}
