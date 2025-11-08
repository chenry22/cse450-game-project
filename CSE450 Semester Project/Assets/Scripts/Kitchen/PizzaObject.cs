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

    private Order linkedOrder = null;
    private int tossQuality = -1;
    private List<Topping> toppings = new List<Topping>();
    private int[] cookAmount = new int[8]; // 8 slices... should directly reference some OvenGameManager var probably
    private int cutQuality = -1;

    // Start is called before the first frame update
    void Start() {
        spr = GetComponent<SpriteRenderer>();
        spr.color = baseColor;
    }
    
    public void LinkOrder(Order o) { linkedOrder = o; }
    public Order GetLinkedOrder() { return linkedOrder; }

    // functions to update state
    public void InitializePizza(int tossQuality) {
        this.tossQuality = Mathf.Max(0, tossQuality);
    }
    public void CutPizza(int cutQuality) {
        this.cutQuality = Mathf.Max(0, cutQuality);
    }
    public void AddTopping(Topping t) {
        toppings.Add(t);
    }
    public void SetCookLevels(int[] cookLevels) {
        this.cookAmount = cookLevels;
    }


    // functions to view current state
    public int GetTossQuality() { return tossQuality; }
    public bool IsCut() {
        return cutQuality >= 0;
    }
    public int GetToppingCount() {
        return toppings.Count;
    }
    public List<Topping> GetToppings() { return toppings; }
    public int[] GetCookLevels() { return cookAmount; }
    public float GetAverageCookLevel() {
        return (float)cookAmount.ToArray().Average();
    }
    /// <summary>
    /// Computes a score for the pizza's cook level based on some expected amount
    /// </summary>
    /// <param name="targetCookAmount">The target cook amount from 80-120, where 100 is a normal cook amount</param>
    /// <returns>The cook score as a decimal / 1.0f</returns>
    public float GetCookScore(float targetCookAmount = 100f) {
        targetCookAmount = Mathf.Min(Mathf.Max(80f, targetCookAmount), 120f); // must be within 80-120
        float score = 0;
        float weight = 100f / cookAmount.Length;
        foreach (int cookLvl in cookAmount) {
            score += Mathf.Max(targetCookAmount -
                Mathf.FloorToInt(
                    Mathf.Pow(
                        Mathf.Abs(cookLvl - targetCookAmount)
                    , 2)
                 / 10f)
            , 0) * weight;
        }
        return score / 100f;
    }
    public float GetCutQuality() { return cutQuality; }
    
    
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
    public string ToStringWithLabel(string label) {
        var pieStr = label + "\n<u>Toss</u>: " + tossQuality + "\n<u>Top</u>: ";
        if(toppings.Count == 0) {
            pieStr += "[ None ]";
        } else {
            foreach(var t in toppings) {
                pieStr += t.ToString() + ", ";
            }
        }
        pieStr += "\n<u>Cook</u>: " + GetAverageCookLevel()
            + "\n<u>Cut</u>: " + (IsCut() ? cutQuality : "[ Not cut ]");
        return pieStr;
    }
}
