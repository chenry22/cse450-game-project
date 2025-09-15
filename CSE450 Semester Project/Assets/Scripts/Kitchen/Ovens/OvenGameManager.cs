using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// This script manages the oven minigame
// TODO: player movement pause needs to be updated here as well

public class OvenGameManager : MonoBehaviour {
    private float tickRate = 2.5f; // number of seconds between each cook update


    [Header("Game")]
    public OvenSliceController[] ovenSlices = new OvenSliceController[8]; // always 8 for our purposes...
    public GameObject pieIndicator;
    public TMP_Text emptyTxt;
    public GameObject gameUI; // nested UI, need this object to be active since it needs to cook in background

    private PizzaObject currentPie = null;
    private float timer = 0f;


    void Start() { gameUI.SetActive(false); }

    void Update() {
        // need to update even in the background
        if (currentPie != null) {
            timer += Time.deltaTime;
            if (timer >= tickRate) {
                for (int i = 0; i < ovenSlices.Length; i++) {
                    ovenSlices[i].CookSlice();
                }
                timer = 0f;

                if (!gameUI.activeSelf) {
                    // TODO: something here that shows progress when main UI is hidden
                }
            }
        }

        // ignore user input unless this game screen is actually active
        if (gameUI.activeSelf) {
            if (Input.GetKeyDown(KeyCode.Q)) {
                timer = 0f; // reset timer when transferring
                if (currentPie == null) {
                    // place current user pie in this slot if possible
                    var playerPie = GameObject.FindWithTag("Player").GetComponentInChildren<PizzaObject>();
                    if (playerPie != null) {
                        playerPie.transform.parent = this.gameObject.transform.parent; // give to station object
                        playerPie.transform.localPosition = Vector2.zero;

                        var cookLevels = playerPie.GetCookLevels();
                        for (int i = 0; i < ovenSlices.Length; i++)
                        {
                            ovenSlices[i].SetOvenSlice(cookLevels[i]);
                        }
                        emptyTxt.gameObject.SetActive(false);
                        pieIndicator.SetActive(true);
                        currentPie = playerPie;
                    }
                } else {
                    // save cook status
                    currentPie.SetCookLevels(ovenSlices.Select(slice => slice.GetCookLevel()).ToArray());
                    emptyTxt.gameObject.SetActive(true);
                    pieIndicator.SetActive(false);

                    // then remove from here and give to user
                    currentPie.transform.parent = GameObject.FindWithTag("Player").transform;
                    currentPie.transform.localPosition = new Vector2(0.6f, 0.2f);
                    currentPie = null; // let go of reference
                }
            } else if (currentPie != null) {
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                    // swap left
                    SwapOvenSlots(true);
                } else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
                    // swap right
                    SwapOvenSlots(false);
                }
            }
        }
    }

    private void SwapOvenSlots(bool left) {
        var currCookLevels = ovenSlices.Select(slice => slice.GetCookLevel()).ToArray();
        for (int i = 0; i < ovenSlices.Length; i++) {
            if (left) {
                var leftIndex = (i - 1 + ovenSlices.Length) % ovenSlices.Length;
                ovenSlices[i].SetOvenSlice(currCookLevels[leftIndex]);
            } else {
                var rightIndex = (i + 1) % ovenSlices.Length;
                ovenSlices[i].SetOvenSlice(currCookLevels[rightIndex]);
            }
        }
        timer = Math.Min(timer, tickRate / 2f); // reset to half speed to kind of adjust tick
        // timer = 0f; // reset timer? 
    }

    // basically toggles UI
    public void ShowOvenUI() {
        // TODO: replace with final movement script
        GameObject.FindWithTag("Player").GetComponent<TempPlayerMove>().ToggleMovement();
        if (currentPie == null) {
            emptyTxt.gameObject.SetActive(true);
            pieIndicator.SetActive(false);
        } else {
            emptyTxt.gameObject.SetActive(false);
            pieIndicator.SetActive(true);
        }
        gameUI.SetActive(true);
    }
    public void CloseOvenUI() {
        // TODO: replace with final movement script
        GameObject.FindWithTag("Player").GetComponent<TempPlayerMove>().ToggleMovement();
        gameUI.SetActive(false);
    }
}
