using System;
using System.Collections;
using System.Collections.Generic;
using FileAnalysis;
using UnityEngine;
using UnityEngine.UI;

public class CreatureSelect : MonoBehaviour
{
    private TextMesh nameLabel;
    private TextMesh sizeLabel;
    private CreatureMove creatureMover;
    private Color selectedColor = new Color(0, 11, 8);

    public void InitCreature(string name, long size, Stats stats)
    {
        nameLabel = transform.GetChild(0).GetComponent<TextMesh>();
        sizeLabel = transform.GetChild(1).GetComponent<TextMesh>();
        creatureMover = GameObject.Find("CreatureHandler").GetComponent<CreatureMove>();
        nameLabel.text = name;
        if (size < (1024 * 100))
        {
            sizeLabel.text = (size / 1024) + "kb";
        }
        else
        {
            sizeLabel.text = (Mathf.Round(size / (1024 * 100)) / 10f) + "mb";
        }
        creatureMover.updateSelectedCreature(this.gameObject);
    }

    void OnMouseDown()
    {
        Debug.Log("Clicked: " + gameObject.GetInstanceID());
        var selected = creatureMover.updateSelectedCreature(this.gameObject);
        if (selected)
        {
            nameLabel.color = selectedColor;
            nameLabel.fontStyle = FontStyle.Bold;
        }
        else
        {
            DeselectCreature();
        }
    }
    public void DeselectCreature()
    {
        nameLabel.color = Color.white;
        nameLabel.fontStyle = FontStyle.Normal;
    }
}
