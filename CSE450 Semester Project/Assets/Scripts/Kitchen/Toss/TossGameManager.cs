using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// this class manages the UI overlay and the actual game mechanics of the toss minigame
public class TossGameManager : MonoBehaviour {
    public static float requiredStamina = 10f;
    public static Vector2 pizzaOffset = new Vector2(0.6f, 0.1f); // when new pizza object, where to position

    // Game UI vars (not gameplay)
    private const int rotationVelocityScale = 80; // basically a slider for how extreme the spin will be on each toss
    private const float tossSquareMinScale = 0.7f; // scale at which pie is totally circular


    // player toss stat scaling
    private const float minDoughSpeedIncrease = 0.7f; // for stat 100
    private const float maxDoughSpeedIncrease = 2.1f; // for stat 0
    private float doughSpeedIncrease = 0f; // how much to speed up every toss

    private const float minProgressPerToss = 0.03f; // for stat 0
    private const float maxProgressPerToss = 0.12f; // for stat 100
    private float progressPerToss = 0f; // out of 1.0f

    private const float minBaseDoughSpeed = 4f; // for stat 100
    private const float maxBaseDoughSpeed = 8f; // for stat 0
    private float baseDoughSpeed = 0f; // speed at very start of each toss cycle (including after drops)

    private const float minDoughSpeedCap = 10f; // for stat 100
    private const float maxDoughSpeedCap = 20f; // for stat 0
    private float doughSpeedCap = 0f; // when speed will stop increasing

    private const int minQualityLossPerMistake = 3; // for stat 100
    private const int maxQualityLossPerMistake = 15; // for stat 0
    private int qualityLossPerMistake = 0; // out of 100

    private const string mainHelpText = "[<] [>] or [A] [D] to toss\n[Q] to cancel";
    private const string completionHelpTxt = "[E] to continue";

    [Header("Pizza Prefab")]
    public GameObject pizza;

    [Header("Game")]
    public Rigidbody2D dough;
    public PolygonCollider2D leftTriangle;
    public PolygonCollider2D rightTriangle;
    public GameObject actualGame;

    [Header("UI")]
    public GameObject progressFill;
    public TMP_Text qualityTxt;
    public TMP_Text helpTxt;

    private bool gameActive = false;
    private float progress = 0;
    private int quality = 100;
    private float doughSpeed = 0f;

    public bool IsGameActive() { return gameActive; }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            // end game without saving progress
            // basically kill this UI
            gameActive = false;
            this.gameObject.SetActive(false);
            ResetTossGame();

