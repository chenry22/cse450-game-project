using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaObject : MonoBehaviour {
    public Color baseColor = new Color(237, 219, 152); // yellowish
    private SpriteRenderer spr;
    private int tossQuality = -1;
    private List<Topping> toppings = new List<Topping>();
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
    public void AddTopping(Topping t) {
        toppings.Add(t);
    }

    // state management helpers
    public bool IsCut() {
        return cutQuality >= 0;
    }
    public int GetToppingCount() {
        return toppings.Count;
    }
    public List<Topping> GetToppings() {
        return toppings;
    }
}
