using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// this class manages the UI overlay and the actual game mechanics of the top minigame
// TODO: currently it uses a temp player movement script, to be replaced with a final implementation

// TODO: if a pizza is linked to an order, this should consider that in the randomizations (maybe)

public enum Topping
{
    RedSauce, OliveOil, // bases
    Cheese, // secondary bases
    Sausage, Pepperoni, Bacon, // meats
    Mushrooms, GreenPeppers, WhiteOnions, // veggies
    BlackOlives, BananaPeppers, RedOnions
}

public class TopGameManager : MonoBehaviour
{
    private const float baseTopTime = 3.5f; // how long the topping screen will stay the same
    private const float timeChangePerTopping = 0.4f; // how  much to decrement time per topping

    [Header("Game")]
    public ToppingSlot[] toppingSlots = new ToppingSlot[9];
    public GameObject actualGame;

    [Header("UI")]
    public TMP_Text toppingTxt;

    private bool gameActive = false;
    private PizzaObject pizza;

    private float topChangeTime = 0f;
    private float timer = 0f;

    public bool IsGameActive() { return gameActive;  }

    void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            gameActive = false;
            this.gameObject.SetActive(false);
            GameObject.FindWithTag("Player").GetComponent<TempPlayerMove>().ToggleMovement();
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
        // TODO: replace this with final script
        GameObject.FindWithTag("Player").GetComponent<TempPlayerMove>().ToggleMovement();

        // TODO: for now we are assuming player has pie, implementation may change
        pizza = GameObject.FindWithTag("Player").GetComponentInChildren<PizzaObject>();
        ResetTopGame();

        actualGame.SetActive(true);
        pizza.GetToppings().ForEach((t) => {
            toppingTxt.text += t.ToString() + ", ";
        });
        topChangeTime = baseTopTime - (pizza.GetToppingCount() * timeChangePerTopping);
        StartCoroutine(SwapTopOptions());
    }
    public void ResetTopGame() {
        timer = 0f;
        toppingTxt.text = "Current: ";
        gameActive = false;
        actualGame.SetActive(false);
    }


    public void SelectTopping(Topping t) {
        gameActive = false;
        timer = 0;
        topChangeTime -= timeChangePerTopping;

        toppingTxt.text += t.ToString() + ", ";
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
            options.Add(Topping.RedSauce);
            options.Add(Topping.OliveOil);
        } else if (pizza.GetToppingCount() == 1) {
            // if only base, always show secondary bases
            options.Add(Topping.Cheese);
        }

        var allOptions = System.Enum.GetValues(typeof(Topping)).Cast<Topping>().ToArray();
        while (options.Count < toppingSlots.Length) {
            options.Add(allOptions[Random.Range(0, allOptions.Length)]);
        }
        var optionsArr = options.ToArray();
        System.Random random = new System.Random();
        return optionsArr.OrderBy(x => random.Next()).ToArray(); // shuffle order
    }
}
