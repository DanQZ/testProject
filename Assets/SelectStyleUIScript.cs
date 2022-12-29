using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectStyleUIScript : MonoBehaviour {
    public GameObject hookButton;
    public GameObject jabButton;
    public GameObject jabComboButton;
    public GameObject uppercutButton;

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

        allMoveButtons.Add(pushKickButton);
        allMoveButtons.Add(roundhouseKickButton);
        allMoveButtons.Add(highRoundhouseKickButton);
        allMoveButtons.Add(flyingKickButton);
        allMoveButtons.Add(kneeButton);
    }

    public void DisplayOnlyMovesOfStyle(string style, string armsOrLegs) {

        // hides all movebuttons
        foreach (GameObject button in allMoveButtons) {
            button.SetActive(false);
        }

        if (armsOrLegs == "arms") {
            Show(hookButton);
            Show(jabButton);
            Show(jabComboButton);
        }
        else {
            Show(pushKickButton);
        }

        switch (style) {
            case "muaythai":
                if (armsOrLegs == "arms") {
                    Show(uppercutButton);
                    // Show(elbowButton); to be added
                }
                else {
                    Show(kneeButton);
                    Show(roundhouseKickButton);
                }
                break;
            case "wingchun":
                if (armsOrLegs == "arms") {
                    Show(uppercutButton);
                    //Show(elbowButton);
                    //Show(chainPunchButton);
                }
                else {
                }
                break;
            case "taekwondo":

                if (armsOrLegs == "arms") {

                }
                else {
                    Show(flyingKickButton);
                    Show(roundhouseKickButton);
                    Show(highRoundhouseKickButton);
                    Show(kneeButton);
                }
                break;
        }
    }

    private void Show(GameObject button) {
        button.SetActive(true);
    }
}
