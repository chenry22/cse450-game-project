using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script that handles assigning a creature to a station in the kitchen

// TODO: right now this is a lazy implementation where the creature just goes in a straight line
// to get to its assigned station... works for simple kitchen layouts, but if there was an
// immovable object blocking it, it would get very stuck

public class CreatureAssign : MonoBehaviour
{
    // how close a creature has to move to its assigned station
    private const float closeEnoughDist = 0.1f; // this is probably too small, so updates will happen basically every frame
    // this is kind of subjective, but I feel like the player should always feel faster
    // this diminishes the speed of the auto-move compared to when the player controls the same creature
    private const float speedFactor = 0.8f;

    // mapping station object to creature objects (using parent game objects)
    private Dictionary<CreatureSelect, StationInteract> creatureToStation = new Dictionary<CreatureSelect, StationInteract>();
    private CreatureSelect currentCreature = null;
    private StationInteract currentStation = null;

    // use fixed update because physics engine prefers that i guess
    void FixedUpdate()
    {
        if (creatureToStation.Count > 0)
        {
            foreach (var pair in creatureToStation)
            {
                var cs = pair.Key;
                var station = pair.Value;
                if (Vector2.Distance(cs.transform.position, station.transform.position) > closeEnoughDist)
                {
                    // player should 
                    float speed = cs.gameObject.GetComponent<CreatureStats>().speed * speedFactor;
                    Vector3 offset = (station.transform.position - cs.transform.position).normalized * Time.fixedDeltaTime * speed;
                    cs.gameObject.GetComponent<Rigidbody2D>().MovePosition(cs.transform.position + offset);
                }
            }
        }
    }

    public void UnassignCreature(CreatureSelect cs)
    {
        creatureToStation.Remove(cs);
    }

    // returns station creature is assigned to or Station.Table if none
    public Station GetCreatureStation(CreatureSelect cs)
    {
        if (creatureToStation.ContainsKey(cs))
        {
            return creatureToStation[cs].station;
        }
        return Station.Table; // no station
    }


    public bool IsTrackingCreature()
    {
        return currentCreature != null;
    }
    public void BeginTrackingCreature(CreatureSelect cs)
    {
        currentCreature = cs;
        currentStation = null;
    }
    public void FinishTrackingCreature(CreatureSelect cs)
    {
        // If same creature as start AND station is defined, do assignment
        if (cs == currentCreature && currentStation != null)
        {
            creatureToStation[cs] = currentStation;
            // TODO: send signal to MOVE this guy to the station (if not already there)

        }
        currentCreature = null;
        currentStation = null;
    }

    public void BeginTrackingStation(StationInteract si)
    {
        currentStation = si;
    }
    public void StopTrackingStation()
    {
        currentStation = null;
    }

    public bool IsCreatureIdle(CreatureSelect cs)
{
    return GetCreatureStation(cs) == Station.Table;
}
}
