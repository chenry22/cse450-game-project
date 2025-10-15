using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FileAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatureManagerUIController : MonoBehaviour
{
    public GameObject ui;
    public GameObject exitButton;

    public GameObject[] activeSlots = new GameObject[6];
    private CreatureSelect[] activeCreatures = new CreatureSelect[6];
    public GameObject[] storedSlots = new GameObject[12];
    private CreatureSelect[] storedCreatures = new CreatureSelect[12];

    // private int pageNum = 0;
    private int activeCreatureIdx = -1;
    private int storedCreatureIdx = -1;


    // should be called instead of basic view activation
    public void LoadCreatureManagerUI() {
        ClearUISlots();

        var names = new List<string>(); // TODO: THIS IS A VERY LAZY SOLUTION LIKE BAD
        // need to set up some id system instead.

        // load current creatures
        var playerCreature = GameObject.FindWithTag("Player");
        if (playerCreature != null) {
            exitButton.SetActive(true);
            var stats = playerCreature.GetComponent<CreatureStats>().GetStats();
            var cs = playerCreature.GetComponent<CreatureSelect>();

            activeCreatures[0] = cs;
            names.Add(cs.GetName());
            SetActiveCreatureSlot(activeSlots[0].transform, cs.GetSprite(), cs.GetSpriteColor(), cs.GetName(), stats);
        } else {
            exitButton.SetActive(false);
        }

        var creatures = GameObject.FindGameObjectsWithTag("Creature");
        var offset = 1;
        for (int i = 1; i < activeSlots.Length; i++) {
            if (creatures.Length + offset < i) { break; }
            var c = creatures[i - 1 + offset];
            var stats = c.GetComponent<CreatureStats>().GetStats();
            var cs = c.GetComponent<CreatureSelect>();

            if (names.Contains(cs.GetName())) {
                // don't add, check next index
                i--;
                continue;
            }

            activeCreatures[i] = cs;
            SetActiveCreatureSlot(activeSlots[i].transform, cs.GetSprite(), cs.GetSpriteColor(), cs.GetName(), stats);
        }

        // load one page of stored creatures
        List<GameObject> registeredCreatures = GameObject.Find(GameManager.kitchenGameManager).GetComponent<GameManager>().GetRegisteredCreatures();
        for (int i = 0; i < storedSlots.Length; i++) {
            if (registeredCreatures.Count < i) { break; }
            var c = registeredCreatures[i];
            var cs = c.GetComponent<CreatureSelect>();

            storedCreatures[i] = cs;
            SetStoredCreatureSlot(storedSlots[i].transform, cs.GetSprite(), cs.GetSpriteColor(), cs.GetName());
        }

        ui.SetActive(true);
    }
    
    private void SwapCreatures(int activeIdx, int storedIdx) {
        var activeCreature = activeCreatures[activeIdx];
        var storedCS = storedCreatures[storedCreatureIdx];
        var storedCreature = storedCS.transform.parent.gameObject;

        Destroy(activeCreature.transform.parent.gameObject);
        activeCreatures[activeCreatureIdx] = null;
        storedCreatures[storedIdx] = activeCreature;
        SetStoredCreatureSlot(storedSlots[storedIdx].transform, activeCreature.GetSprite(), activeCreature.GetSpriteColor(), activeCreature.GetName());
        if (storedCreature != null) {
            Instantiate(storedCreature, GameObject.Find("Spawner").transform);
            storedCreatures[storedCreatureIdx] = activeCreature;
            SetActiveCreatureSlot(activeSlots[activeIdx].transform, storedCS.GetSprite(),
                storedCS.GetSpriteColor(), storedCS.GetName(), storedCreature.GetComponent<CreatureStats>().GetStats());
        } else {
            SetActiveCreatureSlot(activeSlots[activeIdx].transform, null, Color.white, "---", null);
        }
    }
    public void SelectActiveCreature(int idx) {
        if (storedCreatureIdx >= 0) {
            SwapCreatures(idx, storedCreatureIdx);
        } else {
            activeCreatureIdx = activeCreatureIdx == idx ? -1 : idx;
        }
    }
    public void SelectStoredCreature(int idx) {
        if (activeCreatureIdx >= 0) {
            SwapCreatures(activeCreatureIdx, idx);
        } else {
            storedCreatureIdx = storedCreatureIdx == idx ? -1 : idx;
        }
    }

    private void SetActiveCreatureSlot(Transform slot, Sprite s, Color color, string name, Stats stats) {
        slot.GetChild(0).GetComponent<Image>().sprite = s;
        slot.GetChild(0).GetComponent<Image>().color = color;
        slot.GetChild(1).GetComponent<TMP_Text>().text = name;
        if (stats == null) {
            slot.GetChild(2).GetComponent<TMP_Text>().text = "";
        } else {
            slot.GetChild(2).GetComponent<TMP_Text>().text =
                "<b>Toss</b>: " + stats.DoughHandling +
                "\n<b>Top</b>: " + stats.Toppings +
                "\n<b>Ovens</b>: " + stats.Cooking +
                "\n<b>Cut</b>: " + stats.Cutting;
        }
    }
    private void SetStoredCreatureSlot(Transform slot, Sprite s, Color color, string name) {
        slot.GetChild(0).GetComponent<Image>().sprite = s;
        slot.GetChild(0).GetComponent<Image>().color = color;
        slot.GetChild(1).GetComponent<TMP_Text>().text = name;
    }
    
    private void ClearUISlots() {
        foreach (GameObject slot in storedSlots) {
            SetStoredCreatureSlot(slot.transform, null, Color.white, "---");
        }
        foreach(GameObject slot in activeSlots) {
            SetActiveCreatureSlot(slot.transform, null, Color.white, "---", null);
        }
    }

    public void HideCreatureUI() {
        ui.SetActive(false);
    }
    public void ShowCreatureUI(){
        ui.SetActive(true);
    }
}
