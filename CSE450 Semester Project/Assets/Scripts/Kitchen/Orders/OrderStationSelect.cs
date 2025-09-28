using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// in the order station UI, handles order being selected
public class OrderStationSelect : MonoBehaviour {
    public OrderStationManager manager;
    public int index;

    private TMP_Text label;
    private SpriteRenderer spr;
    private Color hovered = new Color(.97f, .97f, .97f);
    private Color normal = new Color(1f, 1f, 1f);

    private Order order = null;

    void Start() {
        label = this.GetComponent<TMP_Text>();
        spr = this.GetComponentInChildren<SpriteRenderer>();
        spr.color = normal;
        label.text = "---";
    }
    
    public void SetOrder(Order o) {
        this.order = o;
        SetText(o.ToString());
    }
    
    private void SetText(string s) {
        if(label != null) {
            label.text = s;
        }
    }

    public void OnMouseEnter() {
        spr.color = hovered;
    }
    public void OnMouseExit() {
        spr.color = normal;
    }

    public void OnMouseDown() {
        manager.SelectOrder(order);
        spr.color = normal;
    }
}
