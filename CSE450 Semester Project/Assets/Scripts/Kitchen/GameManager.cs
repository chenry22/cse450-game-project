using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static string kitchenGameManager = "GameManager"; // name for other scripts to reference
    private int day = 0; // basically keeping track of some progression
    private float totalProfit = 0;
    private float currentBalance = 0;
    private List<GameObject> availableCreatures = new List<GameObject>();

    private DayManager dayManager;
    public GameObject fileUploadUI;
    public Button dayBeginButton;

    void Start() {
        dayManager = GameObject.Find(DayManager.dayManagerObjName).GetComponent<DayManager>();
        dayBeginButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "Begin Day " + day;
        dayBeginButton.gameObject.SetActive(false);
    }
    
    public void RegisterCreature(GameObject creature) {
        if (creature.GetComponent<CreatureStats>() != null) {
            // is valid creature
            GameObject creatureCopy = Instantiate(creature);
            creatureCopy.SetActive(false);
            availableCreatures.Add(creatureCopy);
            
            if (day == 0) {
                dayBeginButton.gameObject.SetActive(true);
            }
            // DeactivateFileUpload();
        }
    }

    public void BeginDay() {
        dayBeginButton.gameObject.SetActive(false);
        dayManager.StartDay(day);
    }

    // called by DayManager
    public void EndDay(float profit) {
        day++;
        totalProfit += profit;
        currentBalance += profit;

        dayBeginButton.GetComponent<TMP_Text>().text = "Begin Day " + day;
        dayBeginButton.gameObject.SetActive(false);
    }

    public void ActivateFileUpload() {
        fileUploadUI.SetActive(true);
    }

    public void DeactivateFileUpload() {
        // TODO: un-comment this, just for the purposes of testing station assignment
        fileUploadUI.SetActive(false);
    }

    public void ToggleMovement() {
        GameObject.Find("CreatureHandler").GetComponent<CreatureMove>().ToggleMovement();
    }
}
