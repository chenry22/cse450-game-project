using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script that handles assigning a creature to a station in the kitchen

public class CreatureAssign : MonoBehaviour
{
    // mapping station object to creature objects (using parent game objects)
    private Dictionary<CreatureSelect, StationInteract> creatureToStation = new Dictionary<CreatureSelect, StationInteract>();
    private CreatureSelect currentCreature = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // returns station creature is assigned to or Station.Table if none
    public Station GetCreatureStation(CreatureSelect cs) {
        if (creatureToStation.ContainsKey(cs)) {
            return creatureToStation[cs].station;
        }
        return Station.Table; // no station
    }

    public void BeginTrackingCreature(CreatureSelect cs)
    {
        currentCreature = cs;
    }
    public void FinishTrackingCreature(CreatureSelect cs) {
        // If same creature as start AND station is defined, do assignment
        if (cs == currentCreature) {
            Debug.Log("STATION ASSIGN");
        }
        currentCreature = null;
    }
}
