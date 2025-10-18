using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DayManager : MonoBehaviour {
    public static string dayManagerObjName = "DayManager"; // name for other scripts to reference
    public const float profitThreshold = 20f;   // minimum profit required to continue the game


    // consts defining gameplay
    private const int baseNumOrders = 4;
    private const float extraOrdersPerDay = 1 / 3; // will take FLOOR of computed val
    // e.g. if 1/3, extra order required every 3 days
    private const float baseOrderInterval = 80f; // in seconds
    private const float orderIntervalDecreasePerDay = 0.95f; // multiplified to default
                                                             // e.g. if 0.9f, day 2 will have interval of (defaultOrderInterval * 0.9f * 0.9f)
    private const float baseOrderTimeAllowed = 170f; // in seconds
    private const float orderTimeAllowedDecreasePerDay = 0.5f; // in seconds

     
    // order generation vars
    private const int minTossQuality = 82;
    private const int maxTossQuality = 100;
    private const int maxNumToppings = 3;
    private int[] cookLevels = new int[] { 90, 95, 100, 105, 110 };
    private const int minCutQuality = 82;
    private const int maxCutQuality = 100;


    private bool dayActive = false;
    private int numOrders;
    private float profit;
    private float balance;

    private List<Order> activeOrders;
    private List<Order> completedOrders;
    private float orderTimer;
    private float orderInterval;
    private float orderTimeAllowed;

    public TMP_Text moneyText;
    public TMP_Text orderNotifyText;
    

    public List<Order> GetActiveOrders() { return activeOrders; }
    public bool DayIsActive() { return dayActive; }
    
    public void StartDay(int day, float balance) {
        Debug.Log("Starting day " + day);
        this.balance = balance;
        this.numOrders = baseNumOrders + (int)(day * extraOrdersPerDay);
        this.orderInterval = baseOrderInterval * Mathf.Pow(orderIntervalDecreasePerDay, day);
        this.orderTimeAllowed = baseOrderTimeAllowed - (day * orderTimeAllowedDecreasePerDay);

        // destroy all active pies (don't let player set up to "cheat" day system)
        // but we should still allow practice/intermediate work if desired
        foreach (GameObject pie in GameObject.FindGameObjectsWithTag("Pizza")) {
            Destroy(pie);
        }

        activeOrders = new List<Order>();
        completedOrders = new List<Order>();
        profit = 0;
        orderTimer = orderInterval * 9f / 10f;
        dayActive = true;
    }
    public void EndDay() {
        dayActive = false;

        // check if restaurant met the profit threshold ($50)
        if (profit < profitThreshold) {
            Debug.Log("GAME OVER: Your restaurant is not profitable. All employees have quit.");
            moneyText.text = "Daily Profit: $0\nBalance: $0";
            profit = 0;
            balance = 0;

            GameObject.Find(GameManager.kitchenGameManager).GetComponent<GameManager>().GameOver();
        } else {
            // continue to next day
            GameObject.Find(GameManager.kitchenGameManager).GetComponent<GameManager>().EndDay(profit);
        }
    }

    private Order GenerateRandomOrder() {
        int targetTossQuality = RandomBellCurve(minTossQuality, maxTossQuality);
        List<Topping> targetToppings = new List<Topping> {
            ToppingMethods.GetRandomBase(),
            ToppingMethods.GetRandomSecondaryBase()
        };
        
        for (int i = 0; i < Random.Range(0, maxNumToppings + 1); i++) {
            targetToppings.Add(ToppingMethods.GetRandomNonbaseTopping());
        }

        int targetCookAmount = cookLevels[RandomBellCurve(0, cookLevels.Length)];
        int targetCutQuality = RandomBellCurve(minCutQuality, maxCutQuality);

        // TODO: maybe we want to add a cool random order label here, or maybe not
        Order newOrder = new Order(
            "Order " + (activeOrders.Count + completedOrders.Count + 1),
            targetTossQuality, targetToppings,
            targetCookAmount, targetCutQuality,
            orderTimeAllowed
        );
        activeOrders.Add(newOrder);
        // Debug.Log("New random order generated: \nToss: " + targetTossQuality
        //     + "\nTop: " + targetToppings + "\nCook Lvl: " + targetCookAmount
        //     + "\nCut: " + targetCutQuality + "\nTime: " + orderTimeAllowed);
        return newOrder;
    }

    public OrderResult SubmitOrderWithPizza(Order o, PizzaObject p)
    {
        OrderResult or = o.SubmitPizza(p);
        Destroy(p.gameObject); // this pizza gets "used" if successfully submitted

        // TODO: player should see this result somewhere?
        Debug.Log("Pizza: $" + or.GetPizzaCost() + "\nTip: $" + or.GetTip() + "\nTotal: $" + or.GetProfit());
        completedOrders.Add(o);
        activeOrders.Remove(o);
        GameObject.Find(GameManager.kitchenGameManager).GetComponent<GameManager>().UpdateOrdersCompleted(completedOrders.Count, numOrders);
        profit += or.GetProfit();
        moneyText.text = "Daily Profit: $" + System.Math.Round(profit, 2)
                + "\nBalance: $" + System.Math.Round(balance, 2);

        if (activeOrders.Count == 0 && completedOrders.Count >= numOrders)
        {
            EndDay();
        }
        return or;
    }
    
    public int GetNumOrders() { return numOrders; }

    // we want generated numbers to tend away from extremes
    private int RandomBellCurve(int min, int max) {
        return Mathf.RoundToInt((Random.Range(min, max) + Random.Range(min, max)) / 2f);
    }

    void Update() {
        if (dayActive) {
            foreach(Order o in activeOrders) {
                o.IncreaseTime(Time.deltaTime); // keep track of time while game is active
            }

            if (completedOrders.Count + activeOrders.Count < numOrders) {
                orderTimer += Time.deltaTime;
                if(orderTimer >= orderInterval) {
                    Order newOrder = GenerateRandomOrder();
                    var orderStation = orderNotifyText.transform.parent.GetComponentInChildren<StationInteract>();

                    // notify creature handler
                    GameObject.Find("CreatureHandler").GetComponent<CreatureAssign>().HandleNewOrder(newOrder, orderStation);

                    // notify order station
                    orderNotifyText.text = "<b>[ NEW ORDER ]</b>";
                    orderNotifyText.gameObject.SetActive(true);
                    orderTimer = 0f;
                }
            }
        }
    }
}
