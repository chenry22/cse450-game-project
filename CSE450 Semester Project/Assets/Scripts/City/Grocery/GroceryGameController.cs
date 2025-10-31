using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GroceryGameController : MonoBehaviour {

    public RectTransform listDropdown;
    public TMP_Text[] shelfLabels;
    public TMP_Text timeTxt;

    public int shelfCapacity = 4;
    public float shelfUpdateTime = 10f;
    public float shoppingTime = 30f;

    private float timeRemaining;
    private int[] toppingsRemaining;

    void Start() {
        LoadShelfToppings();
        StartCoroutine(LoadShelfToppingLoop());
    }
    
    private IEnumerator LoadShelfToppingLoop() {
        yield return new WaitForSeconds(shelfUpdateTime);
        if (timeRemaining > 0f) {
            LoadShelfToppings();
            StartCoroutine(LoadShelfToppingLoop());
        }
    }
    
    private void LoadShelfToppings() {
        for (int i = 0; i < shelfLabels.Length; i++) {
            toppingsRemaining[i] = shelfCapacity;
            shelfLabels[i].text = ToppingMethods.ToString(ToppingMethods.GetRandomTopping()) + " " + shelfCapacity + "/" + shelfCapacity;
        }
    }

    void Update() {
        if (timeRemaining >= 0f) {
            timeRemaining -= Time.deltaTime;
        } else {
            // end game...
        }
    }
}
