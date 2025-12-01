using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrivingGameController : MonoBehaviour
{
    public Camera mainCamera;
    public Transform playerCar;
    public SpriteRenderer playerSprite;
    public SpriteRenderer buddySprite;

    void Start() {
        if (GameManager.instance == null) { return; }
        var player = GameManager.instance.playerCreature;
        var buddy = GameManager.instance.buddyCreature;
        if (player != null) {
            playerSprite.sprite = player.GetComponent<SpriteRenderer>().sprite;
            playerSprite.color = player.GetComponent<SpriteRenderer>().color;
        }
        if (buddy != null) {
            buddySprite.sprite = buddy.GetComponent<SpriteRenderer>().sprite;
            buddySprite.color = buddy.GetComponent<SpriteRenderer>().color;
            buddySprite.gameObject.SetActive(true);
        }
    }

    void Update() {
        mainCamera.transform.position = new Vector3(playerCar.position.x, playerCar.position.y, -10);
    }
}
