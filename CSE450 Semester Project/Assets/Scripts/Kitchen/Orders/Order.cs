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

    public override string ToString() {
        return "Pizza: $" + Math.Round(GetPizzaCost(), 2)
            + " | Tip: $" + Math.Round(GetTip(), 2)
            + "\nTotal: $" + Math.Round(GetProfit(), 2);
    }
}

public class Order {
    private const float baseProfit = 15f;
    private const float baseTip = 5f;

    private const float notCutPenalty = 5f;
    private const float maxNotCookedPenalty = 10f;
    private const float cookedBaseline = 40f; // how much the pizza has to be cooked to be consider "acceptable" (no penalty)

    private string orderLabel = "Order"; // TODO: idk could be a generated person name or just increment?
    private int targetTossQuality;
    private List<Topping> targetToppings;
    private int targetCookAmount;
    private int targetCutQuality;

    private float timeAllowed;
    private float timeActive;

    private bool completed;

    public Order(string label, int toss, List<Topping> toppings, int cook, int cut, float time) {
        this.orderLabel = label;
        this.targetTossQuality = toss;
        this.targetToppings = toppings;
        this.targetCookAmount = cook;
        this.targetCutQuality = cut;

        this.timeAllowed = time;
        timeActive = 0f;
        completed = false;
    }
    
    public bool IsCompleted() { return completed; }

    // basically just a way for a manager script to tell order timer to tick
    public void IncreaseTime(float t) {
        if (completed) { return;  }
        timeActive += t;
    }

    /// <summary>
    /// Takes in an active PizzaObject and returns the profit 
    /// </summary>
    /// <returns>profit as a float</returns>
    public OrderResult SubmitPizza(PizzaObject p) {
        this.completed = true;
        float profit = baseProfit;
        float tossTipRate = (100f - Mathf.Max(0, targetTossQuality - p.GetTossQuality())) / 100f;

        // toppings
        float topTipRate = 0f;
        List<Topping> toppingsCopy = p.GetToppings();
        if (toppingsCopy.Count > 0) {
            float rateChange = 1f / toppingsCopy.Count;
            foreach (Topping t in targetToppings) {
                if (toppingsCopy.Remove(t)) {
                    profit++; // matched toppings means charge more
                    topTipRate += rateChange;
                }
            }
            topTipRate -= rateChange * toppingsCopy.Count; // penalty for wrong extra stuff
            topTipRate = Mathf.Max(0, topTipRate);
        } else {
            topTipRate = 1f;
        }

        // cook level
        float avgCookLevel = p.GetAverageCookLevel();
        float cookTipRate = 0f;
        if (avgCookLevel < cookedBaseline) {
            profit -= maxNotCookedPenalty * (cookedBaseline - avgCookLevel) / cookedBaseline;
        } else {
            cookTipRate = p.GetCookScore(targetCookAmount) / 100f;
        }

        // cut
        float cutTipRate = 0f;
        if (!p.IsCut()) {
            profit = Mathf.Max(0, profit - notCutPenalty);
        } else {
            cutTipRate = (100f - Math.Max(0, targetTossQuality - p.GetTossQuality())) / 100f;
        }

        // for timing penalty/reward, my thought is if you're on time, that's a tip rate of 1.0
        // if you're early, the % you're early by is added
        //      e.g. if you take half as much time as allowed, your rate is 1.5f
        // if you're late, the % you're late by is subtracted
        //      e.g. if you take twice as long, your rate is 0f;
        float timeDiff = (timeAllowed - timeActive) / timeAllowed;
        float timeTipRate = Math.Max(0f, 1f + timeDiff);

        Debug.Log("base tip: " + baseTip + ", toss: " + tossTipRate + ", top: " + topTipRate
            + ", cook: " + cookTipRate + ", cut: " + cutTipRate + ", time: " + timeTipRate);
        float tip = baseTip * tossTipRate * topTipRate * cookTipRate * cutTipRate * timeTipRate;
        return new OrderResult(tip, profit);
    }

    override public string ToString() {
        if (completed) { return "[ None ]"; }
        return "<b>" + orderLabel + "</b>"
            + "\nToss: " + targetTossQuality
            + "\nTop: " + string.Join(", ", targetToppings.ToArray())
            + "\nCook: " + targetCookAmount + " | Cut: " + targetCutQuality
            + "\nTime Left: " + ((int)(timeAllowed - timeActive));
    }
    public string ToStringFull() {
        if (completed) { return "[ None ]"; }
        return "<b>-" + orderLabel + "-</b>"
            + "\n<u>Toss</u>: " + targetTossQuality
            + "\n<u>Top</u>: " + string.Join(", ", targetToppings.ToArray())
            + "\n<u>Cook</u>: " + targetCookAmount
            + "\n<u>Cut</u>: " + targetCutQuality
            + "\n<u>Time Left</u>: " + ((int)(timeAllowed - timeActive));
    }
}
