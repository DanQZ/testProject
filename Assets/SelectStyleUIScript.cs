using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectStyleUIScript : MonoBehaviour {
    public GameObject hookButton;
    public GameObject jabButton;
    public GameObject jabComboButton;
    public GameObject uppercutButton;
    public GameObject chainPunchButton;
    public GameObject elbowButton;

    public GameObject pushKickButton;
    public GameObject roundhouseKickButton;
    public GameObject highRoundhouseKickButton;
    public GameObject flyingKickButton;
    public GameObject kneeButton;
    public List<GameObject> allMoveButtons = new List<GameObject>();

    void Awake() {
        allMoveButtons.Add(hookButton);
        allMoveButtons.Add(jabButton);
        allMoveButtons.Add(jabComboButton);
        allMoveButtons.Add(uppercutButton);
        allMoveButtons.Add(chainPunchButton);
        allMoveButtons.Add(elbowButton);

        allMoveButtons.Add(pushKickButton);
        allMoveButtons.Add(roundhouseKickButton);
        allMoveButtons.Add(highRoundhouseKickButton);
        allMoveButtons.Add(flyingKickButton);
        allMoveButtons.Add(kneeButton);
    }

    public void DisplayOnlyMovesOfStyle(string style, string armsOrLegs) {

        // hides all movebuttons
        foreach (GameObject button in allMoveButtons) {
            TurnOff(button);
        }

        if (armsOrLegs == "arms") {
            TurnOn(hookButton);
            TurnOn(jabButton);
            TurnOn(jabComboButton);
        }
        else {
            TurnOn(pushKickButton);
        }

        switch (style) {
            case "muaythai":
                if (armsOrLegs == "arms") {
                    TurnOn(uppercutButton);
                    TurnOn(elbowButton);
                }
                else {
                    TurnOn(kneeButton);
                    TurnOn(roundhouseKickButton);
                }
                break;
            case "wingchun":
                if (armsOrLegs == "arms") {
                    TurnOn(uppercutButton);
                    TurnOn(elbowButton);
                    TurnOn(chainPunchButton);
                }
                else {
                }
                break;
            case "taekwondo":

                if (armsOrLegs == "arms") {

                }
                else {
                    TurnOn(flyingKickButton);
                    TurnOn(roundhouseKickButton);
                    TurnOn(highRoundhouseKickButton);
                    TurnOn(kneeButton);
                }
                break;
        }
    }

    private void TurnOn(GameObject buttonObject) {
        buttonObject.GetComponent<Button>().interactable = true;
    }
    private void TurnOff(GameObject buttonObject) {
        buttonObject.GetComponent<Button>().interactable = false;
    }
}
