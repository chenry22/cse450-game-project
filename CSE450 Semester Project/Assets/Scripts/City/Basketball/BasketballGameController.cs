using TMPro;
using UnityEngine;

// Mainly used to handle main player and their interactions in the game

public class BasketballGameController : MonoBehaviour {
    private const KeyCode shootingKey = KeyCode.E;
    private const KeyCode dribbleKey = KeyCode.Space;
    private const KeyCode passkey = KeyCode.Q;

    public CreatureMove cm;
    public GameObject leftHoop;
    public GameObject rightHoop;
    public TMP_Text scoreLabel;

    [Header("Shooting UI")]
    public RectTransform shootingUI;
    public RectTransform shootingWindow;
    public RectTransform shootingBar;
    public float minShootingWindowSize = 4;
    public float maxShootingWindowSize = 50;
    public float barSpeed = 120f;
    public float accuracyImpact = 6f; // divides inaccuracy to compute potential aiming offset
    // higher numbers mean accuracy has LESS impact on if the shot actually goes in
    public float staminaDisplayTime = 1f;

    [Header("Stamina")]
    public GameObject staminaUI;
    public RectTransform staminaFill;
    public float staminaRecoveryRate = 2f;
    public float dribbleStaminaCost = 3f;
    public float shootStaminaCost = 10f;


    private GameObject player;
    private CreatureStats stats;
    private bool directedLeft = false;
    private BasketballController ball;
    private float staminaDisplayTimer = 0f;
    private int playerScore = 0;
    private int opponentScore = 0;

    void Start() {
        player = GameObject.FindWithTag("Player");
        stats = player.GetComponent<CreatureStats>();
        shootingUI.gameObject.SetActive(false);
        cm.UpdateSelectedCreature(player);
        UpdateScoreUI();
    }
    
    public void UpdateScore(int points, bool player) { 
        if (player) {
            playerScore += points;
        } else {
            opponentScore += points;
        }
        UpdateScoreUI();
    }
    private void UpdateScoreUI() {
        scoreLabel.text = "[HOME] " + playerScore + " - " + opponentScore + " [AWAY]";
    }

    void Update() {
        stats.RecoverStamina(Time.deltaTime * staminaRecoveryRate);
        if (staminaDisplayTimer >= 0f) {
            staminaDisplayTimer -= Time.deltaTime;
            if (staminaDisplayTimer <= 0f) {
                staminaUI.SetActive(false);
            }
        }

        // shooting logic
        if (Input.GetKeyUp(shootingKey) && ball != null) {
            ShootCurrentBall();
            HandleStaminaCost(shootStaminaCost);
        }
        if (Input.GetKeyDown(shootingKey) && stats.stamina >= shootStaminaCost) {
            var b = GameObject.Find("Basketball").GetComponent<BasketballController>();
            if (b.AttachedTo(player)) {
                shootingBar.anchoredPosition = Vector2.zero;
                var diff = Mathf.Abs((player.transform.position - rightHoop.transform.position).magnitude);
                shootingWindow.sizeDelta = new Vector2(Mathf.Lerp(maxShootingWindowSize, minShootingWindowSize, diff / 14), shootingWindow.sizeDelta.y);
                shootingWindow.anchoredPosition = new Vector2(Random.Range(shootingUI.sizeDelta.x * 0.65f, shootingUI.sizeDelta.x), 0);
                shootingUI.gameObject.SetActive(true);
                ball = b;
            }
        }
        if (Input.GetKey(shootingKey) && ball != null) {
            shootingBar.anchoredPosition += Vector2.right * barSpeed * Time.deltaTime;
            if (shootingBar.anchoredPosition.x > shootingUI.sizeDelta.x) {
                ShootCurrentBall();
            }
        }
        
        // dribbling logic
        if (Input.GetKeyDown(dribbleKey)) {
            var b = GameObject.Find("Basketball").GetComponent<BasketballController>();
            if (b.AttachedTo(player) && stats.stamina >= dribbleStaminaCost) {
                if (b.Dribble()) {
                    HandleStaminaCost(dribbleStaminaCost);
                }
            }
        }
    }
    
    private void HandleStaminaCost(float cost) {
        stats.TryPerformTask(cost);
        staminaFill.anchoredPosition = new Vector2(
            Mathf.Lerp(-staminaUI.GetComponent<RectTransform>().sizeDelta.x / 2f, 0f, stats.stamina / stats.maxStamina), 0);
        staminaDisplayTimer = staminaDisplayTime;
        staminaUI.SetActive(true);
    }
    
    private void ShootCurrentBall() {
        if (ball == null) { return; }

        Vector2 accuracyImpactVector;
        float offset = shootingBar.anchoredPosition.x - shootingWindow.anchoredPosition.x;
        if (Mathf.Abs(offset) < shootingWindow.sizeDelta.x / 2f) {
            accuracyImpactVector = Vector2.zero;
        } else if (offset > 0) {
            offset -= shootingWindow.sizeDelta.x / 2f;
            offset /= accuracyImpact * 2;
            accuracyImpactVector = new Vector2(Random.Range(0, offset), Random.Range(0, offset));
        } else {
            offset += shootingWindow.sizeDelta.x / 2f;
            offset /= accuracyImpact * 2;
            accuracyImpactVector = new Vector2(Random.Range(offset, 0), Random.Range(offset, 0));
        }
        Debug.Log(accuracyImpactVector);
        
        shootingUI.gameObject.SetActive(false);
        Vector2 target = directedLeft ? leftHoop.transform.position : rightHoop.transform.position;
        target += accuracyImpactVector + (Vector2.down * 0.2f);
        target = new Vector2(Mathf.Max(player.transform.position.x + 0.8f, target.x), target.y);
        if (ball.ShootBall(target)) {
            stats.TryPerformTask(shootStaminaCost);
            ball = null;
        }
    }
}
