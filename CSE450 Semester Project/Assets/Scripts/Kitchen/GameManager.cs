using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private const KeyCode creatureUIKey = KeyCode.Tab;
    private const KeyCode orderUIKey = KeyCode.RightShift;

    private const float creatureCostIncreaseRate = 50f; // how much more expensive each consecutive upload is

    public static string kitchenGameManager = "GameManager"; // name for other scripts to reference
    private int day = 0; // basically keeping track of some progression
    private float totalProfit = 0;
    private float currentBalance = 50;
    private List<GameObject> registeredCreatures = new List<GameObject>();

    private DayManager dayManager;
    private CreatureInfoUIController creatureUI;
    private OrderUIController orderUI;

    public Button fileUploadButton;
    public GameObject creatureUIInstructions;
    public Button dayBeginButton;
    public TMP_Text moneyTxt;


    void Start() {
        dayManager = GameObject.Find(DayManager.dayManagerObjName).GetComponent<DayManager>();
        creatureUI = this.GetComponent<CreatureInfoUIController>();
        orderUI = this.GetComponent<OrderUIController>();
        dayBeginButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "Begin Day " + day;
        dayBeginButton.gameObject.SetActive(false);
        UpdateFileUploadButton();
        UpdateMoneyLabel();

        // on new game, start by showing creature screen with upload button
        if (day == 0) {
            creatureUI.ShowCreatureInfo();
        } else {
            creatureUI.HideCreatureInfo();
            creatureUIInstructions.SetActive(false);
        }
    }

    void Update() {
        if (Input.GetKeyDown(creatureUIKey) && !Input.GetKey(orderUIKey)) {
            creatureUI.ShowCreatureInfo();
        }
        if (Input.GetKeyDown(orderUIKey) && !Input.GetKey(creatureUIKey) && !creatureUIInstructions.activeSelf) {
            orderUI.Show();
        }
        
        if (Input.GetKeyUp(creatureUIKey)) {
            creatureUI.HideCreatureInfo();
        }
        if (Input.GetKeyUp(orderUIKey)) {
            orderUI.Hide();
        }
    }
    
    public void UpdateMoneyLabel() {
        moneyTxt.text = "Balance: $" + System.Math.Round(currentBalance, 2)
            + "\nTotal Profit: $" + System.Math.Round(totalProfit, 2);
    }

    public void RegisterCreature(GameObject creature) {
        if (currentBalance >= GetCurrentUploadCost() && creature.GetComponent<CreatureStats>() != null) {
            // handle cost
            currentBalance -= GetCurrentUploadCost();

            // is valid creature
            GameObject creatureCopy = Instantiate(creature);
            creatureCopy.SetActive(false);
            registeredCreatures.Add(creatureCopy);
            
            if (day == 0) {
                dayBeginButton.gameObject.SetActive(true);
                creatureUIInstructions.SetActive(false);
                creatureUI.HideCreatureInfo();
            }
            UpdateFileUploadButton();
            UpdateMoneyLabel();
        }
    }
    public float GetCurrentUploadCost() {
        return creatureCostIncreaseRate * registeredCreatures.Count;
    }
    public void UpdateFileUploadButton() {
        var cost = GetCurrentUploadCost();
        if (cost > 0) {
            fileUploadButton.GetComponentInChildren<TMP_Text>().text = "Upload New File ($"
                + GetCurrentUploadCost() + ")";
        } else {
            fileUploadButton.GetComponentInChildren<TMP_Text>().text = "Upload New File (Free)";
        }
        fileUploadButton.interactable = cost <= currentBalance;
    }

    public void BeginDay() {
        dayBeginButton.gameObject.SetActive(false);
        dayManager.StartDay(day, currentBalance);
        DeactivateFileUpload();
    }

    // called by DayManager
    public void EndDay(float profit) {
        Debug.Log("Ended Day " + day + " with $" + profit + " profit");
        day++;
        totalProfit += profit;
        currentBalance += profit;

        dayBeginButton.GetComponentInChildren<TMP_Text>().text = "Begin Day " + day;
        dayBeginButton.gameObject.SetActive(true);
        // don't allow new upload if can't afford
        fileUploadButton.interactable = GetCurrentUploadCost() <= currentBalance;
        ActivateFileUpload();
        UpdateMoneyLabel();
    }

    public void GameOver() {
        // remove all creatures
        foreach (var creature in GameObject.FindGameObjectsWithTag("Creature")) {
            Destroy(creature);
        }
        // reset game day and balances
        day = 0;
        currentBalance = 0;
        totalProfit = 0;
        BeginDay();
    }

    public void ActivateFileUpload() {
        fileUploadButton.gameObject.SetActive(true);
    }

    public void DeactivateFileUpload() {
        // TODO: un-comment this, just for the purposes of testing station assignment
        fileUploadButton.gameObject.SetActive(false);
    }

    public void ToggleMovement() {
        GameObject.Find("CreatureHandler").GetComponent<CreatureMove>().ToggleMovement();
    }
    public void EnableMovement() {
        GameObject.Find("CreatureHandler").GetComponent<CreatureMove>().movementEnabled = true;
    }
    public void DisableMovement() {
        GameObject.Find("CreatureHandler").GetComponent<CreatureMove>().movementEnabled = false;
    }
}
