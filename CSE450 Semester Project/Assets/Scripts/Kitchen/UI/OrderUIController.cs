using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OrderUIController : MonoBehaviour {
    private const float updateRate = 0.5f; // rate at which UI is updated (for order timer)

    public GameObject orderUI;
    public GameObject orderToPizzaLink;
    public TMP_Text currOrderTxt;
    public TMP_Text currPizzaTxt;
    public GameObject orderTicketPrefab;

    private PizzaObject currPie;
    private OrderTicket currTicket;
    private float timer = 0f;
    
    public void LinkComponents()
    {
        orderUI = GameObject.Find("GameUI").transform.GetChild(5).gameObject;
        orderToPizzaLink = orderUI.transform.GetChild(0).GetChild(0).GetChild(3).gameObject;
        currOrderTxt = orderUI.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>();
        currPizzaTxt = orderUI.transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<TMP_Text>();
    }

    void Start() { Hide(); }

    void Update() {
        if (orderUI != null && orderUI.activeSelf) {
            if (timer >= updateRate) {
                Order currOrder = currPie?.GetLinkedOrder();
                if (currOrder != null) {
                    currOrderTxt.text = currOrder.ToStringFull();
                } else if (currTicket?.GetOrder() != null) {
                    currOrderTxt.text = currTicket.GetOrder().ToStringFull();
                }
                timer = 0f;
            } else {
                timer += Time.deltaTime;
            }
            
            // links or unlinks order to current pizza
            if (Input.GetKeyDown(KeyCode.Return) && currPie != null) {
                if (orderToPizzaLink.activeSelf && currPie.GetLinkedOrder() != null) {
                    // unlink order
                    if (currTicket == null) {
                        GameObject player = GameObject.FindWithTag("Player");
                        GameObject newTicket = Instantiate(orderTicketPrefab);
                        newTicket.transform.parent = player.transform;
                        newTicket.transform.localPosition = Vector2.zero;
                        currTicket = newTicket.GetComponent<OrderTicket>();
                    }
                    currTicket.SetOrder(currPie.GetLinkedOrder());
                    currPie.LinkOrder(null);
                    orderToPizzaLink.SetActive(false);
                } else if (currTicket?.GetOrder() != null) {
                    currPie.LinkOrder(currTicket.GetOrder());
                    currTicket.SetOrder(null);
                    orderToPizzaLink.SetActive(true);
                }
            }
        }
    }

    public void Show() {
        GameObject player = GameObject.FindWithTag("Player");
        currPie = player?.GetComponentInChildren<PizzaObject>();
        currTicket = player?.GetComponentInChildren<OrderTicket>();
        Order currOrder = currPie?.GetLinkedOrder();
        if (currOrder != null) {
            currOrderTxt.text = currOrder.ToStringFull();
            orderToPizzaLink.SetActive(true);
        } else if (currTicket?.GetOrder() != null) {
            currOrderTxt.text = currTicket.GetOrder().ToStringFull();
            orderToPizzaLink.SetActive(false);
        } else {
            currOrderTxt.text = "<b>-Current Order-</b>\n\n  [ None ]";
            orderToPizzaLink.SetActive(false);
        }
        currPizzaTxt.text = currPie == null ? "<b>-Current Pizza-</b>\n\n  [ None ]" : currPie.ToStringWithLabel("<b>-Current Pizza-</b>");
        orderUI.SetActive(true);
    }
    public void Hide() {
        orderUI.SetActive(false);
    }
}
