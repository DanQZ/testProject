using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUIScript : MonoBehaviour {
    GameStateManagerScript gameStateManagerScript;
    public GameObject redCircle;
    public GameObject acolyteButton;
    public GameObject tricksterButton;
    public GameObject brawlerButton;

    List<GameObject> allButtons = new List<GameObject>();
    void Awake() {
        allButtons.Add(acolyteButton);
        allButtons.Add(tricksterButton);
        allButtons.Add(brawlerButton);
    }
    // Update is called once per frame
    private void CircleSelected(GameObject buttonInput) {
        redCircle.transform.position = buttonInput.transform.position;
    }

    public void ShowOnlyUnlockedCharacters() {
        foreach (GameObject button in allButtons) {
            TurnOff(button);
        }
    }

    public void SetCharacter(string characterID) {

        switch (characterID) {
            case "acolyte":
                TurnOn(acolyteButton);
                break;
            case "trickster":
                TurnOn(tricksterButton);
                break;
            case "brawler":
                TurnOn(brawlerButton);
                break;
        }
        gameStateManagerScript.SetChosenCharacter(characterID);
    }
    private void TurnOn(GameObject buttonObject) {
        buttonObject.GetComponent<Button>().interactable = true;
    }
    private void TurnOff(GameObject buttonObject) {
        buttonObject.GetComponent<Button>().interactable = false;
    }
}
