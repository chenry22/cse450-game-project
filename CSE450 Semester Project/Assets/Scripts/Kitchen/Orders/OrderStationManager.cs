using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// station manager, handles UI toggle and shifting between active orders
public class OrderStationManager : MonoBehaviour {
    public DayManager dayManager;

    private List<Order> activeOrders;
    private int orderEndIndex; // first active order listed, used for bounding left and right movement
    public OrderStationSelect[] orderSlots = new OrderStationSelect[3];

    private float timer = 0f;
    private float updateRate = 1f; // rate at which UI updates (basically showing time left on orders change)

    void Update() {
        if (gameObject.activeSelf) {
            if (timer > updateRate) {
                SetOrderSlots();
                timer = 0f;
            }
            timer += Time.deltaTime;
            
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                MoveOrderViewLeft();
            } else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
                MoveOrderViewRight();
            }
        }
    }
    
    private void SetOrderSlots() {
        for (int i = 0; i < orderSlots.Length; i++) {
            int orderIndex = Mathf.Max(0, orderEndIndex - orderSlots.Length + 1 + i);
            if (orderIndex < activeOrders.Count) {
                orderSlots[i].SetText(activeOrders[orderIndex].ToString());
                orderSlots[i].gameObject.SetActive(true);
            } else {
                orderSlots[i].gameObject.SetActive(false);
            }
        }
    }
    public void SelectOrder(int index) {
        // TODO: if holding pizza, just link to pizza
        // else if holding ticket, update
        // else create ticket object
        CloseOrderUI();
    }

    public void ShowOrderUI() {
        // lock movement
        GameObject.Find(GameManager.kitchenGameManager).GetComponent<GameManager>().ToggleMovement();
        // load current orders
        activeOrders = dayManager.GetActiveOrders();
        orderEndIndex = Mathf.Min(activeOrders.Count - 1, orderSlots.Length - 1);
        SetOrderSlots();
        timer = 0f;
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
