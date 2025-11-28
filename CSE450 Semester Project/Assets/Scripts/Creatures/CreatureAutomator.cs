using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// there are 4 phases of any creature's automatic behavior
//   0. Idle (waiting to begin next task)
//   1. Getting a pie from a previous station/moving it to current station
//   2. Doing the work of the current station
//   3. Moving the pie to the next station
// "Waiting" phase to allow for pauses between phases
public enum AutomationPhase {
    Idle, TransferToCurrent, StationWork, TransferToNext, Waiting
}


// TODO: I think this should probably be updated to handle TriggerEnter and TriggerExit events instead of the current implementation
// so like if you knock your guy out of doing their work, they have to reset and re-trigger to start over

// TODO: INTEGRATE STAMINA SYSTEM INTO AUTOMATOR

public class CreatureAutomator : MonoBehaviour {
    // some consts
    private const float maxVelocityForStaminaRecovery = 0.02f; // what it sounds like...
    private const float staminaRecoveryRate = 0.4f;

    private const float basePhaseWait = 1.8f; // time it takes creature to start next phase
    private const float baseTransferWait = 0.7f;
    private const float maxStationWorkTime = 6f; // how long a creature with 0 skill takes to complete station work
    private const float maxStationWorkReduce = 0.4f; // max amount station work time can be reduced (aka reduction factor for skill 100)
    
    private const float closeEnoughDist = 0.1f; // how close a creature has to move to its assigned station
    
    // this is kind of subjective, but I feel like it should always be more effective to play the game yourself
    // e.g. this diminishes the speed of the auto-move compared to when the player controls the same creature
    private const float automationFactor = 0.8f; // factor at which an automatic creature performs in terms of their actual skill levels
    
    
    private class AutoTask {
        public StationInteract prevStation;
        public PizzaObject pizza; // may be null IF prevStation is Station.Order, since we have to create that pie during this task
        public Order order;
        public AutoTask(Order order, PizzaObject pizza, StationInteract prevStation) {
            this.order = order;
            this.pizza = pizza;
            this.prevStation = prevStation;
        }
    }


    private CreatureStats creature;
    private Rigidbody2D rb;
    public StationInteract assignedStation; // public so CreatureInteract can use it
    private PizzaObject assignedPizza; // not yet implemented

    private Queue<AutoTask> queuedTasks;
    private AutoTask task;
    private AutomationPhase phase;

    private CreatureAssign assigner = null;
    private Coroutine activeTask;
    public GameObject pizzaPrefab;


    void Start() {
        assigner = GameObject.Find("CreatureHandler")?.GetComponent<CreatureAssign>();
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        creature = this.gameObject.GetComponent<CreatureStats>();

        queuedTasks = new Queue<AutoTask>();
        assignedStation = null;
        task = null;
        phase = AutomationPhase.Idle;
    }
    
    public void UnassignCreature() {
        Debug.Log("Clearing all tasks from automator");
        // basically cancel all queued tasks
        if (activeTask != null) {
            StopCoroutine(activeTask);
        }
        queuedTasks?.Clear();
        task = null;
        assignedStation = null;
    }
    public void BeginStationAutomation(StationInteract s) {
        assignedStation = s;
        phase = AutomationPhase.Idle;
    }
    public void StopStationAutomation() {
        // keep state, just stop working basically
        assignedStation = null;
    }

    public void BeginPizzaAutomation(PizzaObject p) {
        if (p.GetLinkedOrder() == null) { return; } // can't do anything to a pizza w/o an order
        
        assignedPizza = p;
        StartCoroutine("CompleteAssignedPizza");
    }
    public void EndPizzaAutomation() {
        StopCoroutine("CompleteAssignedPizza");
        StopCoroutine("TopPizza");
        assignedPizza = null;
    }
    private IEnumerator CompleteAssignedPizza() {
        if (assignedPizza.GetToppingCount() == 0 && assignedPizza.GetLinkedOrder().GetToppings().Count > 0) {
            yield return StartCoroutine("TopPizza");
        }
    }

