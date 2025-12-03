using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HatShopController : MonoBehaviour
{
    private SpriteRenderer spr;

    public GameObject dialogueBox;
    public TMP_Text dialogue;

    private Color defaultColor = new Color(1f, 1f, 1f, 0.1f);
    private Color triggeredColor = new Color(1f, 0, 0, 0.2f);
    private bool interacting = false;

    // Start is called before the first frame update
    void Start()
    {
        spr = this.GetComponent<SpriteRenderer>();
        dialogueBox.SetActive(false);
        dialogue.gameObject.SetActive(false);
        spr.color = defaultColor;
    }

    // Update is called once per frame
    void Update() {
        if (interacting && Input.GetKeyDown(KeyCode.E)) {
            if (dialogueBox.activeSelf) {
                dialogue.text = Dialogue.hatShop[Random.Range(0, Dialogue.hatShop.Length)];
            } else {
                dialogue.text = "Welcome to the hat shop! Feel free to look around or buy something!";
                dialogue.gameObject.SetActive(true);
                dialogueBox.SetActive(true);
            }
        }
    }

    public void EndInteraction() {
        interacting = false;
        StartCoroutine("DialogueEnd");
    }
    private IEnumerator DialogueEnd() {
        dialogueBox.SetActive(true);
        dialogue.gameObject.SetActive(true);

        dialogue.text = Dialogue.byeResponse[Random.Range(0, Dialogue.byeResponse.Length)];
        yield return new WaitForSeconds(2f);
        dialogueBox.SetActive(false);
        dialogue.gameObject.SetActive(false);
    }


    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player") {
            StopCoroutine("DialogueEnd");
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
        }
    }
}
