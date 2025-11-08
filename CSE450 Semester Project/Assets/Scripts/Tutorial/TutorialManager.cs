using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour {
    public GameObject tossInteract;
    public GameObject topInteract;
    public GameObject ovensInteract;
    public GameObject cutInteract;
    public GameObject submitInteract;

    public TMP_Text tutorialText;
    public TMP_Text skipButtonText;

    private DayManager dayManager;
    private CreatureMove cm;
    private CreatureAssign ca;
    public GameObject stationOrderUI;
    public GameObject currOrderUI;
    private GameObject tempGO;
    private int phase;


    public void EndTutorial() {
        GameObject.FindWithTag("MainCamera").transform.parent = null;
        Destroy(GameObject.FindWithTag("Player"));
        foreach (GameObject creature in GameObject.FindGameObjectsWithTag("Creature"))
        {
            Destroy(creature);
        }
        foreach (GameObject creature in GameManager.instance.GetRegisteredCreatures()) {
            Destroy(creature);
        }
        Destroy(GameManager.instance.gameObject);
        PlayerPrefs.SetInt("SkipTutorial", 1);
        SceneManager.LoadScene("MainKitchenScene");
    }

    void Start() {
        cm = GameObject.Find("CreatureHandler").GetComponent<CreatureMove>();
        ca = GameObject.Find("CreatureHandler").GetComponent<CreatureAssign>();
        dayManager = GameObject.Find(DayManager.dayManagerObjName).GetComponent<DayManager>();
        phase = 0;
        tossInteract.SetActive(false);
        topInteract.SetActive(false);
        ovensInteract.SetActive(false);
        cutInteract.SetActive(false);
        submitInteract.SetActive(false);

        tutorialText.gameObject.SetActive(true);
        tutorialText.text = "Welcome to Creature Kitchen Simulator Game! To start, upload a file to become your first creature!";

        GameManager.instance.SetBalance(500);
        GameManager.instance.fileUploadButton.onClick.AddListener(OnFirstFile);
        GameManager.instance.dayBeginButton.gameObject.SetActive(false);
    }
    
    void Update() {
        if (phase == 1 && Input.GetKeyDown(KeyCode.E)) {
            StartCoroutine(CheckCreatureUIAfterDelay());
        }
        if (phase == 2 && !cm.IsSelectedCreature(tempGO)) {
            tutorialText.text = "You can also drag and drop from one of your creatures to a station to assign it to that station. Try assigning your friend to a station (toss, top, ovens, or cut) by clicking and dragging.";
            phase = 3;
        }
        if (phase == 3 && ca.CreaturesAssigned() > 0) {
            phase = 4;
            tutorialText.text = "When a creature is assigned to a station, they will do the work required at that station. Otherwise they will stand idly by, content to be directionless. You can hold [Tab] to view your staff and their station assignments. Take back control of your other creature to continue.";
            tempGO = cm.selectedCreature;
        }
        
        if (phase == 4 && !cm.IsSelectedCreature(tempGO)) {
            tutorialText.text = "Now that you know how to control your staff, you can start making pizzas!";
            StartCoroutine(GenerateOrderAfterDelay());
            phase = -1;
        }
        
        if (phase == 5 && Input.GetKeyDown(KeyCode.E)) {
            StartCoroutine(CheckStationOrderUIAfterDelay());
        }
        if (phase == 6) {
            OrderTicket o = GameObject.FindWithTag("Player").GetComponentInChildren<OrderTicket>();
            if (o != null && o.GetOrder() != null) {
                phase = 7;
                tutorialText.text = "Great! The next step is tossing out the pie. For all stations, your stats will affect the difficulty of the minigame. Hold [Tab] to view your current staff's stats. Take control of the worker with the higher toss rating and interact with the toss station!";
                tossInteract.SetActive(true);
            }
        }
        
        if (phase == 7) {
            PizzaObject p = GameObject.FindWithTag("Player").GetComponentInChildren<PizzaObject>();
            if (p != null) {
                phase = 8;
                tutorialText.text = "Awesome! Next you have to add the toppings. Hold [ShiftR] to view your current order and pizza, then interact with the topping station. Remember you can view the topping skill of your creatures by holding [Tab]!";
                topInteract.SetActive(true);

            }
        }
        
        if (phase == 8) {
            PizzaObject p = GameObject.FindWithTag("Player").GetComponentInChildren<PizzaObject>();
            if (p != null && p.GetToppingCount() > 2) {
                phase = 9;
                tutorialText.text = "Now you have to cook the pizza. Hold [ShiftR] to view the target cook level of your order. The oven station is a little special, because it will cook your pie even when the minigame UI is not active. Cook your pizza to continue";
                ovensInteract.SetActive(true);
            }
        }
        
        if (phase == 9) {
            PizzaObject p = GameObject.FindWithTag("Player").GetComponentInChildren<PizzaObject>();
            if (p != null && p.GetAverageCookLevel() >= 90) {
                phase = 10;
                tutorialText.text = "Now that your pizza is cooked, the last step is to cut it. Remember, you can always hold [Tab] to see which creature is most skilled at each station.";
                cutInteract.SetActive(true);
            }
        }
        
        if (phase == 10) {
            PizzaObject p = GameObject.FindWithTag("Player").GetComponentInChildren<PizzaObject>();
            if (p != null && p.IsCut()) {
                phase = 11;
                tutorialText.text = "Awesome! You now have a completed pie. Submit your order at the [Submit] station to see your profit. Pizzas that are higher quality and more accurate to the order will receive higher tips.";
                submitInteract.SetActive(true);
            }
        }
        if (phase == 11) {
            if (GameManager.instance.GetProfit() > 0) {
                phase = 12;
                skipButtonText.text = "End Tutorial";
                tutorialText.text = "You now know the basics of running your own creature kitchen. Your goal is to make enough money each week to keep the pizzeria open. Find your favorite file workers and optimize your kitchen staff! Click [End Tutorial] to begin the game";
            }
        }
    }
    
    private IEnumerator CheckCreatureUIAfterDelay() {
        yield return new WaitForEndOfFrame();
        if (GameObject.Find("CreatureManagerUI").transform.GetChild(0).gameObject.activeSelf) {
            tutorialText.text = " This is also where you can upload more files to turn into creatures! Upload a second file to continues.";
        }
    }

    private IEnumerator GenerateOrderAfterDelay() {
        yield return new WaitForSeconds(3f);
        dayManager.AddOrder(new Order("Tutorial Order", 90, new List<Topping> { Topping.RedSauce, Topping.Mozzarella, Topping.Pepperoni }, 100, 90, 100));
        tutorialText.text = "Look! You have a new order. Go to the [Order] station and press [E] to view your active orders!";
        phase = 5;
    }
    
    private IEnumerator CheckStationOrderUIAfterDelay() {
        yield return new WaitForEndOfFrame();
        if (stationOrderUI.activeSelf) {
            phase = 6;
            tutorialText.text = "Here you can see all active orders and the time remaining on them. Click on the one order to claim it. View your current claimed order or owned pie by holding [Shift-R]";
        }
    }
    
    private IEnumerator CheckOrderUIAfterDelay() {
        yield return new WaitForEndOfFrame();
        if (currOrderUI.activeSelf) {
            phase = 8;
            tutorialText.text = "Here you can see all active orders and the time remaining on them. Click on the one order to claim it. View your current claimed order or owned pie by holding [Shift-R]";
        }
    }



    private void OnFirstFile() {
        if (GameObject.FindWithTag("Player") == null) { return; } // only do this if file uploaded
        phase = 1;
        GameObject.Find("CreatureManagerUI").transform.GetChild(0).gameObject.SetActive(false);
        GameManager.instance.EnableMovement();
        tutorialText.text = "This is your file as a creature! Use [WASD] or the arrow keys to move around. Go to the [Creatures] station and push [E] to view your creature's stats.";

        GameManager.instance.fileUploadButton.onClick.RemoveListener(OnFirstFile);
        GameManager.instance.fileUploadButton.onClick.AddListener(OnSecondFile);
    }
    
    private void OnSecondFile() {
        if (GameObject.FindWithTag("Creature") == null) { return; } // only do this if file uploaded
        phase = 2;
        tempGO = cm.selectedCreature;
        GameObject.Find("CreatureManagerUI").transform.GetChild(0).gameObject.SetActive(false);
        GameManager.instance.EnableMovement();
        tutorialText.text = "You now have two creatures! Click on a creature to switch control over to it.";
        
        GameManager.instance.fileUploadButton.onClick.RemoveListener(OnSecondFile);
    }

}
