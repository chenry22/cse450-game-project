using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// this class manages the UI overlay and the actual game mechanics of the toss minigame
// TODO: currently it uses a temp player movement script, to be replaced with a final implementation

public class TossGameManager : MonoBehaviour {
    private const float progressPerToss = 0.06f; // out of 1.0f
    private const int qualityLossPerMistake = 5; // out of 100
    private const float baseDoughSpeed = 5f;
    private const float doughSpeedIncrease = 1f;
    private const float maxDoughSpeed = 15f; // max speed of back and forth movement
    private const int rotationVelocityScale = 80; // basically a slider for how extreme the spin will be on each toss
    private const float tossSquareMinScale = 0.7f; // scale at which pie is totally circular, probably shouldn't change
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
            GameObject.Find(GameManager.kitchenGameManager).GetComponent<GameManager>().ToggleMovement();
        } else if (!gameActive && progress >= 1f && Input.GetKeyDown(KeyCode.E)) {
            gameObject.SetActive(false); // basically just kill UI
            GameObject.Find(GameManager.kitchenGameManager).GetComponent<GameManager>().ToggleMovement();
            var newPie = Instantiate(pizza);
            newPie.GetComponent<PizzaObject>().InitializePizza(quality);
            newPie.transform.parent = GameObject.FindWithTag("Player").transform;
            newPie.transform.localPosition = new Vector2(0.6f, 0.2f);
        }
    }


    // Main game managers
    public void BeginTossGame() {
        GameObject.Find(GameManager.kitchenGameManager).GetComponent<GameManager>().ToggleMovement();
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
        dough.gameObject.transform.GetChild(0).transform.localScale = Vector2.one;
        progressFill.transform.localPosition = new Vector3(-0.5f, 0);
        progressFill.transform.localScale = new Vector3(0, 0);
    }
    private void EndTossGame() {
        gameActive = false;
        dough.velocity = Vector2.zero;
        dough.transform.localPosition = Vector3.zero;
        helpTxt.text = completionHelpTxt;
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
            if (Mathf.Abs(doughSpeed) > maxDoughSpeed) {
                doughSpeed = doughSpeed < 0 ? -maxDoughSpeed : maxDoughSpeed;
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
        quality -= qualityLossPerMistake;
        qualityTxt.text = "<b>Quality:</b> " + quality + " / 100";
        dough.velocity = Vector2.zero;
        dough.transform.localPosition = Vector2.zero;
        StartCoroutine(ActivateDough(1f));
    }
}