    private IEnumerator TopPizza() {
        // find top station 
        // move there
        // simulate topping work

        // placeholder
        yield return new WaitForEndOfFrame();
    }

    public void ResetStationAutomation() {
        // continue working, but state may have changed (e.g. if )
        var assigned = assignedStation;
        StopStationAutomation();
        BeginStationAutomation(assigned);
    }


    public void HandleNewOrder(Order order, StationInteract orderStation) {
        // only toss should be allowed to handle new orders
        if (assignedStation.station == Station.Toss) {
            queuedTasks.Enqueue(new AutoTask(order, null, orderStation));
        }
    }
    public void HandlePizza(PizzaObject pizza, StationInteract table) {
        // toss should not handle pizza objects
        if (assignedStation.station != Station.Toss) {
            queuedTasks.Enqueue(new AutoTask(null, pizza, table));
        }
    }
    
    private int RandomBellCurve(int min, int max) {
        return Mathf.RoundToInt((Random.Range(min, max) + Random.Range(min, max)) / 2f);
    }


    // basically just lets us delay next move to give impression of "doing work"
    private IEnumerator FinishPhase(float waitForNext, AutomationPhase ph) {
        phase = AutomationPhase.Waiting;
        switch (ph) {
            case AutomationPhase.Idle:
                yield return new WaitForSeconds(waitForNext);
                // if you have the pie already, just go
                if (this.gameObject.GetComponentInChildren<PizzaObject>() != null) {
                    phase = AutomationPhase.StationWork;
                } else {
                    phase = AutomationPhase.TransferToCurrent;
                }
                break;
            case AutomationPhase.TransferToCurrent:
                yield return new WaitForSeconds(waitForNext);
                phase = AutomationPhase.StationWork;
                break;
            case AutomationPhase.StationWork:
                phase = AutomationPhase.Idle; // always have creature run back to station it's working at if it gets pushed around
                switch (assignedStation.station) {
                    case Station.Toss:
                        yield return new WaitForSeconds(waitForNext);
                        // have to create new pie
                        var newPie = Instantiate(pizzaPrefab);

                        // TODO: quality should depend on toss skill... better than this maybe
                        int minQual = (int) (creature.GetTossStat() * automationFactor); // automation factor here so that creature will likely perform suboptimal
                        int quality = RandomBellCurve(minQual, creature.GetTossStat());

                        newPie.GetComponent<PizzaObject>().InitializePizza(quality);
                        newPie.GetComponent<PizzaObject>().LinkOrder(task.order);
                        task.pizza = newPie.GetComponent<PizzaObject>();
                        newPie.transform.parent = creature.gameObject.transform;
                        newPie.transform.localPosition = TossGameManager.pizzaOffset;
                        break;
                    case Station.Top:
                        float accuracy = automationFactor * creature.GetTopStat() / 100f;
                        List<Topping> toppings = task.pizza.GetLinkedOrder().GetToppings();
                        foreach (Topping t in toppings) {
                            yield return new WaitForSeconds(waitForNext / toppings.Count); // incrementally add with delay
                            if ((Random.Range(0f, 1f) < accuracy)
                                || (creature.GetTopStat() >= 70 && Random.Range(0f, 1f) < accuracy)
                                || (creature.GetTopStat() >= 80 && Random.Range(0f, 1f) < accuracy)
                                || (creature.GetTopStat() >= 90 && Random.Range(0f, 1f) < accuracy)
                                // special ability for skilled toppers, more chances to have
                                // accurate placement if you hit above some thresholds
                            ) {
                                task.pizza.AddTopping(t);
                            }
                        }
                        break;
                    case Station.Ovens:
                        // this one is kind of tough, because it should rely a bit on the actual cook time
                        // of a pizza and always do a bit worse than the player 
                        float skill = Mathf.Max(0.01f, creature.GetOvensStat() / 100f); // cannot be 0 since we're dividing...
                        float target = task.pizza.GetLinkedOrder().GetTargetCookLevel();
                        while (task.pizza.GetAverageCookLevel() < target) {
                            // another special ability type thing, if INCREDIBLY SKILLED OVEN-er, go twice as fast
                            if (skill >= 0.95f) {
                                yield return new WaitForSeconds(OvenGameManager.baseTickRate / 1.5f);
                            } else {
                                yield return new WaitForSeconds(OvenGameManager.baseTickRate);
                            }
                
                            var updated = task.pizza.GetCookLevels();
                            for (int i = 0; i < updated.Length; i++) {
                                var cook = Mathf.Lerp(OvenSliceController.minCookRate, OvenSliceController.maxCookRate, skill);
                                cook *= Random.Range(automationFactor * skill, 1f);
                                updated[i] += (int)cook;
                            }
                            task.pizza.SetCookLevels(updated);
                        }
                        break;
                    case Station.Cut:
                        yield return new WaitForSeconds(waitForNext);
                        // automation factor here so that creature usually underperforms compared to skill lvl
                        int minCutQual = (int) (creature.GetCutStat() * automationFactor);
                        int cutQual = RandomBellCurve(minCutQual, creature.GetCutStat());
                        task.pizza.CutPizza(cutQual);
                        break;
                }
                phase = AutomationPhase.TransferToNext;
                break;
            case AutomationPhase.TransferToNext:
                yield return new WaitForSeconds(waitForNext);
                task = null; // completed! handle next one (if there is one)
                phase = AutomationPhase.Idle;
                Debug.Log("Completed task, back to idle.");
                break;
        }
        activeTask = null; // end of task
    }



