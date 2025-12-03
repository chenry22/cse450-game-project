using System.Collections;
using System.Collections.Generic;
using FileAnalysis;
using TMPro;
using UnityEngine;

public class CityCreatureBehavior : MonoBehaviour
{
    private enum CreaturePhase
    {
        Idle, Move, Talk, Flee, Dance
    }

    private Rigidbody2D rb;
    private Stats stats;
    public TMP_Text speechText;

    private CreaturePhase phase;
    private Vector3 targetPos;
    private float maxMoveTargetDist = 5f;
    private float speed = 0f;
    private float timer = 0f;
    private float interval = 1f;

    public float minPauseTime = 0.5f;
    public float maxPauseTime = 3.2f;
    public float minMoveTime = 1f;
    public float maxMoveTime = 2.5f;
    public float minFleeTime = 0.7f;
    public float maxFleeTime = 1.5f;

    private GameObject other;

    private const float moveChance = 0.7f;

    void Start() {
        rb = this.GetComponent<Rigidbody2D>();
        stats = this.GetComponent<CreatureStats>().GetStats();
        speed = this.GetComponent<CreatureStats>().speed * 0.7f; // automation decrease
        speechText = this.transform.GetChild(2).GetComponentInChildren<TMP_Text>();
        interval = Random.Range(minPauseTime, maxPauseTime);

        phase = CreaturePhase.Idle;
    }

    // Update is called once per frame
    void Update() {     
        if (this.tag == "Player" || this.tag == "Partner") { return; } // don't automate player (for testing)   
        switch(phase) {
            case CreaturePhase.Idle:
                if (timer < interval) {
                    timer += Time.deltaTime;
                    return;
                } else {
                    timer = 0f;
                }
                
                if(Random.Range(0, 1f) < moveChance) {
                    GenerateMovePosition();

                    phase = CreaturePhase.Move;
                    interval = Random.Range(minMoveTime, maxMoveTime);
                } else if (Random.Range(0, 1f) < (stats.Extroversion / 120f) - 0.25f) {
                    phase = CreaturePhase.Dance;
                    StartCoroutine("Dance");
                }
                break;
            case CreaturePhase.Move:
                if (timer < interval) {
                    timer += Time.deltaTime;

                    // move to position if not already there
                    if ((targetPos - transform.position).sqrMagnitude > 1) {
                        rb.velocity = (targetPos - transform.position).normalized * speed;
                    } else {
                        rb.velocity = Vector2.zero;
                    }
                } else {
                    phase = CreaturePhase.Idle;
                    timer = 0f;
                    interval = Random.Range(minPauseTime, maxPauseTime);
                }
                break;
            case CreaturePhase.Flee:
                if (timer < interval) {
                    timer += Time.deltaTime;
                    rb.velocity = (transform.position - other.transform.position).normalized * speed;
                } else {
                    rb.velocity = Vector2.zero;
                    phase = CreaturePhase.Idle;
                    timer = 0f;
                    interval = Random.Range(minPauseTime, maxPauseTime);
                    StartCoroutine("HideResponseAfterDelay");
                }
                break;
        }
    }
    
    public void GenerateMovePosition() {
        float x = Random.Range(-maxMoveTargetDist, maxMoveTargetDist);
        float y = Random.Range(-maxMoveTargetDist, maxMoveTargetDist);
        targetPos = new Vector3(x + transform.position.x, y + transform.position.y, 0);
    }
    
    public void RespondToConversation() {
        speechText.text = Dialogue.startResponse[Random.Range(0, Dialogue.startResponse.Length)];
        transform.GetChild(2).gameObject.SetActive(true);
    }
    public void HideResponse() {
        transform.GetChild(2).gameObject.SetActive(false);
    }
    public IEnumerator HideResponseAfterDelay() {
        yield return new WaitForSeconds(1.5f);
        HideResponse();
    }
    
    public IEnumerator DoConversation() {
        yield return new WaitForSeconds(0.2f);
        speechText.text = Dialogue.start[Random.Range(0, Dialogue.start.Length)];
        transform.GetChild(2).gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        HideResponse();
        yield return new WaitForSeconds(0.5f);
        other.GetComponent<CityCreatureBehavior>().RespondToConversation();
        yield return new WaitForSeconds(3f);
        other.GetComponent<CityCreatureBehavior>().HideResponse();
        yield return new WaitForSeconds(0.5f);
        speechText.text = Dialogue.endReponse[Random.Range(0, Dialogue.endReponse.Length)];
        transform.GetChild(2).gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        HideResponse();

        GenerateMovePosition();
        timer = 0f;
        interval = Random.Range(minMoveTime, maxMoveTime);
        phase = CreaturePhase.Move;
    }
    
    private IEnumerator Dance() {
        yield return new WaitForSeconds(0.4f);
        this.GetComponent<SpriteRenderer>().flipX = true;
        yield return new WaitForSeconds(0.9f);
        this.GetComponent<SpriteRenderer>().flipX = false;
        yield return new WaitForSeconds(0.7f);
        this.GetComponent<SpriteRenderer>().flipX = true;
        yield return new WaitForSeconds(0.6f);
        this.GetComponent<SpriteRenderer>().flipX = false;
    }
    

    void OnCollisionEnter2D(Collision2D other) {
        if (rb == null) { return; }
        if (other.gameObject.tag == "Creature" && this.tag == "Creature") {
            rb.velocity = Vector2.zero; // freeze at collision
            StopCoroutine("Dance");
            this.GetComponent<SpriteRenderer>().flipX = false;
            phase = CreaturePhase.Talk;
            this.other = other.gameObject;

            // only one of the creatures should act.
            if (gameObject.GetInstanceID() < other.gameObject.GetInstanceID()) {
                if (Random.Range(0, 1f) < 0.25f - (stats.Extroversion / 120f)) {
                    Debug.Log("FLEEING, extroversion: " + stats.Extroversion);
                    timer = 0f;
                    interval = Random.Range(minFleeTime, maxFleeTime);
                    transform.GetChild(2).gameObject.SetActive(true);

                    speechText.text = Dialogue.fleeing[Random.Range(0, Dialogue.fleeing.Length)];
                    phase = CreaturePhase.Flee;
                } else {
                    StartCoroutine("DoConversation");
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision) {
        StopCoroutine("HideResponseAfterDelay");
        StopCoroutine("DoConversation");

        // don't affect player, flee state has own functionality
        if (this.tag == "Player" || phase == CreaturePhase.Flee) { return; }

        HideResponse();
        timer = 0f;
        phase = CreaturePhase.Idle;
    }
}
