using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Helper script that manages with station colliders and mouse evnets

public class StationAssignmentController : MonoBehaviour
{
    private StationInteract si;
    private CreatureAssign creatureAssigner;
    private SpriteRenderer spr;
    private Color baseColor;
    private Color highlightedColor;

    void Start() {
        creatureAssigner = GameObject.Find("CreatureHandler").GetComponent<CreatureAssign>();
        si = GetComponentInChildren<StationInteract>();
        spr = GetComponent<SpriteRenderer>();
        baseColor = spr.color;
        highlightedColor = spr.color;
        highlightedColor.a = 0.8f;
    }

    // when our mouse hovers over a station WHILE BEING DRAGGED (aka mouse down)
    // send ping to CreatureAssign script
    void OnMouseEnter() {
        if (Input.GetMouseButton((int)MouseButton.Left) && creatureAssigner.IsTrackingCreature()) {
            creatureAssigner.BeginTrackingStation(si);
            spr.color = highlightedColor;
        }
    }

    void OnMouseExit() {
        if (Input.GetMouseButton((int)MouseButton.Left)) {
            creatureAssigner.StopTrackingStation();
        }
        spr.color = baseColor;
    }

    void OnMouseUp() {
        spr.color = baseColor;
    }
}