            // and enable user movement again
            GameManager.instance.ToggleMovement();
            // allow re-interaction right after
            transform.parent.GetComponentInChildren<StationInteract>().StartInteraction();
        } else if (!gameActive && progress >= 1f && Input.GetKeyDown(KeyCode.E)) {
            gameObject.SetActive(false); // basically just kill UI
            GameManager.instance.ToggleMovement();

            // create new pie
            var newPie = Instantiate(pizza);
            newPie.GetComponent<PizzaObject>().InitializePizza(quality);
            newPie.transform.parent = GameObject.FindWithTag("Player").transform;
            newPie.transform.localPosition = pizzaOffset;

            // if player is holding ticket, automatically link order (for automation system)
            var ticketOrder = GameObject.FindWithTag("Player").GetComponentInChildren<OrderTicket>()?.GetOrder();
            if (ticketOrder != null) {
                Debug.Log("Linked held ticket!");
                newPie.GetComponent<PizzaObject>().LinkOrder(ticketOrder);
            }
        }
    }


    // Main game managers
    public void BeginTossGame() {
        var playerCreature = GameObject.FindWithTag("Player");
        var stats = playerCreature.GetComponent<CreatureStats>();
        if (stats.stamina < 10f) {
            return;
        }

        var tossSkill = stats.GetStats().DoughHandling / 100f;
        baseDoughSpeed = Mathf.Lerp(maxBaseDoughSpeed, minBaseDoughSpeed, tossSkill);
        doughSpeedCap = Mathf.Lerp(maxDoughSpeedCap, minDoughSpeedCap, tossSkill);
        doughSpeedIncrease = Mathf.Lerp(maxDoughSpeedIncrease, minDoughSpeedIncrease, tossSkill);
        progressPerToss = Mathf.Lerp(minProgressPerToss, maxProgressPerToss, tossSkill);
        qualityLossPerMistake = Mathf.RoundToInt(Mathf.Lerp(maxQualityLossPerMistake, minQualityLossPerMistake, tossSkill));

        GameManager.instance.ToggleMovement();
        ResetTossGame();
        
        actualGame.SetActive(true);
        StartCoroutine(ActivateDough(1f));
    }
    public void ResetTossGame() {
        helpTxt.text = mainHelpText;
        gameActive = false;
        actualGame.SetActive(false);
        progress = 0;
        quality = 100;
        qualityTxt.text = "<b>Quality:</b> " + quality + " / 100";

        dough.gameObject.transform.GetChild(0).transform.localScale = Vector2.one;
        dough.gameObject.transform.localPosition = Vector2.zero;
        progressFill.transform.localPosition = new Vector3(-0.5f, 0);
        progressFill.transform.localScale = new Vector3(0, 0);
    }
    private void EndTossGame()
    {
        gameActive = false;
        dough.velocity = Vector2.zero;
        dough.transform.localPosition = Vector3.zero;
        helpTxt.text = completionHelpTxt;
        
        // use stamina to act
        var playerCreature = GameObject.FindWithTag("Player");
        var stats = playerCreature.GetComponent<CreatureStats>();
        stats.TryPerformTask(requiredStamina);
    }


    // allow for delay to give reaction time
    private IEnumerator ActivateDough(float delay) {
        yield return new WaitForSeconds(delay); // small delay to allow for some reaction time
        gameActive = true;
        doughSpeed = baseDoughSpeed;
        var dir = Random.Range(0, 1f) > 0.5f ? -1 : 1;
        doughSpeed *= dir;
        dough.velocity = new Vector2(doughSpeed, 0);
    }


    // actual game interaction
    // single iteration of toss
    public void TossDough() {
        var newX = Mathf.Min(0, progressFill.transform.localPosition.x + (0.5f * progressPerToss));
        var newScale = Mathf.Min(1, progressFill.transform.localScale.x + progressPerToss);
        progressFill.transform.localPosition = new Vector3(newX, 0);
        progressFill.transform.localScale = new Vector3(newScale, 1);
        progress += progressPerToss;

        if (progress >= 1) {
            EndTossGame();
        } else {
            // make more circular...
            var newSquareScale = 1f - ((1f - tossSquareMinScale) * progress);
            dough.gameObject.transform.GetChild(0).transform.localScale = new Vector2(newSquareScale, newSquareScale);

            // basically just flip/bump to other side
            doughSpeed += doughSpeed < 0 ? -doughSpeedIncrease : doughSpeedIncrease;
            if (Mathf.Abs(doughSpeed) > doughSpeedCap) {
                doughSpeed = doughSpeed < 0 ? -doughSpeedCap : doughSpeedCap;
            }
            doughSpeed *= -1f;
            dough.velocity = new Vector2(doughSpeed, 0);

            // add some cool random rotation also
            var rotationRange = doughSpeed * rotationVelocityScale;
            dough.angularVelocity = Random.Range(-rotationRange, rotationRange);
        }
    }
    public void DropDough() {
        gameActive = false;
        quality = Mathf.Max(0, quality - qualityLossPerMistake);
        qualityTxt.text = "<b>Quality:</b> " + quality + " / 100";
        dough.velocity = Vector2.zero;
        dough.transform.localPosition = Vector2.zero;
        StartCoroutine(ActivateDough(1f));
    }
}
