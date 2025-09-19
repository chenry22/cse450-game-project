using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static string kitchenGameManager = "GameManager"; // name for other scripts to reference

    public GameObject fileUploadUI;

    public void ActivateFileUpload()
    {
        fileUploadUI.SetActive(true);
    }

    public void DeactivateFileUpload()
    {
        // TODO: un-comment this, just for the purposes of testing station assignment
        // fileUploadUI.SetActive(false);
    }

    public void ToggleMovement()
    {
        GameObject.Find("CreatureHandler").GetComponent<CreatureMove>().ToggleMovement();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
