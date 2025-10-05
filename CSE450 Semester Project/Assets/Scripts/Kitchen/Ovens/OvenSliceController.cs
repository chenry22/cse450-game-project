using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Helper class which manages the individual oven slots/slices
// Depending on the position relative to the heat source, the slot will heat up slower or faster

public enum OvenPosition {
    Coldest, Cold, // further from heat source
    Hot, Hottest // closer to heat source
}

public class OvenSliceController : MonoBehaviour {
    private const float actualMaxAlpha = 255f; // dont change this...

    private const float cookedAlpha = 200f; // whatever we define as "Cooked" as far as visual color
    public static int maxCookRate = 7; // how much the hottest part of the oven cooks per tick
    public static int minCookRate = 4; // how the much coldest part of the oven cooks per tick (must be less than maxCookRate) 

    private int midDiff = Mathf.FloorToInt((maxCookRate - minCookRate) / 2f); // probably dont need to change this? defining cook rates of non max slots

    public OvenPosition slotHeat;
    public TMP_Text cookTxt;
    public SpriteRenderer spr;

    private Color baseCookColor = new Color(179/255f, 113/255f, 34/255f);
    private int cookLevel = 0;

    public int GetCookLevel() { return cookLevel; }
    public Color GetSpriteColor() { return spr.color; }

    void Start() {
        spr = GetComponent<SpriteRenderer>();
        cookTxt = GetComponentInChildren<TMP_Text>();
        SetOvenSlice(0); // reset 
    }
    
    public void UpdateDisplay() {
        var newColor = baseCookColor;
        newColor.a = cookedAlpha / actualMaxAlpha * (cookLevel / 100f);
        spr.color = newColor;
        cookTxt.text = cookLevel.ToString();
    }

    public void SetOvenSlice(int cookLevel) {
        this.cookLevel = cookLevel;
        UpdateDisplay();
    }
    public void CookSlice() {
        switch (slotHeat) {
            case OvenPosition.Coldest:
                cookLevel += minCookRate;
                break;
            case OvenPosition.Cold:
                cookLevel += minCookRate + midDiff;
                break;
            case OvenPosition.Hot:
                cookLevel += maxCookRate - midDiff;
                break;
            case OvenPosition.Hottest:
                cookLevel += maxCookRate;
                break;
        }
        UpdateDisplay();
    }
}
