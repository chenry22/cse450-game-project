using System;
using System.Collections;
using System.Collections.Generic;
using FileAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Script handling basic in-game creature interaction and view
// Essentially triggers for mouse events interfacing w CreatureAssign and CreatureMove

public class CreatureSelect : MonoBehaviour {
    private Sprite spr;
    private Color spriteColor;
    private TMP_Text nameLabel;
    private TMP_Text sizeLabel;
    private CreatureMove creatureMover;
    private CreatureAssign creatureAssign;

    // label colors for different states
    private Color normalColor = Color.black;
    private Color selectedColor = new Color(0, 49/255f, 118/255f);
    private Color highlightedColor = new Color(230/255f, 32/255f, 65/255f);

    public string GetName() { return nameLabel.text; }
    public Sprite GetSprite() { return spr; }
    public Color GetSpriteColor() { return spriteColor;  }

    public void InitCreature(string name, long size, Stats stats) {
        spr = GetComponent<SpriteRenderer>().sprite;
        spriteColor = GetComponent<SpriteRenderer>().color;
        nameLabel = transform.GetChild(0).GetComponent<TMP_Text>();
        sizeLabel = transform.GetChild(1).GetComponent<TMP_Text>();
        creatureMover = GameObject.Find("CreatureHandler").GetComponent<CreatureMove>();
        creatureAssign = GameObject.Find("CreatureHandler").GetComponent<CreatureAssign>();

        nameLabel.text = name;
        if (size < (1024 * 100)) {
            sizeLabel.text = (size / 1024) + "kb";
        } else {
            sizeLabel.text = (Mathf.Round(size / (1024 * 100)) / 10f) + "mb";
        }
        SetTextSelected();
        creatureMover.UpdateSelectedCreature(this.gameObject); // new creatures are automatically taken control of by the user
        GetComponent<CreatureStats>().SetStats(stats, size); // moved setup to stats script
    }

    void OnMouseDown() {
        if (!creatureMover.IsSelectedCreature(this.gameObject)) {
            creatureAssign.BeginTrackingCreature(this);
            SetTextHighlighted();
        }
    }
    void OnMouseUp() {
        creatureAssign.FinishTrackingCreature(this);
        if (creatureMover.IsSelectedCreature(this.gameObject)) {
            SetTextSelected();
        } else {
            SetTextNormal();
        }
    }

    // Only triggers when OnMouseUp was on SAME collider as the last OnMouseDown
    // if selected like this, change creature selection
    // this seemingly always happens BEFORE OnMouseUp()
        // probably, maybe
    void OnMouseUpAsButton() {
        var selected = creatureMover.UpdateSelectedCreature(this.gameObject);
        if (selected) {
            SetTextSelected();
        }
    }

    public void SetTextSelected() {
        nameLabel.color = selectedColor;
        nameLabel.fontStyle = FontStyles.Bold;
    }
    public void SetTextHighlighted() {
        nameLabel.color = highlightedColor;
        nameLabel.fontStyle = FontStyles.Italic;
    }
    public void SetTextNormal() {
        nameLabel.color = normalColor;
        nameLabel.fontStyle = FontStyles.Normal;
    }
    public void DeselectCreature() {
        // i know this is silly rn, but we may want to add other functionality later
        SetTextNormal();
    }
}
