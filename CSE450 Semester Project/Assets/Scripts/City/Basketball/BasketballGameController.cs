using UnityEngine;

public class BasketballGameController : MonoBehaviour {
    private const KeyCode shootingKey = KeyCode.E;
    private const KeyCode dribbleKey = KeyCode.Space;
    private const KeyCode passkey = KeyCode.Q;

    public CreatureMove cm;
    public GameObject leftHoop;
    public GameObject rightHoop;

    [Header("UI")]
    public RectTransform shootingUI;
    public RectTransform shootingWindow;
    public RectTransform shootingBar;
    public float barSpeed = 120f;
    public float accuracyImpact = 6f; // divides inaccuracy to compute potential aiming offset
    // higher numbers mean accuracy has LESS impact on if the shot actually goes in


    private GameObject player;
    private bool directedLeft = false;
    private BasketballController ball;

    void Start() {
        player = GameObject.FindWithTag("Player");
        shootingUI.gameObject.SetActive(false);
        cm.UpdateSelectedCreature(player);
    }

    void Update() {
        if (Input.GetKeyUp(shootingKey) && ball != null) {
            ShootCurrentBall();
        }
        
        if (Input.GetKeyDown(shootingKey)) {
            var b = GameObject.Find("Basketball").GetComponent<BasketballController>();
            if (b.AttachedTo(player)) {
                shootingBar.anchoredPosition = Vector2.zero;
                shootingWindow.anchoredPosition = new Vector2(Random.Range(shootingUI.sizeDelta.x / 2, shootingUI.sizeDelta.x), 0);
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
        ball.ShootBall(target);
        ball = null;
    }
}
