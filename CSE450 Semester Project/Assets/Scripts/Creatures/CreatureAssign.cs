using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Script that handles assigning a creature to a station in the kitchen
// Linked to a GLOBAL creature handler object, not to individual creatures
// also HELPS w creature automation by sending appropriate pings, does not do the indiviudal work though

// logic for indivudal creature behvario is in CreatureAutomator

public class CreatureAssign : MonoBehaviour {

    // mapping station object to creature objects (using parent game objects)
    private Dictionary<CreatureSelect, StationInteract> creatureToStation = new Dictionary<CreatureSelect, StationInteract>();
    private CreatureSelect currentCreature = null;
    private StationInteract currentStation = null;


    // for handling intermediate phases of automation
    public StationInteract[] tossTables; // valid spots for pies to be placed after tossed
    public StationInteract[] topTables; // etc., etc.
    public StationInteract[] ovenTables;
    public StationInteract[] cutTables;


    public void UnassignCreature(CreatureSelect cs) {
        // stop any automation work happening
        cs.gameObject.GetComponent<CreatureAutomator>().UnassignCreature();

        cs.gameObject.GetComponent<CreatureAutomator>().StopStationAutomation();
        creatureToStation.Remove(cs);
    }

    // returns station creature is assigned to or Station.Table if none
    public Station GetCreatureStation(CreatureSelect cs) {
        if (creatureToStation.ContainsKey(cs)) {
            return creatureToStation[cs].station;
        }
        return Station.Table; // no station
    }


    // IF there is a creature assigned to a toss station, tell them to go handle this
    // TODO: currently this is a lazy implementation, just gets the first creature handling toss stuff
    public void HandleNewOrder(Order order, StationInteract orderStation ) {
        foreach (var pair in creatureToStation.ToArray()) {
            if (pair.Value.station == Station.Toss) {
                // send ping to automator
                pair.Key.gameObject.GetComponent<CreatureAutomator>().HandleNewOrder(order, orderStation);
                return; // should send at most one ping (only one guy should work on this)
            }
        }
    }
    
    private Station FindNextStationForTable(StationInteract table) {
        foreach (StationInteract t in tossTables) {
            if (table.Equals(t)) {
                return Station.Top;
            }
        }
        foreach (StationInteract t in topTables) {
            if (table.Equals(t)) {
                return Station.Ovens;
            }
        }
        foreach (StationInteract t in ovenTables) {
            if (table.Equals(t)) {
                return Station.Cut;
            }
        }
        return Station.Table;
    }
    public void HandlePlacedPizza(PizzaObject pizza, StationInteract table, Station justCompleted) {
        Station nextStation = Station.Table;
        switch (justCompleted) {
            case Station.Table:
                // special flag, means user placed, so we don't know what was before...
                // have to search all recognized tables...
                nextStation = FindNextStationForTable(table);
                break;
            case Station.Toss:
                nextStation = Station.Top;
                break;
            case Station.Top:
                nextStation = Station.Ovens;
                break;
            case Station.Ovens:
                nextStation = Station.Cut;
                break;
        }
        if (nextStation == Station.Table) {
            Debug.LogWarning("HandlePlacedPizza() called, but could not identify a valid next station.");
            return;
        }
        Debug.Log("Handling placed pie, next station: " + nextStation);
        
        // TODO: currently greedy approach; find first valid candidate creature to work on this and give them the work.
        // ideally it would find all valid candidates and sends task to one with least queued work
        foreach (var pair in creatureToStation.ToArray()) {
            if (pair.Value.station == nextStation) {
                pair.Key.gameObject.GetComponent<CreatureAutomator>().HandlePizza(pizza, table);
                return; // should send at most one ping (only one guy should work on this)
            }
        }
    }
    
    public StationInteract GetAvailableTable(Station prev) {
        StationInteract[] tables = {};
        switch (prev) {
            case Station.Toss:
                tables = tossTables;
                break;
            case Station.Top:
                tables = topTables;
                break;
            case Station.Ovens:
                tables = ovenTables;
                break;
            case Station.Cut:
                tables = cutTables;
                break;
        }
        foreach (StationInteract table in tables) {
            // if not holding a pizza, it is a valid candidate table
            if (table.transform.parent.GetComponentInChildren<PizzaObject>() == null) {
                return table;
            }
        }
        return null;
    }



    // creature assignment logic
    public int CreaturesAssigned() { return creatureToStation.Count; }
    public bool IsTrackingCreature() {
        return currentCreature != null;
    }
    public void BeginTrackingCreature(CreatureSelect cs) {
        currentCreature = cs;
        currentStation = null;
    }
    public void FinishTrackingCreature(CreatureSelect cs) {
        // If same creature as start AND station is defined, do assignment
        if (cs == currentCreature && currentStation != null) {
            creatureToStation[cs] = currentStation;
            cs.gameObject.GetComponent<CreatureAutomator>().BeginStationAutomation(currentStation);
        }
        currentCreature = null;
        currentStation = null;
    }
    public void BeginTrackingStation(StationInteract si) {
        currentStation = si;
    }
    public void StopTrackingStation() {
        currentStation = null;
    }

    // // TODO: i am considering making it so you can assign a creature a specific pizza, which would break this i think
    // public bool IsCreatureIdle(CreatureSelect cs) {
    //     return GetCreatureStation(cs) == Station.Table;
    // }
}
