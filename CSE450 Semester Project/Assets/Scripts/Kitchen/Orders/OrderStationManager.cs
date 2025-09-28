using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// station manager, handles UI toggle and shifting between active orders
public class OrderStationManager : MonoBehaviour {
    private List<Order> activeOrders;
    private int orderEndIndex; // first active order listed, used for bounding left and right movement
    public OrderStationSelect[] orderSlots = new OrderStationSelect[3];
    public GameObject orderTicketPrefab;

    private float timer = 0f;
    private float updateRate = 1f; // rate at which UI updates (basically showing time left on orders change)

    void Update() {
        if (gameObject.activeSelf) {
            if (timer > updateRate) {
                activeOrders = GameObject.Find(DayManager.dayManagerObjName).GetComponent<DayManager>().GetActiveOrders();
                SetOrderSlots();
                timer = 0f;
            } else {
                timer += Time.deltaTime;
            }
            
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                MoveOrderViewLeft();
            } else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
                MoveOrderViewRight();
            }
        }
    }
    
    private void SetOrderSlots() {
        for (int i = 0; i < orderSlots.Length; i++) {
            int orderIndex = Mathf.Max(0, orderEndIndex - orderSlots.Length + 1) + i;
            if (orderIndex < activeOrders.Count) {
                orderSlots[i].SetOrder(activeOrders[orderIndex]);
                orderSlots[i].gameObject.SetActive(true);
            } else {
                orderSlots[i].gameObject.SetActive(false);
            }
        }
    }
    
    // called when a player clicks on a certain order to take control of
    public void SelectOrder(Order order) {
        GameObject player = GameObject.FindWithTag("Player");
        var playerTicket = player.GetComponentInChildren<OrderTicket>();
        if (playerTicket != null) {
            playerTicket.SetOrder(order);
        } else {
            GameObject orderTicket = Instantiate(orderTicketPrefab);
            orderTicket.transform.parent = player.transform;
            orderTicket.transform.localPosition = Vector2.zero;
            orderTicket.GetComponent<OrderTicket>().SetOrder(order);
        }
        CloseOrderUI();
    }

    public void ShowOrderUI() {
        // lock movement
        GameObject.Find(GameManager.kitchenGameManager).GetComponent<GameManager>().ToggleMovement();
        activeOrders = GameObject.Find(DayManager.dayManagerObjName).GetComponent<DayManager>().GetActiveOrders();
        orderEndIndex = Mathf.Min(activeOrders.Count - 1, orderSlots.Length - 1);
        // trigger immediate load of orders
        timer = updateRate;
        // then actually show UI
        this.gameObject.SetActive(true);
    }
    public void CloseOrderUI() {
        GameObject.Find(GameManager.kitchenGameManager).GetComponent<GameManager>().ToggleMovement();
        this.gameObject.SetActive(false);
    }

    private void MoveOrderViewRight() {
        if (orderEndIndex < activeOrders.Count - 1 + orderSlots.Length - 1) {
            orderEndIndex++;
            SetOrderSlots();
        }
    }
    private void MoveOrderViewLeft() {
        if (orderEndIndex >= orderSlots.Length) {
            orderEndIndex--;
            SetOrderSlots();
        }
    }
}
