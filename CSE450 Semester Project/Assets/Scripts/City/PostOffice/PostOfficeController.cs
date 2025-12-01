using System.Collections;
using System.Collections.Generic;
using System.IO;
using FileAnalysis;
using TMPro;
using UnityEngine;

public class PostOfficeController : MonoBehaviour {
    public enum Phase {
        // inactive -> begin -> [e] to write letter or [q] to exit -> 
        Inactive, Begin, Writing
    }


    private SpriteRenderer spr;

    public GameObject letterUI;
    public TMP_InputField letterToInput;
    public TMP_InputField letterInput;

    public GameObject dialogueBox;
    public TMP_Text dialogue;
    public TMP_Text nextInstruct;

    public GameObject letterCreaturePrefab;

    private Color defaultColor = new Color(1f, 1f, 1f, 0.1f);
    private Color triggeredColor = new Color(1f, 0, 0, 0.2f);
    private CreatureMove move;
    private Phase p;
    private bool interacting = false;

    // Start is called before the first frame update
    void Start() {
        spr = GetComponent<SpriteRenderer>();
        move = GameObject.Find("CreatureHandler").GetComponent<CreatureMove>();
        spr.color = defaultColor;
        letterUI.SetActive(false);
        dialogueBox.SetActive(false);
        dialogue.gameObject.SetActive(false);
        nextInstruct.gameObject.SetActive(false);
        p = Phase.Inactive;
    }

    // Update is called once per frame
    void Update() {
        if (interacting) {
            switch (p) {
                case Phase.Inactive:
                    // e to begin interaction
                    if (Input.GetKeyDown(KeyCode.E)) {
                        move.ToggleMovement(); // disable
                        dialogue.text = "Welcome to the post office! Would you like to write a letter?";
                        dialogueBox.SetActive(true);
                        dialogue.gameObject.SetActive(true);
                        nextInstruct.gameObject.SetActive(true);
                        p = Phase.Begin;
                    }
                    break;
                case Phase.Begin:
                    // q to quit, w to write letter, e to talk,
                    if (Input.GetKeyDown(KeyCode.Q)) {
                        EndInteraction();
                    } else if (Input.GetKeyDown(KeyCode.W)) {
                        letterToInput.text = "";
                        letterInput.text = "";
                        letterUI.SetActive(true);
                        dialogueBox.SetActive(false);
                        dialogue.gameObject.SetActive(false);
                        nextInstruct.gameObject.SetActive(false);
                        p = Phase.Writing;
                    } else if (Input.GetKeyDown(KeyCode.E)) {
                        dialogue.text = Dialogue.postOffice[Random.Range(0, Dialogue.postOffice.Length)];
                    }
                    break;
            }
        }
    }

    public void EndInteraction(bool sentLetter = false) {
        interacting = false;
        p = Phase.Inactive;
        StartCoroutine(DialogueEnd(sentLetter));
        letterUI.SetActive(false);

        if (!move.movementEnabled) {
            move.ToggleMovement();
        }
    }
    private IEnumerator DialogueEnd(bool sentLetter = false) {
        dialogueBox.SetActive(true);
        dialogue.gameObject.SetActive(true);
        nextInstruct.gameObject.SetActive(false);

        if (sentLetter) {
            dialogue.text = Dialogue.sentLetter[Random.Range(0, Dialogue.sentLetter.Length)];
            yield return new WaitForSeconds(3f);
        } else {
            dialogue.text = Dialogue.byeResponse[Random.Range(0, Dialogue.byeResponse.Length)];
            yield return new WaitForSeconds(2f);
        }

        dialogueBox.SetActive(false);
        dialogue.gameObject.SetActive(false);
    }

    public void SubmitLetter() {
        if (letterToInput.text.Length == 0 || letterInput.text.Length == 0) {
            Debug.Log("Letter is empty");
            Debug.Log(letterToInput.text + ", " + letterInput.text);
            return; // must have both...
        }
        string letterTo = letterToInput.text.Trim().ToLower().Replace(" ", "_");
        var filePath = Path.Combine(Application.persistentDataPath, letterTo + "_letter.txt");
        string content = "Dear " + letterToInput.text + ",\n\n" + letterInput.text;
        File.WriteAllText(filePath, content);
        FileInfo fi = new FileInfo(filePath);

        // register creature to staff (secret)
        var creature = Instantiate(letterCreaturePrefab);
        Stats stats = new Stats(filePath);
        stats.Cooking += 10;
        stats.Cutting += 10;
        stats.DoughHandling += 10;
        stats.Toppings += 10;
        creature.GetComponent<CreatureSelect>().InitCreature(fi.Name, fi.Length, stats);
        GameManager.instance.RegisterCreature(creature, true); // secret :)
        Destroy(creature); // doesn't belong in this city...

        EndInteraction(true);
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player") {
            interacting = true;
            spr.color = triggeredColor; 
        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag == "Player") {
            if (interacting) {
                EndInteraction();
            }
            spr.color = defaultColor;
            p = Phase.Inactive;
        }
    }
}
