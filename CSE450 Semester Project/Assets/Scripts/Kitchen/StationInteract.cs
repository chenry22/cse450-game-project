using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// This should be attached to some trigger collider
// for this to work, currently the GameObject that triggers the collider has to have the tag "Player"

public enum Station {
    Toss, Top, Ovens, Cut, Table
}


public class StationInteract : MonoBehaviour {
    // janky solution, basically prevents multiple simultaneous interactions
    public static GameObject interacting = null;

    public Station station = Station.Table; // default to table
    public GameObject stationGame;
    public TMP_Text helpText;
    public Color defaultColor = new Color(255, 255, 255, 0.1f);
    public Color triggeredColor = new Color(255, 0, 0, 0.2f);

    private SpriteRenderer sr; // for changing color to show interaction
    private bool interactable = false;

    void Start() {
        sr = this.GetComponent<SpriteRenderer>();
        stationGame.SetActive(false);
        helpText.gameObject.SetActive(false);
        SetHelpText();
    }

    // handle keyboard input to initialize games
    void Update() {
        if (interactable) {
            switch (station) {
                case Station.Toss:
                    if (Input.GetKeyDown(KeyCode.E)) {
                        helpText.gameObject.SetActive(false);
                        if (stationGame != null) {
                            stationGame.SetActive(true);
                            stationGame.GetComponent<TossGameManager>().BeginTossGame();
                            interactable = false;
                        }
                    }
                    break;
                case Station.Top:
                    if (Input.GetKeyDown(KeyCode.E)) {
                        helpText.gameObject.SetActive(false);
                        Debug.Log("[E] Top trigger!");
                        interactable = false;
                    }
                    break;
                case Station.Ovens:
                    if (Input.GetKeyDown(KeyCode.E)) {
                        helpText.gameObject.SetActive(false);
                        Debug.Log("[E] Oven trigger!");
                        interactable = false;
                    }
                    break;
                case Station.Cut:
                    if (Input.GetKeyDown(KeyCode.E)) {
                        helpText.gameObject.SetActive(false);
                        Debug.Log("[E] Cut trigger!");
                        interactable = false;
                    }
                    break;
                case Station.Table:
                    if (Input.GetKeyDown(KeyCode.Q)) {
                        // pizza is sibling of this gameobject
                        var tablePie = this.transform.parent.GetComponentInChildren<PizzaObject>();
                        if (tablePie == null) {
                            var playerPie = GameObject.FindWithTag("Player").GetComponentInChildren<PizzaObject>().transform;
                            playerPie.parent = this.gameObject.transform.parent;
                            playerPie.localPosition = Vector2.zero;
                        } else {
                            tablePie.transform.parent = GameObject.FindWithTag("Player").transform;
                            tablePie.transform.localPosition = new Vector2(1f, 0);
                        }
                    }
                    break;
            }
        }
    }

    private void SetHelpText() {
        switch (station) {
            case Station.Toss:
                helpText.text = "Press [E] to begin tossing";
                break;
            case Station.Top:
                helpText.text = "Press [E] to begin topping";
                break;
            case Station.Ovens:
                helpText.text = "Press [E] to view oven";
                break;
            case Station.Cut:
                helpText.text = "Press [E] to begin cutting";
                break;
            case Station.Table:
                helpText.text = "Press [Q] to place/pickup pie";
                break;
        }
    }
    private void StartInteraction()
    {
        interactable = true;
        sr.color = triggeredColor;
    }

    void OnTriggerEnter2D(Collider2D c) {
        if (c.gameObject.tag == "Player" && interacting == null) {
            var currPie = c.gameObject.GetComponentInChildren<PizzaObject>();
            switch (station) {
                case Station.Toss:
                    interacting = this.gameObject;
                    // player cannot already have a pizza object
                    if (currPie != null) {
                        helpText.text = "You can't do this while holding something";
                    } else {
                        StartInteraction();
                    }
                    break;
                case Station.Top:
                    interacting = this.gameObject;
                    // player must be holding a pizza object
                    if (currPie == null) {
                        helpText.text = "You must be holding a pizza to do this";
                    }
                    else if (currPie.IsTopped()) {
                        helpText.text = "This pizza is already topped";
                    } else {
                        StartInteraction();
                    }
                    break;
                case Station.Ovens:
                    interacting = this.gameObject;
                    // this just opens the oven view, so it should always be allowed
                    sr.color = triggeredColor;
                    break;
                case Station.Cut:
                    interacting = this.gameObject;
                    if (currPie == null) {
                        helpText.text = "You must be holding a pizza to do this";
                    } else if (currPie.IsCut()) {
                        helpText.text = "This pizza is already cut";
                    } else {
                        StartInteraction();
                    }
                    break;
                case Station.Table:
                    interacting = this.gameObject;
                    // if there is not already a pizza here
                    var tablePie = this.transform.parent.GetComponentInChildren<PizzaObject>();
                    if (currPie == null && tablePie == null) {
                        helpText.text = "You don't have a pizza to place";
                    } else if (currPie != null && tablePie != null) {
                        helpText.text = "This spot is occupied";
                    } else {
                        // can pick up or set down
                        StartInteraction();
                    }
                    break;
            }
            helpText.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D c) {
        if (c.gameObject.tag == "Player") {
            sr.color = defaultColor;
            helpText.gameObject.SetActive(false);
            interactable = false;
            SetHelpText();

            if (interacting == this.gameObject) {
                interacting = null;
            }
        }
    }
}
