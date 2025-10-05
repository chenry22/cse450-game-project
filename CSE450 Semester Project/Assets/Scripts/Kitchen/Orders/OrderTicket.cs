using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// a wrapper for an Order object, used when assigning pizzas to orders
// basically needed so that orders can be "claimed" or "held" without an active pizza object
// just gives the player a little more freedom to plan or handle linking pizzas to orders
public class OrderTicket : MonoBehaviour {
    private Order order;
    
    public void SetOrder(Order order) { this.order = order;  }
    public Order GetOrder() { return order; }
}
