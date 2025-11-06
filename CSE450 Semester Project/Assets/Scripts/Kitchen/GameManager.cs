using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    private const KeyCode creatureUIKey = KeyCode.Tab;
    private const KeyCode orderUIKey = KeyCode.RightShift;

    private const int maxActiveCreatures = 6;
    private const int startingBalance = 50;
    private const float creatureCostIncreaseRate = 50f; // how much more expensive each consecutive upload is


    public static GameManager instance;
    private int day = 0; // basically keeping track of some progression
    private float totalProfit = 0;
    private float currentBalance = startingBalance; // start with 50 so you can buy a guy on day 0 if you want :)
    private List<GameObject> registeredCreatures = new List<GameObject>();

    private DayManager dayManager;
    private CreatureInfoUIController creatureUI;
    private OrderUIController orderUI;

    public Button fileUploadButton;
    public Button dayBeginButton;
    public GameObject managerWall; // blocks kitchen from manager 
    public TMP_Text moneyTxt;
    public TMP_Text dayTxt;
    public TMP_Text ordersCompletedTxt;


    void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        }

        // always replace but do the starting stuff
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name == "MainKitchenScene") {
            LoadKitchenScene();
        }
    }

    void LoadKitchenScene() {
        dayManager = GameObject.Find(DayManager.dayManagerObjName).GetComponent<DayManager>();
        creatureUI = this.GetComponent<CreatureInfoUIController>();
        orderUI = this.GetComponent<OrderUIController>();

        fileUploadButton = GameObject.FindWithTag("FileUpload").GetComponent<Button>();
        dayBeginButton = GameObject.FindWithTag("BeginDay").GetComponent<Button>();
        managerWall = GameObject.Find("ManagerRoomWall");
        moneyTxt = GameObject.Find("moneyTxt")?.GetComponent<TMP_Text>();

        DeactivateBeginDayButton();
        UpdateFileUploadButton();
        UpdateMoneyLabel();
        managerWall.SetActive(false);

        if (registeredCreatures.Count > 0) {
            creatureUI.LinkComponents();
            orderUI.LinkComponents();
            var mon = Instantiate(registeredCreatures[0].gameObject, GameObject.Find("Spawner").transform);
            mon.transform.localPosition = new Vector3(Random.Range(-0.5f, .5f), Random.Range(-0.5f, .5f), 0);
            mon.SetActive(true);
            mon.GetComponent<CreatureSelect>().CopyCreature(registeredCreatures[0].GetComponent<CreatureSelect>());
            mon.GetComponent<CreatureSelect>().SelectCreature();
            GameObject.Find("CreatureManagerUI").GetComponent<CreatureManagerUIController>().HideCreatureUI();
        }
    }

    void Update() {
        // do the same for the other scenes
        // this fixes the game crash null references

        // only allow kitchen UI toggles when the MainKitchenScene is active
        if (SceneManager.GetActiveScene().name == "MainKitchenScene")
        {
            if (Input.GetKeyDown(creatureUIKey) && !Input.GetKey(orderUIKey))
            {
                creatureUI.ShowCreatureInfo();
            }
            if (Input.GetKeyDown(orderUIKey) && !Input.GetKey(creatureUIKey))
            {
                orderUI.Show();
            }

            if (Input.GetKeyUp(creatureUIKey))
            {
                creatureUI.HideCreatureInfo();
            }
            if (Input.GetKeyUp(orderUIKey))
            {
                orderUI.Hide();
            }
        }

    }
    
    public void UpdateMoneyLabel() {
        moneyTxt.text = "Balance: $" + System.Math.Round(currentBalance, 2)
            + "\nTotal Profit: $" + System.Math.Round(totalProfit, 2);
    }
    public void ActivateBeginDayButton() { 
        if (dayManager.DayIsActive()) { return; }
        dayBeginButton.GetComponentInChildren<TMP_Text>().text = "Begin Day " + (day + 1);
        dayBeginButton.gameObject.SetActive(true);
    }
    public void DeactivateBeginDayButton() {
        dayBeginButton.gameObject.SetActive(false);
    }

    public void RegisterCreature(GameObject creature) {
        if (creature.GetComponent<CreatureStats>() != null) {
            var cs = creature.GetComponent<CreatureSelect>();
            // handle cost
            currentBalance -= GetCurrentUploadCost();

            // is valid creature
            GameObject creatureCopy = Instantiate(creature);
            DontDestroyOnLoad(creatureCopy);
            creatureCopy.SetActive(false);
            creatureCopy.GetComponent<CreatureSelect>().CopyCreature(cs);
            registeredCreatures.Add(creatureCopy);

            // if more than 6 active creatures, push this one to storage
            if (GameObject.FindGameObjectsWithTag("Creature").Length >= maxActiveCreatures) {
                Destroy(creature);
            }

            if (registeredCreatures.Count == 1) {
                // auto take control of just first upload
                cs.SelectCreature();
            }
            GameObject.Find(CreatureManagerUIController.sceneName).GetComponent<CreatureManagerUIController>().LoadUI();
            UpdateFileUploadButton();
            UpdateMoneyLabel();
        }
    }
    
    public List<GameObject> GetRegisteredCreatures() {
        return registeredCreatures;
    }
    public int GetDay() { return day; }
    public float GetBalance() { return currentBalance; }
    public float GetProfit() { return totalProfit; }
    
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
        DeactivateBeginDayButton();
        managerWall.SetActive(true);
        dayManager.StartDay(day, currentBalance);
    }

    // called by DayManager
    public void EndDay(float profit) {
        Debug.Log("Ended Day " + day + " with $" + profit + " profit");
        day++;
        totalProfit += profit;
        currentBalance += profit;

        ActivateBeginDayButton();
        managerWall.SetActive(false);
        // don't allow new upload if can't afford
        fileUploadButton.interactable = GetCurrentUploadCost() <= currentBalance;
        UpdateMoneyLabel();
        dayTxt.text = "Day " + (day + 1);
    }

    public void GameOver()
    {
        // remove all creatures
        foreach (var creature in GameObject.FindGameObjectsWithTag("Creature"))
        {
            registeredCreatures.Remove(creature);
            Destroy(creature);
        }

        // reset game day and balances
        day = 0;
        currentBalance = startingBalance;
        totalProfit = 0;

        ActivateBeginDayButton();
        UpdateFileUploadButton();
        UpdateMoneyLabel();
    }
    
    public void UpdateOrdersCompleted(int completed, int total) {
        ordersCompletedTxt.text = completed + "/" + total + " orders completed";
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
