using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Helper class that handles interaction with the top game

public class ToppingSlot : MonoBehaviour {
    public TopGameManager manager;
    public SpriteRenderer spr;
    public TMP_Text slotTxt;

    public Color hovered = new Color(0, 0, 0, 0.1f);
    public Color normal = new Color(0, 0, 0, 0.05f);
    private Topping currTopping;

    // Start is called before the first frame update
    void Start() {
        slotTxt = this.GetComponentInChildren<TMP_Text>();
        spr = this.GetComponent<SpriteRenderer>();
        spr.color = normal;
        slotTxt.text = "---";
    }

    public void SetText(string s) {
        if(slotTxt != null) {
            slotTxt.text = s;
        }
    }
    public void SetCurrentTopping(Topping t) {
        currTopping = t;
        SetText(ToppingMethods.ToString(t));
    }

    public void OnMouseEnter() {
        if (manager.IsGameActive()) {
            spr.color = hovered;
        }
    }
    public void OnMouseExit() {
        spr.color = normal;
    }

    public void OnMouseDown() {
        if (manager.IsGameActive()) {
            manager.SelectTopping(currTopping);
            spr.color = normal;
        }
    }
}
