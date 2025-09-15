using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// This class defines a pizza object in game
// A pizza object is initialized with no state besides toss quality
    // This class lets a pizza's state be viewed and updated

public class PizzaObject : MonoBehaviour {
    public Color baseColor = new Color(237, 219, 152); // yellowish
    private SpriteRenderer spr;

    private int tossQuality = -1;
    private List<Topping> toppings = new List<Topping>();
    private int[] cookAmount = new int[8]; // 8 slices... should directly reference some OvenGameManager var probably
    private int cutQuality = -1;

    // Start is called before the first frame update
    void Start() {
        spr = GetComponent<SpriteRenderer>();
        spr.color = baseColor;
    }

    // functions to update state
    public void InitializePizza(int tossQuality) {
        this.tossQuality = tossQuality;
    }
    public void CutPizza(int cutQuality) {
        this.cutQuality = cutQuality;
    }
    public void AddTopping(Topping t) {
        toppings.Add(t);
    }
    public void SetCookLevels(int[] cookLevels) {
        this.cookAmount = cookLevels;
    }


    // functions to view current state
    public bool IsCut() {
        return cutQuality >= 0;
    }
    public int GetToppingCount() {
        return toppings.Count;
    }
    public List<Topping> GetToppings() {
        return toppings;
    }
    public int[] GetCookLevels() {
        return cookAmount;
    }
    public double GetAverageCookLevel() {
        return cookAmount.ToArray().Average();
    }
    public float GetCookScore() {
        // TODO: should penalize for undercook and overcook
        // probably quadratic (e.g. if you're 20 off it should be more than twice as bad as )
        float score = 0;
        var weight = 100f / cookAmount.Length;
        foreach (int cookLvl in cookAmount) {
            // expected is 100, 
            score += Mathf.Max(100 -
                Mathf.FloorToInt(
                    Mathf.Pow(
                        Mathf.Abs(cookLvl - 100f)
                    , 2)
                 / 10f)
            , 0) * weight;
        }
        return score / 100f;
    }
    
    override public string ToString() {
        var pieStr = "Pizza Object\nToss Quality: " + tossQuality + "\nToppings: ";
        if(toppings.Count == 0) {
            pieStr += "[ None ]";
        } else {
            foreach(var t in toppings) {
                pieStr += t.ToString() + ", ";
            }
        }
        pieStr += "\nAvg Cook Level: " + GetAverageCookLevel()
            + "\nCook Score: " + GetCookScore()
            + "\nCut Quality: " + cutQuality;
        return pieStr;
    }
}
