using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Topping {
    Sauce, Cheese, Mushroom, Pepperoni
}

public class PizzaObject : MonoBehaviour {
    public Color baseColor = new Color(237, 219, 152); // yellowish
    private SpriteRenderer spr;
    private int tossQuality = -1;
    private Topping[] toppings = null;
    private int cutQuality = -1;

    // Start is called before the first frame update
    void Start() {
        spr = GetComponent<SpriteRenderer>();
        spr.color = baseColor;
    }

    public void InitializePizza(int tossQuality) {
        this.tossQuality = tossQuality;
    }
    public void CutPizza(int cutQuality) {
        this.cutQuality = cutQuality;
    }

    // state management helpers
    public bool IsTopped()
    {
        return toppings != null;
    }
    public bool IsCut() {
        return cutQuality >= 0;
    }
}
