using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CheckpointScript : MonoBehaviour
{
    public GameStateManagerScript GMScript;
    public Text checkpointInfoText;
    public Text characterStatsText;
    public void UpdateStatsText()
    {
        checkpointInfoText.text =
        "Congrations you reached checkpoint"
        + "\n"
        + "You have " + GMScript.skillPoints + " skillpoints";
        FighterScript cpfs = GMScript.currentPFScript;
        EnemyManagerScript enemyManagerScript = GMScript.enemyManagerScript;

        characterStatsText.text =
            "Checkpoint " + enemyManagerScript.difficultyLevel + " stats" + "\n"
            + "hp: " + (int)cpfs.hp + "/" + (int)cpfs.maxhp + "\n"
            + "max energy: " + (int)cpfs.maxEnergy + "\n"
            + "energy/second: " + (int)cpfs.energyPerSecond + "\n"
            + "arm power: " + cpfs.armPower + "\n"
            + "leg power: " + cpfs.legPower + "\n"
            + "speed: " + cpfs.speedMultiplier + "\n"
            + "\n"
            + "Vampiric Style Level " + cpfs.vampirismLevel + "\n"
            + "Snake Style Level " + cpfs.poisonerLevel + "\n"
            + "Explosive Style Level " + cpfs.explosiveLevel + "\n"
            + "Lightning Style Level " + cpfs.lightningLevel + "\n"
            + "Colossus Style Level " + cpfs.colossusLevel + "\n"
            ;
    }

    public string GetStatsText(){
        UpdateStatsText();
        return characterStatsText.text;
    }


}
