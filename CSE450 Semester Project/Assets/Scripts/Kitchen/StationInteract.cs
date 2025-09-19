using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// This script handles mini-game activation and general station interfacing through a Collider2D
    // For this to function probably the Collider2D of the object this is attached to must be marked
    // as a trigger, and only GameObject with the "Player" tag will trigger collisions

public enum Station {
    Toss, Top, Ovens, Cut, Table
}


public class StationInteract : MonoBehaviour {
    // janky solution, basically prevents multiple simultaneous interactions
    public static GameObject interacting = null;

    public Station station = Station.Table; // default to table
    public GameObject stationGame;
    public TMP_Text helpText;
    public Color defaultColor = new Color(1f, 1f, 1f, 0.1f);
    public Color triggeredColor = new Color(1f, 0, 0, 0.2f);

    private SpriteRenderer sr; // for changing color to show interaction
    private bool interactable = false;

    void Start() {
        sr = this.GetComponent<SpriteRenderer>();
        sr.color = defaultColor;
        
        if (station != Station.Table) {
            stationGame.SetActive(station == Station.Ovens); // ovens should be running in background always
        }
        helpText.gameObject.SetActive(false);
        SetHelpText();
    }

    // handle keyboard input to initialize games
    void Update() {
        if (interactable) {
            switch (station) {
                case Station.Toss:
                    if (Input.GetKeyDown(KeyCode.E) && stationGame != null) {
                        helpText.gameObject.SetActive(false);
                        stationGame.SetActive(true);
                        stationGame.GetComponent<TossGameManager>().BeginTossGame();
                        interactable = false;
                    }
                    break;
                case Station.Top:
                    if (Input.GetKeyDown(KeyCode.E) && stationGame != null) {
                        helpText.gameObject.SetActive(false);
                        stationGame.SetActive(true);
                        stationGame.GetComponent<TopGameManager>().BeginTopGame();
                        interactable = false;
                    }
                    break;
                case Station.Ovens:
                    if (Input.GetKeyDown(KeyCode.E) && stationGame != null) {
                        // first child is the actual game UI
                        if (stationGame.transform.GetChild(0).gameObject.activeSelf) {
                            helpText.gameObject.SetActive(true);
                            stationGame.GetComponent<OvenGameManager>().CloseOvenUI();
                        } else {
                            helpText.gameObject.SetActive(false);
                            stationGame.GetComponent<OvenGameManager>().ShowOvenUI();
                        }
                    }
                    break;
                case Station.Cut:
                    if (Input.GetKeyDown(KeyCode.E) && stationGame != null) {
                        helpText.gameObject.SetActive(false);
                        stationGame.SetActive(true);
                        stationGame.GetComponent<CutGameManager>().BeginCutGame();
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
                            tablePie.transform.localPosition = new Vector2(0.6f, 0.2f);
                        }
                    }
                    break;
            }
        }

        // should trigger if creature selection is swapped during an interaction
        // cancels previous interaction to reset station for player
        if (interacting == null && helpText.gameObject.activeSelf) {
            sr.color = defaultColor;
            helpText.gameObject.SetActive(false);
            interactable = false;
            SetHelpText();
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
    public void StartInteraction() {
        interactable = true;
        sr.color = triggeredColor;
        helpText.gameObject.SetActive(true);
    }
    public void StopInteraction(){
        sr.color = defaultColor;
        helpText.gameObject.SetActive(false);
        interactable = false;
        SetHelpText();
    }

    void OnTriggerEnter2D(Collider2D c) {
        if (c.gameObject.tag == "Player") {
            if(interacting != null) {
                interacting.GetComponent<StationInteract>().StopInteraction();
            }
            interacting = this.gameObject;

            var currPie = c.gameObject.GetComponentInChildren<PizzaObject>();
            switch (station) {
                case Station.Toss:
                    // player cannot already have a pizza object
                    if (currPie != null) {
                        helpText.text = "You can't do this while holding something";
                    } else {
                        StartInteraction();
                    }
                    break;
                case Station.Top:
                    // player must be holding a pizza object
                    if (currPie == null) {
                        helpText.text = "You must be holding a pizza to do this";
                    } else {
                        StartInteraction();
                    }
                    break;
                case Station.Ovens:
                    // this just opens the oven view, so it should always be allowed
                    StartInteraction();
                    break;
                case Station.Cut:
                    if (currPie == null) {
                        helpText.text = "You must be holding a pizza to do this";
                    } else if (currPie.IsCut()) {
                        helpText.text = "This pizza is already cut";
                    } else {
                        StartInteraction();
                    }
                    break;
                case Station.Table:
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
            StopInteraction();
            if (interacting == this.gameObject) {
                interacting = null;
            }
        }
    }
}
