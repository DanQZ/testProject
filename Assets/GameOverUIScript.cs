using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUIScript : MonoBehaviour
{
    public GameStateManagerScript GMScript;
    // Start is called before the first frame update
    public Text gameOverText;
    public Text scoreText;
    public TextMeshProUGUI classAndStatsText;
    public TextMeshProUGUI gameInfoText;

    public void UpdateAll()
    {
        gameOverText.text = "GAME OVER";
        UpdatePlayerStats();
        UpdateGameStats();
    }
    public void UpdatePlayerStats()
    {
        scoreText.text = "Score: " + GMScript.currentScore;
        classAndStatsText.text = "Class: " + GMScript.chosenCharacterType + "\n";
        classAndStatsText.text += GMScript.checkpointScript.GetStatsText();
    }
    public void UpdateGameStats()
    {
        gameInfoText.text = "" 
            +ToStatFormat("Damage dealt", GMScript.damageDealtCurrent)
            +ToStatFormat("Damage taken", GMScript.damageTakenCurrent)
            +ToStatFormatInt("Punches thrown", GMScript.armAttacksUsedCurrent)
            +ToStatFormatInt("Kicks thrown", GMScript.legAttacksUsedCurrent)
            +ToStatFormat("Checkpoint healing", GMScript.checkpointHealingCurrent)
            +ToStatFormat("Venom damage done", GMScript.poisonDamageDealtCurrent)
            +ToStatFormat("Colossus damage reduced", GMScript.colossusDamageReduced)
            +ToStatFormat("Vampirism healing", GMScript.vampirismHealingCurrent)
            +ToStatFormat("Explosive damage done", GMScript.explosiveDamageCurrent)
            ;

    }
    private string ToStatFormat(string start, float number)
    {
        return start + ": " + (Mathf.Abs(number).ToString("F0")) + "\n";
    }
    private string ToStatFormatInt(string start, int number)
    {
        return start + ": " + (Mathf.Abs(number)) + "\n";
    }

}
