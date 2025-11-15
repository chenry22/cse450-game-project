using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreatureInteract : MonoBehaviour
{
    private const KeyCode interactKey = KeyCode.E;
    private const KeyCode transferKey = KeyCode.Q; // q to transfer pie, shift+q to transfer control too

    private GameObject creature;
    private TMP_Text dialogueBox; // parent should be toggled to show
    private CreatureAutomator automator;
    private SpriteRenderer spr;
    private bool interactable = false;

    void Start() {
        creature = this.transform.parent.gameObject;
        dialogueBox = creature.transform.GetChild(2).GetComponentInChildren<TMP_Text>();
        automator = creature.GetComponent<CreatureAutomator>();
        spr = this.GetComponent<SpriteRenderer>();
        spr.enabled = false;
    }

    void Update() {
        if (interactable) {
            if (Input.GetKeyDown(interactKey)) {
                // interact, allow more in
                StopCoroutine("DialogueInteraction");
                StartCoroutine("DialogueInteraction");
            } else if (Input.GetKeyDown(transferKey)) {
                if (Input.GetKey(KeyCode.RightShift)) {
                    // transfer control + pie
                    spr.enabled = false;
                    interactable = false;
                } else {
                    
                }
            }
        }
    }

    private IEnumerator DialogueInteraction() {
        dialogueBox.text = Dialogue.start[Random.Range(0, Dialogue.start.Length)];
        dialogueBox.transform.parent.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        dialogueBox.transform.parent.gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other){
        if (other.tag == "Player") {
            spr.enabled = true;
            interactable = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player" || creature.tag == "Player") {
            spr.enabled = false;
            interactable = false;
        }
    }
}
