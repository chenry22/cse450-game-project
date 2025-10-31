using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RhythmGameController : MonoBehaviour
{
    public static RhythmGameController instance;
    public Transform arrowCircle;
    public float rotationRate = 10f; // degrees
    public float arrowGenerateInterval = 1f;
    public float arrowGenerateChance = 0.8f;

    [Header("Arrow Properties")]
    public GameObject arrowPrefab;
    public float arrowDistance = 8f;
    public float minArrowTime = 0.5f;
    public float maxArrowTime = 2f;

    [Header("Gameplay")]
    public TMP_Text scoreText;
    public int maxScorePerArrow = 5;
    public int minScorePerArrow = 1;
    public int failPenalty = 3;
    private int score = 0;


    private float timer = 0f;

    void Awake() {
        instance = this;
    }

    // Update is called once per frame
    void Update() {
        arrowCircle.rotation = Quaternion.Euler(0, 0,
            arrowCircle.rotation.eulerAngles.z + (rotationRate * Time.deltaTime));

        timer += Time.deltaTime;
        if (arrowGenerateInterval <= timer) {
            timer = 0f;
            if (arrowGenerateChance >= Random.Range(0, 1f)) {
                // generate arrow
                Debug.Log("Arrow spawned");
                float arrowTime = Random.Range(minArrowTime, maxArrowTime);
                GameObject arrow = Instantiate(arrowPrefab);

                Quaternion q = new Quaternion();
                q.eulerAngles = new Vector3(0, 0, arrowCircle.rotation.eulerAngles.z + (rotationRate * arrowTime));
                Vector2 pos = q * Vector2.up * arrowDistance;
                arrow.transform.position = pos;
                q.eulerAngles = new Vector3(0, 0, q.eulerAngles.z + 180);
                arrow.transform.rotation = q;
                

                // v = d / t
                arrow.GetComponent<Rigidbody2D>().velocity = new Vector2(
                    (arrowCircle.transform.position.x - pos.x) / arrowTime,
                    (arrowCircle.transform.position.y - pos.y) / arrowTime);
            }
        } 
    }

    public void RhythmScore(Transform arrow) {
        score += (int)Mathf.Lerp(minScorePerArrow, maxScorePerArrow,
            Mathf.Clamp01(Mathf.Abs((arrow.position - arrowCircle.position).magnitude)));
        scoreText.text = "Score: " + score;
    }
    
    public void RhyhtmFail() {
        score -= failPenalty;
        scoreText.text = "Score: " + score;
    }   
}
