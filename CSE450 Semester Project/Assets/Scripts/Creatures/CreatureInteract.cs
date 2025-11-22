using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreatureInteract : MonoBehaviour
{
    private const KeyCode interactKey = KeyCode.E;
    private const KeyCode transferKey = KeyCode.Q; // q to transfer pie, shift+q to transfer control too
    private const KeyCode buddyKey = KeyCode.F;

    private GameObject creature;
    private TMP_Text dialogueBox; // parent should be toggled to show
    private CreatureAutomator automator;
    private SpriteRenderer spr;

    private float creatureSpeechTime = 1.2f;
    private GameObject interacting = null;

    void Start() {
        creature = this.transform.parent.gameObject;
        dialogueBox = creature.transform.GetChild(2).GetComponentInChildren<TMP_Text>();
        automator = creature.GetComponent<CreatureAutomator>();
        spr = this.GetComponent<SpriteRenderer>();
        spr.enabled = false;
    }

    void Update() {
        if (interacting != null) {
            if (Input.GetKeyDown(interactKey)) {
                // interact, allow more in
                StopCoroutine("DialogueInteraction");
                StartCoroutine("DialogueInteraction");
            } else if (Input.GetKeyDown(transferKey)) {
                if (Input.GetKey(KeyCode.LeftShift)) {
                    TransferPieIfPossible();

                    // transfer control
                    var control = GameObject.Find("CreatureHandler")?.GetComponent<CreatureMove>();
                    if (control != null) {
                        creature.GetComponent<CreatureSelect>().SetTextSelected();
                        interacting.GetComponent<CreatureSelect>().SetTextNormal();
                        
                        control.UpdateSelectedCreature(this.creature);
                        automator.StopStationAutomation();
                    }

                    spr.enabled = false;
                    interacting = null;
                } else {
                    var claimed = TransferPieIfPossible();

                    if (automator.assignedStation != null) {
                        automator.ResetStationAutomation();
                    }
                    if (claimed != null) {
                        if (automator.assignedStation != null) {
                            automator.HandlePizza(claimed, null);
                        } else {
                            // TODO: implement singular pizza automation for unassigned creature
                            // automator.BeginPizzaAutomation(claimed);
                        }
                    }
                }
            } else if (Input.GetKeyDown(buddyKey)) {
                // cant buddy w city creatures
                if (creature.GetComponent<CityCreatureBehavior>() != null) { return; }
                var control = GameObject.Find("CreatureHandler").GetComponent<CreatureMove>();
                if (control != null) {
                    control.UpdateBuddyCreature(creature);
                }
            }
        }
    }

    public PizzaObject TransferPieIfPossible() {
        var thisPie = creature.GetComponentInChildren<PizzaObject>();
        var otherPie = interacting.GetComponentInChildren<PizzaObject>();

        if (thisPie == null && otherPie != null) {
            // move to this creature
            otherPie.transform.parent = creature.transform;
            otherPie.transform.localPosition = TossGameManager.pizzaOffset;
            return otherPie;
        } else if (thisPie != null && otherPie == null) {
            // move to player
            thisPie.transform.parent = interacting.transform;
            thisPie.transform.localPosition = TossGameManager.pizzaOffset;
        }
        return null;
    }

    private IEnumerator DialogueInteraction() {
        dialogueBox.text = Dialogue.start[Random.Range(0, Dialogue.start.Length)];
        dialogueBox.transform.parent.gameObject.SetActive(true);
        yield return new WaitForSeconds(creatureSpeechTime);
        dialogueBox.transform.parent.gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other){
        if (other.tag == "Player") {
            if (spr == null) {
                spr = GetComponent<SpriteRenderer>();
            }
            spr.enabled = true;
            interacting = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player" || creature.tag == "Player") {
            if (spr == null) {
                spr = GetComponent<SpriteRenderer>();
            }
            spr.enabled = false;
            interacting = null;
        }
    }
}
