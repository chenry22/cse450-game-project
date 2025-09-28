using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class OrderResult {
    private float tip;
    private float pizzaCost;

    public OrderResult(float tip, float pizzaCost) {
        this.tip = tip;
        this.pizzaCost = pizzaCost;
    }
    public float GetTip() { return tip; }
    public float GetPizzaCost() { return pizzaCost; }
    public float GetProfit() { return GetPizzaCost() + GetTip(); }
}

public class Order
{
    private const float baseProfit = 15f;
    private const float baseTip = 5f;

    private string orderLabel = "Order"; // TODO: idk could be a generated person name or just increment?
    private int targetTossQuality;
    private List<Topping> targetToppings;
    private int targetCookAmount;
    private int targetCutQuality;

    private float timeAllowed;
    private float timeActive;

    public Order(string label, int toss, List<Topping> toppings, int cook, int cut, float time) {
        this.orderLabel = label;
        this.targetTossQuality = toss;
        this.targetToppings = toppings;
        this.targetCookAmount = cook;
        this.targetCutQuality = cut;

        this.timeAllowed = time;
        timeActive = 0f;
    }

    // basically just a way for a manager script to tell order timer to tick
    public void IncreaseTime(float t) {
        timeActive += t;
    }

    /// <summary>
    /// Takes in an active PizzaObject and returns the profit 
    /// </summary>
    /// <returns>profit as a float</returns>
    public OrderResult SubmitOrder(PizzaObject p) {
        float profit = baseProfit;
        profit += targetToppings.Count; // more toppings means more spensive

        float tossTipRate = (100f - Math.Max(0, targetTossQuality - p.GetTossQuality())) / 100f;
        float topTipRate = 0f;

        List<Topping> toppingsCopy = p.GetToppings();
        float rateChange = 1f / toppingsCopy.Count;
        foreach (Topping t in targetToppings) {
            if (toppingsCopy.Remove(t)) {
                topTipRate += rateChange;
            }
        }
        topTipRate -= rateChange * toppingsCopy.Count; // penalty for wrong extra stuff
        topTipRate = Math.Max(0, topTipRate);

        float cookTipRate = p.GetCookScore(targetCookAmount) / 100f;
        float cutTipRate = (100f - Math.Max(0, targetTossQuality - p.GetTossQuality())) / 100f;

        // for timing penalty/reward, my thought is if you're on time, that's a tip rate of 1.0
        // if you're early, the % you're early by is added
        //      e.g. if you take half as much time as allowed, your rate is 1.5f
        // if you're late, the % you're late by is subtracted
        //      e.g. if you take twice as long, your rate is 0f;
        float timeDiff = (timeAllowed - timeActive) / timeAllowed;
        float timeTipRate = Math.Max(0f, 1f + timeDiff);

        float tip = baseTip * tossTipRate * topTipRate * cookTipRate * cutTipRate * timeTipRate;
        tip = (float)Math.Round(tip, 2);
        return new OrderResult(tip, profit);
    }

    override public string ToString() {
        return "<b>" + orderLabel + "</b>"
            + "\nToss: " + targetTossQuality
            + "\nTop: " + string.Join(", ", targetToppings.ToArray())
            + "\nCook: " + targetCookAmount + " | Cut: " + targetCutQuality
            + "\nTime Left: " + ((int)(timeAllowed - timeActive));
    }
    public string ToStringFull() {
        return "<b>-" + orderLabel + "-</b>"
            + "\n<u>Toss</u>: " + targetTossQuality
            + "\n<u>Top</u>: " + string.Join(", ", targetToppings.ToArray())
            + "\n<u>Cook</u>: " + targetCookAmount
            + "\n<u>Cut</u>: " + targetCutQuality
            + "\n<u>Time Left</u>: " + ((int)(timeAllowed - timeActive));
    }
}
