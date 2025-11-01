using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// manages the UI overlay and game mechanics of the top minigame

// TODO: if a pizza is linked to an order, this should maybe consider that in the randomizations
// TODO: maybe we could implement as part of topping skill # of slots seen each shuffle?
//   so like bad toppers may only see like 5 each shuffle, but really good toppers can see 9

public class TopGameManager : MonoBehaviour {
    private const float staminaCost = 5f; // per topping
    
    // THESE VARS AFFECT GAMEPLAY
    private const float timeChangePerTopping = 0.08f; // % to decrement time per placed topping

    // player toppings stat scaling
    private float minTopTime = 1f; // for 0 topping stat
    private float maxTopTime = 3f; // for 100 topping stat



    [Header("Game")]
    public ToppingSlot[] toppingSlots = new ToppingSlot[9];
    public GameObject actualGame;

    private bool gameActive = false;
    private PizzaObject pizza;

    private float topChangeTime = 0f;
    private float timer = 0f;


    public bool IsGameActive() { return gameActive; }

    void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            gameActive = false;
            this.gameObject.SetActive(false);
            GameManager.instance.ToggleMovement();
            transform.parent.GetComponentInChildren<StationInteract>().StartInteraction(); // allow re-interact
        }
        if (gameActive && timer > topChangeTime) {
            timer = 0f;
            StartCoroutine(SwapTopOptions());
        } else if (gameActive){
            timer += Time.deltaTime;   
        }
    }


    // Main game managers
    public void BeginTopGame() {
        var player = GameObject.FindWithTag("Player");
        var stats = player.GetComponent<CreatureStats>();
        if (stats.stamina < staminaCost) {
            transform.parent.GetComponentInChildren<StationInteract>().ShowStaminaMessage();
            return;
        }

        GameManager.instance.ToggleMovement();

        // TODO: for now we are assuming player has pie, implementation may change
        pizza = player.GetComponentInChildren<PizzaObject>();
        ResetTopGame();

        float baseTime = Mathf.Lerp(minTopTime, maxTopTime, stats.GetStats().Toppings / 100f);
        topChangeTime = baseTime * Mathf.Pow(1f -  timeChangePerTopping, pizza.GetToppingCount());

        actualGame.SetActive(true);
        StartCoroutine(SwapTopOptions());
    }
    public void ResetTopGame() {
        timer = 0f;
        gameActive = false;
        actualGame.SetActive(false);
    }


    public void SelectTopping(Topping t) {
        var playerCreature = GameObject.FindWithTag("Player");
        var stats = playerCreature.GetComponent<CreatureStats>();
        if (!stats.TryPerformTask(staminaCost)) { // if fail to top, send msg about stamina requirement
            gameActive = false;
            this.gameObject.SetActive(false);
            GameManager.instance.ToggleMovement();
            transform.parent.GetComponentInChildren<StationInteract>().ShowStaminaMessage();
            return;
        }

        gameActive = false;
        timer = 0;
        topChangeTime *= 1f - timeChangePerTopping;

        pizza.AddTopping(t);
        StartCoroutine(SwapTopOptions());
    }

    private IEnumerator SwapTopOptions() {
        gameActive = false;
        for (int i = 0; i < toppingSlots.Length; i++) {
            toppingSlots[i].SetText("");
        }

        var options = GenerateToppingSelection();
        yield return new WaitForSeconds(0.3f); // small delay for smoothness
        for (int i = 0; i < toppingSlots.Length; i++) {
            toppingSlots[i].SetCurrentTopping(options[i]);
        }
        gameActive = true;
    }
    
    public Topping[] GenerateToppingSelection() {
        var options = new HashSet<Topping>();
        if (pizza.GetToppingCount() == 0) {
            // if no toppings, always show bases as options
            foreach (Topping t in ToppingMethods.GetBases()) {
                options.Add(t);
            }
        } else if (pizza.GetToppingCount() == 1) {
            // if only base, always show secondary bases
            foreach (Topping t in ToppingMethods.GetSecondaryBases()) {
                options.Add(t);
            }
        }
        
        while (options.Count < toppingSlots.Length) {
            options.Add(ToppingMethods.GetRandomNonbaseTopping());
        }
        var optionsArr = options.ToArray();
        System.Random random = new System.Random();
        return optionsArr.OrderBy(x => random.Next()).ToArray(); // shuffle order
    }
}
