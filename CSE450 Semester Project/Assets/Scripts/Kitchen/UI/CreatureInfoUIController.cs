using FileAnalysis;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// toggles station assignment UI, loads active creatures
public class CreatureInfoUIController : MonoBehaviour {
    public GameObject creatureUI;
    public GameObject[] creatureSlots = new GameObject[6];
    public CreatureAssign creatureAssigner;

    public void LinkComponents() {
        creatureUI = GameObject.Find("GameUI").transform.GetChild(4).gameObject;
        for (int i = 0; i < creatureSlots.Length; i++) {
            creatureSlots[i] = creatureUI.transform.GetChild(1).GetChild(i).gameObject;
        }
        creatureAssigner = GameObject.Find("CreatureHandler").GetComponent<CreatureAssign>();
    }

    void Start() {
        creatureAssigner = GameObject.Find("CreatureHandler").GetComponent<CreatureAssign>();
        ClearUISlots();
    }

    public void ShowCreatureInfo() {
        ClearUISlots();
        var playerCreature = GameObject.FindWithTag("Player");
        if (playerCreature != null) {
            var stats = playerCreature.GetComponent<CreatureStats>().GetStats();
            var cs = playerCreature.GetComponent<CreatureSelect>();
            SetCreatureSlot(creatureSlots[0].transform, cs.GetSprite(), cs.GetSpriteColor(), cs.GetName(), "[ Player ]", stats);
        }

        // fill in remaining slots with active creature data
        var creatures = GameObject.FindGameObjectsWithTag("Creature");
        for(int i = 1; i < creatureSlots.Length; i++){
            if (creatures.Length < i){ break; }
            var c = creatures[i - 1];
            Debug.Log(c);
            var stats = c.GetComponent<CreatureStats>().GetStats();
            var cs = c.GetComponent<CreatureSelect>();
            var role = "none";
            var station = creatureAssigner.GetCreatureStation(cs);
            if (station != Station.Table) {
                role = "[ " + station.ToString() + " ]";
            }
            SetCreatureSlot(creatureSlots[i].transform, cs.GetSprite(), cs.GetSpriteColor(), cs.GetName(), role, stats);
        }

        creatureUI.SetActive(true);
    }

    public void HideCreatureInfo() {
        creatureUI.SetActive(false);
    }

    // children ALWAYS ordered sprite, name, role, stats
    private void SetCreatureSlot(Transform slot, Sprite s, Color color, string name, string role, Stats stats) {
        slot.GetChild(0).GetComponent<Image>().sprite = s;
        slot.GetChild(0).GetComponent<Image>().color = color;
        slot.GetChild(1).GetComponent<TMP_Text>().text = name;
        slot.GetChild(2).GetComponent<TMP_Text>().text = role;
        if (stats == null) {
            slot.GetChild(3).GetComponent<TMP_Text>().text = "";
        } else {
            slot.GetChild(3).GetComponent<TMP_Text>().text =
                "<b>Toss</b>: " + stats.DoughHandling +
                "  |  <b>Top</b>: " + stats.Toppings +
                "\n<b>Ovens</b>: " + stats.Cooking +
                "  |  <b>Cut</b>: " + stats.Cutting +
                "\n<b>Speed</b>: " + stats.Speed +
                "  |  <b>Stamina</b>: " + stats.Stamina;
        }
    }

    private void ClearUISlots() {
        foreach (GameObject slot in creatureSlots) {
            SetCreatureSlot(slot.transform, null, Color.white, "---", "---", null);
        }
    }
    
}
