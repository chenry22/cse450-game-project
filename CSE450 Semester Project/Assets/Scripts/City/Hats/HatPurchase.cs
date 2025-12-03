using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HatPurchase : MonoBehaviour
{
    private SpriteRenderer spr;
    private Color defaultColor = new Color(1f, 1f, 1f, 0.1f);
    private Color triggeredColor = new Color(1f, 0, 0, 0.2f);

    public TMP_Text instruct;
    public GameObject hat;
    public string hatID;
    public int price;

    // Start is called before the first frame update
    void Start() {
        spr = this.GetComponent<SpriteRenderer>();
        spr.color = defaultColor;
        instruct.gameObject.SetActive(false);
    }

    void Update() {
        if (instruct.gameObject.activeSelf && Input.GetKeyDown(KeyCode.E)) {
            if (GameManager.instance.HatUnlocked(hatID)) {
                var player = GameObject.FindWithTag("Player");
                // remove any existing hat
                for(int i = 0; i < player.transform.childCount; i++) {
                    if (player.transform.GetChild(i).tag == "Hat") {
                        Destroy(player.transform.GetChild(i).gameObject);
                    }
                }
                // put hat on
                Instantiate(hat, GameObject.FindWithTag("Player").transform);
            } else if (price <= GameManager.instance.GetBalance()) {
                GameManager.instance.UnlockHat(hatID);
                var player = GameObject.FindWithTag("Player");
                // remove any existing hat
                for(int i = 0; i < player.transform.childCount; i++) {
                    if (player.transform.GetChild(i).tag == "Hat") {
                        Destroy(player.transform.GetChild(i).gameObject);
                    }
                }
                Instantiate(hat, player.transform);
                GameManager.instance.SetBalance(GameManager.instance.GetBalance() - price);
                GameObject.Find("BalanceText").GetComponent<TMP_Text>().text = "Balance: $" + GameManager.instance.GetBalance();
                instruct.text = "Hat purchased!";
            } else {
                instruct.text = "You can't afford this hat";
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player") {
            if (GameManager.instance.HatUnlocked(hatID)) {
                instruct.text = "[E] to wear hat";
            } else {
                instruct.text = "[E] to buy hat ($" + price + ")";
            }
            instruct.gameObject.SetActive(true);
            spr.color = triggeredColor; 
        }
    }
    void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag == "Player") {
            instruct.gameObject.SetActive(false);
            spr.color = defaultColor;
        }
    }
}