    // basically just handles stamina recovery 
    void Update() {
        if(creature.stamina < creature.maxStamina) {
            // always recover a little bit of stamina
            creature.RecoverStamina(Time.deltaTime * staminaRecoveryRate);
            if (assignedStation == null) {
                // if no station, regain extra stamina when rigid body is not moving
                if (rb.velocity.magnitude <= maxVelocityForStaminaRecovery) {
                    creature.RecoverStamina(Time.deltaTime * staminaRecoveryRate * 2);
                }
            } else {
                // if assigned, should be in "Idle" phase
                if (phase == AutomationPhase.Idle) {
                    creature.RecoverStamina(Time.deltaTime * staminaRecoveryRate * 2);
                }
            }
        }
    }



    // FixedUpdate because physics
    void FixedUpdate() {
        if (assignedStation != null) {
            // if new task, handle that, if already handling task, do that...
            if (task == null && queuedTasks.Count > 0) {
                task = queuedTasks.Dequeue();

                // DONT ALLOW THESE GUYS TO REDO WORK THAT'S ALREADY DONE!!!!
                // also if they don't have a pizza order they won't know what to do (except for toss)
                switch (assignedStation.station) {
                    case Station.Top:
                        if (task.pizza.GetLinkedOrder() == null || task.pizza.GetToppingCount() > 0) {
                            task = null;
                            phase = AutomationPhase.Idle;
                            return;
                        }
                        break;
                    case Station.Ovens:
                        if (task.pizza.GetLinkedOrder() == null
                            || task.pizza.GetAverageCookLevel() >= task.pizza.GetLinkedOrder().GetTargetCookLevel()
                        ) {
                            task = null;
                            phase = AutomationPhase.Idle;
                            return;
                        }
                        break;
                    case Station.Cut:
                        if (task.pizza.GetLinkedOrder() == null || task.pizza.IsCut() ) {
                            task = null;
                            phase = AutomationPhase.Idle;
                            return;
                        }
                        break;
                }

                // if no linked order and not toss, automator doesn't know what to do
                if (assignedStation.station != Station.Toss && task.pizza.GetLinkedOrder() == null) {
                    // if this is the case, we will ignore this task
                    task = null;
                    phase = AutomationPhase.Idle;
                } else {
                    activeTask = StartCoroutine(FinishPhase(basePhaseWait, AutomationPhase.Idle));
                }
            }

            Vector2 targetPos;
            Vector2 dir;
            // Vector3 offset;
            switch (phase) {
                case AutomationPhase.Waiting: break; // wait...
                case AutomationPhase.Idle:
                    targetPos = assignedStation.transform.position;
                    dir = targetPos - (Vector2)creature.transform.position;
                    if (dir.magnitude > closeEnoughDist) {
                        float speed = creature.speed * automationFactor;
                        rb.velocity = dir.normalized * speed;
                    } else {
                        rb.velocity = Vector2.zero;
                    }
                    break;
                case AutomationPhase.TransferToCurrent:
                    // if not toss station, it's possible pizza gets picked up while we're trying to claim it
                    if (assignedStation.station != Station.Toss && !task.prevStation.HasPie(task.pizza)){
                        // if this happens, cancel this task
                        task = null;
                        phase = AutomationPhase.Idle;
                        return;
                    }
                
                    targetPos = task.prevStation.transform.position;
                    dir = targetPos - (Vector2)creature.transform.position;
                    if (dir.magnitude > closeEnoughDist) {
                        float speed = creature.speed * automationFactor;
                        rb.velocity = dir.normalized * speed;
                    } else {
                        rb.velocity = Vector2.zero;
                        if (assignedStation.station == Station.Toss) {
                            // since we already have the order reference in AutoTask
                            // we just need to pretend we're doing the ticket claim stuff
                            activeTask = StartCoroutine(FinishPhase(basePhaseWait, AutomationPhase.TransferToCurrent));
                        } else {
                            // otherwise actually claim the pizza
                            if (task.prevStation.TryClaimPie(this.gameObject)) {
                                activeTask = StartCoroutine(FinishPhase(baseTransferWait, AutomationPhase.TransferToCurrent));
                            } else {
                                Debug.LogWarning("Pie not claimed from task's previous section. Ending task.");
                                task = null;
                                phase = AutomationPhase.Idle;
                                return;
                            }
                        }
                    }
                    break;
                case AutomationPhase.StationWork:
                    // move back to assigned station w new order or pie
                    targetPos = assignedStation.transform.position;
                    dir = targetPos - (Vector2)creature.transform.position;
                    if (dir.magnitude > closeEnoughDist) {
                        float speed = creature.speed * automationFactor;
                        rb.velocity = dir.normalized * speed;
                    } else {
                        rb.velocity = Vector2.zero;

                        // once close enough, do work...
                        float time = maxStationWorkTime;
                        switch (assignedStation.station) {
                            // ovens work will just ignore this since it actually does the cooking
                            case Station.Toss:
                                time *= Mathf.Lerp(maxStationWorkReduce, 1f, 1f - creature.GetTossStat() / 100f);
                                break;
                            case Station.Top:
                                time *= Mathf.Lerp(maxStationWorkReduce, 1f, 1f - creature.GetTopStat() / 100f);
                                break;
                            case Station.Cut:
                                time *= Mathf.Lerp(maxStationWorkReduce, 1f, 1f - creature.GetCutStat() / 100f);
                                break;
                        }
                        activeTask = StartCoroutine(FinishPhase(time, AutomationPhase.StationWork));
                    }
                    break;
                case AutomationPhase.TransferToNext:
                    // move pie to wherever it should be next (always ask CreatureAssign script)
                    StationInteract target = assigner.GetAvailableTable(assignedStation.station);
                    if (target != null) { 
                        targetPos = target.transform.position;
                        dir = targetPos - (Vector2)creature.transform.position;
                        if (dir.magnitude > closeEnoughDist) {
                            float speed = creature.speed * automationFactor;
                            rb.velocity = dir.normalized * speed;
                        } else {
                            rb.velocity = Vector2.zero;
                            
                            // once close enough, do transfer...
                            if (!target.TryPlacePie(task.pizza)) {
                                Debug.LogError("FAILED TO PLACE PIE, AVAILABLE TABLE WAS WRONG");
                            } else if (assignedStation.station != Station.Cut) { // don't send ping for cut, since that's the completed pie
                                // send ping to next station that this was placed...
                                assigner.HandlePlacedPizza(task.pizza, target, assignedStation.station);
                            }
                            activeTask = StartCoroutine(FinishPhase(baseTransferWait, AutomationPhase.TransferToNext));
                        }
                    }
                    break;
            }
        }
    }
}
