using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIScript : MonoBehaviour
{
    public GameStateManagerScript GMScript;
    public Text levelTimerText;
    public Text scoreText;
    public void UpdateAll()
    {
        UpdateTimer();
        UpdateScore();
    }

    public void UpdateTimer()
    {
        levelTimerText.text = "Checkpoint " + GMScript.enemyManagerScript.difficultyLevel + " in " + (int)((float)GMScript.currentFramesToCheckpoint / 60f);
    }

    public void UpdateScore()
    {
        scoreText.text = "Score: " + GMScript.currentScore;
    }

    public void SetTrainingText()
    {
        scoreText.text = "Training";
        levelTimerText.text = "";
    }
}
