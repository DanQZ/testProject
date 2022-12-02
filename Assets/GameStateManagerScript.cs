using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateManagerScript : MonoBehaviour
{
    public GameObject playerPrefab;
    private GameObject currentPlayer;
    private FighterScript currentPFScript;
    public GameObject enemyManager;

    public EnemyManagerScript enemyManagerScript;

    public bool gameStarted;

    //tracking scores
    public int score;
    public int highScore;
    public Text scoreCounterText;
    public Text gameOverText;
    public Text highScoreText;

    // in game timer 
    public int gameStartFrame;
    public int nextCheckPointFrame;
    public Text nextCheckPointTimerText;

    // UI objects that have all the necessary UI inside of them
    public GameObject UI_PARENT;
    public GameObject MAIN_MENU_UI;
    public GameObject IN_GAME_UI;
    public GameObject GAME_OVER_UI;
    public GameObject CREDITS_UI;
    public GameObject INSTRUCTIONS_UI;
    public GameObject CHECKPOINT_UI;
    public List<GameObject> ALL_UI_LIST = new List<GameObject>();

    // choosing character stuff
    public string chosenCharacterType;
    public Text chosenCharacterText;

    void Awake()
    {

        Debug.Log("screen width/height = " + Screen.width + ", " + Screen.height);
        ALL_UI_LIST.Add(MAIN_MENU_UI);
        ALL_UI_LIST.Add(IN_GAME_UI);
        ALL_UI_LIST.Add(GAME_OVER_UI);
        ALL_UI_LIST.Add(CREDITS_UI);
        ALL_UI_LIST.Add(INSTRUCTIONS_UI);
        ALL_UI_LIST.Add(CHECKPOINT_UI);

        transform.position = new Vector3(0f, 0f, 0f);

        Application.targetFrameRate = 60; // sets frame rate to 60fps, i will likely move this to another script later
        QualitySettings.vSyncCount = 0; // if vSync is on, it puts frame rate to the monitor's frame rate, ignoring the line above

        // sets up the EnemyManager
        enemyManagerScript = enemyManager.GetComponent<EnemyManagerScript>();
        enemyManagerScript.gameStateManager = this.gameObject;

        gameStarted = false;
        highScore = 0;
        chosenCharacterType = "acolyte";
        DisplayUI("main menu");
    }

    public void StartNewGame()
    {
        DisplayUI("in game");

        // makes and returns new player object
        currentPlayer = CreatePlayer();

        enemyManagerScript.playerPrefab = currentPlayer;
        enemyManagerScript.NewGame();

        // start counting score
        score = 0;
        scoreCounterText.text = "Score: " + score;

        StartNextLevel();
    }

    public GameObject CreatePlayer()
    {
        GameObject newPlayerPrefab = Instantiate(playerPrefab, transform.position, transform.rotation);
        currentPlayer = newPlayerPrefab;
        PlayerScript newPlayerScript = currentPlayer.GetComponent<PlayerScript>();
        currentPFScript = newPlayerScript.playerFighter.GetComponent<FighterScript>();

        // updates the player fighter and makes reference to the fighter
        newPlayerScript.gameStateManager = transform.gameObject;
        currentPFScript.gameStateManager = transform.gameObject;
        currentPFScript.gameStateManagerScript = transform.gameObject.GetComponent<GameStateManagerScript>();
        currentPFScript.SetCharacterType(chosenCharacterType);

        return newPlayerPrefab;
    }

    private IEnumerator CountDownToNextCheckpoint(float secondsUntil)
    {
        int framesUntil = (int)(secondsUntil * 60f);
        Debug.Log(framesUntil + " frames until next checkpoint");
        for (int i = 0; i < framesUntil; i++)
        {
            yield return null;
        }
        EndLevel();
    }

    public void SetChosenCharacter(string typeArg)
    {
        chosenCharacterType = typeArg;
        chosenCharacterText.text = "Chosen: " + chosenCharacterType;
    }

    public void CheckpointUpgrade(string upgradeArg){
        switch(upgradeArg){
            case "health":
                currentPFScript.maxhp += 10;
                currentPFScript.hp += 10;
                currentPFScript.UpdateHealthBar();
            break;
            case "energy":
                currentPFScript.maxEnergy += 20;
                currentPFScript.UpdateEnergyBar();
            break;
            case "damage":
                currentPFScript.ChangeMultiplier("damage", 0.1f, "add");
            break;
        }
        StartNextLevel();
    }

    public void EndLevel()
    {
        DisplayUI("checkpoint");
        currentPFScript.TakeHealing(currentPFScript.maxhp / 4);
        enemyManagerScript.StopSpawningEnemies();
        enemyManagerScript.ClearAllEnemies();
    }

    public void StartNextLevel()
    { // referenced by button objects
        DisplayUI("in game");
        enemyManagerScript.StartSpawningEnemies();
        StartCoroutine(CountDownToNextCheckpoint(10f));
    }

    public void UpdateHighScore()
    {
        if (score > highScore)
        {
            highScore = score;
        }
        highScoreText.text = "High Score: " + highScore;
    }

    public void ExitApplication()// referenced by button objects
    {
        Application.Quit();
    }

    public void EndGame()// referenced by button objects
    {
        enemyManagerScript.EndGame();
        Destroy(currentPlayer);
        DisplayUI("main menu");
    }

    public void GameOver() // referenced on an event in FighterScript.Die() not sure why vscode thinks it has 0 references
    {
        DisplayUI("game over");
        gameOverText.text = "GAME OVER\nScore: " + score;
        enemyManagerScript.StopSpawningEnemies();
    }

    public void HideAllUI()
    {
        foreach (GameObject UIGameObject in ALL_UI_LIST)
        {
            UIGameObject.SetActive(false);
        }
    }
    void ScaleUIToScreen()// used very time the ui is changed
    {
        // this might be an autistic way to do this but whatever it scales it correctly for 16:9 ratio
        float UIHeightMult = (((float)Screen.height) / 604f);
        float UIWidthMult = (((float)Screen.width) / 1074f);
        float ratio = ((float)Screen.width) / ((float)Screen.height);
        float UIMult = 0f;
        if (ratio > 1.777f)
        {
            UIMult = UIHeightMult;
        }
        else
        {
            UIMult = UIWidthMult;
        }

        UI_PARENT.transform.localScale = new Vector3(UIMult, UIMult, 1f);

    }
    public void DisplayUI(string buttonSetNameArg)
    {
        ScaleUIToScreen();
        HideAllUI();
        string buttonSetName = buttonSetNameArg.ToLower();

        switch (buttonSetName)
        {
            case "main menu":
                UpdateHighScore();
                MAIN_MENU_UI.SetActive(true);
                break;
            case "in game":
                IN_GAME_UI.SetActive(true);
                break;
            case "game over":
                GAME_OVER_UI.SetActive(true);
                break;
            case "credits":
                CREDITS_UI.SetActive(true);
                break;
            case "instructions":
                INSTRUCTIONS_UI.SetActive(true);
                break;
            case "checkpoint":
                CHECKPOINT_UI.SetActive(true);
                break;
        }
    }

    public void AddScore(int toAdd) // referenced by FighterScript.Die()
    {
        score += toAdd;
        UpdateScore();
    }

    void UpdateScore()
    {
        scoreCounterText.text = "Score: " + score;
    }
}
