using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// This script handles mini-game activation and general station interfacing through a Collider2D
    // For this to function probably the Collider2D of the object this is attached to must be marked
    // as a trigger, and only GameObject with the "Player" tag will trigger collisions

public enum Station {
    Toss, Top, Ovens, Cut, Table,
    Orders, Submit, Trash, Creatures
}


public class StationInteract : MonoBehaviour {
    // kind of janky solution, basically prevents multiple simultaneous interactions
    public static GameObject interacting = null;
    private DayManager dayManager;
    private CreatureAssign assigner;


    public Station station = Station.Table; // default to table
    public GameObject stationGame;
    public TMP_Text helpText;
    public Color defaultColor = new Color(1f, 1f, 1f, 0.1f);
    public Color triggeredColor = new Color(1f, 0, 0, 0.2f);

    private SpriteRenderer sr; // for changing color to show interaction
    private bool interactable = false;

    

    void Start() {
        dayManager = GameObject.Find(DayManager.dayManagerObjName).GetComponent<DayManager>();
        assigner = GameObject.Find("CreatureHandler").GetComponent<CreatureAssign>();
        sr = this.gameObject.GetComponentInChildren<SpriteRenderer>();
        sr.color = defaultColor;
        
        switch (station) {
            case Station.Table:
            case Station.Submit:
            case Station.Trash:
                // table, submit, and trash don't have interactable stuff
                break;
            case Station.Ovens:
            case Station.Creatures:
                // creatures and ovens are container parent objects
                // so they should ALWAYS be active
                stationGame.SetActive(true);
                break;
            default:
                // all others are mini-games, should not be active
                stationGame.SetActive(false);
                break;
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
                        var playerCreature = GameObject.FindWithTag("Player");
                        var stats = playerCreature.GetComponent<CreatureStats>();
                        if (stats.stamina < TossGameManager.requiredStamina) {
                            helpText.text = "Too tired! Rest to regain stamina.";
                            helpText.gameObject.SetActive(true);
                            return;
                        }

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
                        } else  {
                            helpText.gameObject.SetActive(false);
                            stationGame.GetComponent<OvenGameManager>().ShowOvenUI();
                        }
                    }
                    break;
                case Station.Cut:
                    if (Input.GetKeyDown(KeyCode.E) && stationGame != null) {
                        var playerCreature = GameObject.FindWithTag("Player");
                        var stats = playerCreature.GetComponent<CreatureStats>();
                        if (stats.stamina < CutGameManager.requiredStamina) {
                            helpText.text = "Too tired! Rest to regain stamina.";
                            helpText.gameObject.SetActive(true);
                            return;
                        }
                        
                        helpText.gameObject.SetActive(false);
                        stationGame.SetActive(true);
                        stationGame.GetComponent<CutGameManager>().BeginCutGame();
                        interactable = false;
                    }
                    break;
                case Station.Table:
                    if (Input.GetKeyDown(KeyCode.E)) {
                        PizzaObject pie = GameObject.FindWithTag("Player").GetComponentInChildren<PizzaObject>();
                        if (!TryPlacePie(pie)) {
                            if (!TryClaimPie(GameObject.FindWithTag("Player"))) {
                                Debug.LogError("Could not place or claim pie from Station.Table interact");
                            }
                        } else {
                            // send ping to creature assignment manager
                            // it will ping any creature that should be watching this station for work
                            assigner.HandlePlacedPizza(pie, this, Station.Table);
                        }
                    }
                    break;
                case Station.Orders:
                    if (Input.GetKeyDown(KeyCode.E) && stationGame != null) {
                        if (!stationGame.gameObject.activeSelf) {
                            helpText.gameObject.SetActive(false);
                            stationGame.GetComponent<OrderStationManager>().ShowOrderUI();
                        } else {
                            helpText.gameObject.SetActive(true);
                            stationGame.GetComponent<OrderStationManager>().CloseOrderUI();
                        }
                    }
                    break;
                case Station.Submit:
                    if (Input.GetKeyDown(KeyCode.E)) {
                        GameObject player = GameObject.FindWithTag("Player");
                        var currPie = player?.GetComponentInChildren<PizzaObject>();
                        if (currPie == null) {
                            Debug.LogWarning("Station submit interactor triggered with null pie.");
                            return;
                        }

                        var ticket = player?.GetComponentInChildren<OrderTicket>();
                        if (currPie.GetLinkedOrder() != null) {
                            var result = dayManager.SubmitOrderWithPizza(currPie.GetLinkedOrder(), currPie);
                            helpText.text = result.ToString();
                        } else if (ticket?.GetOrder() != null) {
                            var result = dayManager.SubmitOrderWithPizza(ticket.GetOrder(), currPie);
                            helpText.text = result.ToString();

                            ticket.SetOrder(null); // order is used now
                        }
                    }
                    break;
                case Station.Trash:
                    if (Input.GetKeyDown(KeyCode.E)) {
                        PizzaObject currPie = GameObject.FindWithTag("Player").GetComponentInChildren<PizzaObject>();
                        if (currPie != null) {
                            Destroy(currPie.gameObject);
                            helpText.text = "Trashed.";
                        }
                    }
                    break;
                case Station.Creatures:
                    if (Input.GetKeyDown(KeyCode.E)) {
                        if (stationGame.transform.GetChild(0).gameObject.activeSelf) {
                            SetHelpText();
                            helpText.gameObject.SetActive(true);
                            stationGame.GetComponent<CreatureManagerUIController>().HideCreatureUI();
                        } else {
                            helpText.gameObject.SetActive(false);
                            stationGame.GetComponent<CreatureManagerUIController>().ShowCreatureUI();            
                        }
                    }
                    break;
            }
        }

        // should trigger if creature selection is swapped during an interaction
        // cancels previous interaction to reset station for player
        // HOWEVER comma should not affect Order station so that NEW ORDER noti stays
        if (interacting == null && helpText.gameObject.activeSelf && this.station != Station.Orders) {
            sr.color = defaultColor;
            helpText.gameObject.SetActive(false);
            interactable = false;
            SetHelpText();
        }
    }
    
    public bool HasPie(PizzaObject pie) {
        PizzaObject ownedPie = this.transform.parent.GetComponentInChildren<PizzaObject>();
        return ownedPie.Equals(pie);
    }
    
    public bool TryPlacePie(PizzaObject pie) {
        var tablePie = this.transform.parent.GetComponentInChildren<PizzaObject>();
        if (tablePie == null) {
            pie.transform.parent = this.gameObject.transform.parent;
            pie.transform.localPosition = Vector2.zero;
            return true;
        }
        return false;
    }
    public bool TryClaimPie(GameObject creature) {
        var tablePie = this.transform.parent.GetComponentInChildren<PizzaObject>();
        if (tablePie != null) {
            tablePie.transform.parent = creature.transform;
            tablePie.transform.localPosition = TossGameManager.pizzaOffset;
            return true;
        }
        return false;
    }

    public void ShowStaminaMessage() {
        helpText.text = "You're too tired! Idle to regain stamina.";
        interactable = true;
        sr.color = triggeredColor;
        helpText.gameObject.SetActive(true);
    }
    private void SetHelpText() {
        switch (station)
        {
            case Station.Toss:
                helpText.text = "[E] to begin tossing";
                break;
            case Station.Top:
                helpText.text = "[E] to begin topping";
                break;
            case Station.Ovens:
                helpText.text = "[E] to view oven";
                break;
            case Station.Cut:
                helpText.text = "[E] to begin cutting";
                break;
            case Station.Table:
                helpText.text = "[E] to place/pickup pie";
                break;
            case Station.Orders:
                helpText.text = "[E] to view active orders";
                break;
            case Station.Submit:
                helpText.text = "[E] to submit order";
                break;
            case Station.Trash:
                helpText.text = "[E] to trash pie";
                break;
            case Station.Creatures:
                helpText.text = "[E] to manage creatures";
                break;
        }
    }
    public void StartInteraction() {
        interactable = true;
        sr.color = triggeredColor;
        SetHelpText();
        helpText.gameObject.SetActive(true);
    }
    public void StopInteraction(){
        sr.color = defaultColor;
        
        // idk why this is broken, this maybe fixes it?
        // UPDATE: I think it was a collider that was assigned, we should (?) be able to remove this
        if (station == Station.Ovens) {
            stationGame.GetComponent<OvenGameManager>().CloseOvenUI();
        }

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
                    // either the slot must be empty OR player must not have an active pizza
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
                case Station.Orders:
                    // this is a view you should always be allowed to access
                    StartInteraction();
                    break;
                case Station.Submit:
                    if (currPie == null) {
                        helpText.text = "You must be holding a pizza to do this";
                    } else {
                        var currOrder = currPie.GetLinkedOrder();
                        if (currOrder == null) {
                            var ticket = c.gameObject.GetComponentInChildren<OrderTicket>()?.GetOrder();
                            if (ticket == null) {
                                helpText.text = "This pizza is not linked to an order";
                            }  else {
                                StartInteraction();
                            }
                        } else {
                            StartInteraction();
                        }
                    }
                    break;
                case Station.Trash:
                    if (currPie == null) {
                        helpText.text = "You must be holding a pizza to do this.";
                    } else {
                        StartInteraction();
                    }
                    break;
                case Station.Creatures:
                    StartInteraction();
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
