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

    // timer text 
    public Text levelTimerTimeText;


    // UI objects that have all the necessary UI inside of them
    public GameObject UI_PARENT;
    public GameObject MAIN_MENU_UI;
    public GameObject IN_GAME_UI;
    public GameObject GAME_OVER_UI;
    public GameObject CREDITS_UI;
    public GameObject INSTRUCTIONS_UI;
    public GameObject OPTIONS_UI;
    public GameObject CHECKPOINT_UI;
    public GameObject UPDATELOG_UI;
    public List<GameObject> ALL_UI_LIST = new List<GameObject>();

    // choosing character stuff
    public string chosenCharacterType;
    public Text chosenCharacterText;
    public Text characterStatsText;

    public string selectedControls;

    IEnumerator pressEnterToContinue;
    IEnumerator checkpointCountdown;

    void Awake()
    {
        Debug.Log("screen width/height = " + Screen.width + ", " + Screen.height);
        ALL_UI_LIST.Add(MAIN_MENU_UI);
        ALL_UI_LIST.Add(IN_GAME_UI);
        ALL_UI_LIST.Add(GAME_OVER_UI);
        ALL_UI_LIST.Add(CREDITS_UI);
        ALL_UI_LIST.Add(INSTRUCTIONS_UI);
        ALL_UI_LIST.Add(OPTIONS_UI);
        ALL_UI_LIST.Add(CHECKPOINT_UI);
        ALL_UI_LIST.Add(UPDATELOG_UI);

        transform.position = new Vector3(0f, 0f, 0f);

        Application.targetFrameRate = 60; // sets frame rate to 60fps, i will likely move this to another script later
        QualitySettings.vSyncCount = 0; // if vSync is on, it puts frame rate to the monitor's frame rate, ignoring the line above

        // sets up the EnemyManager
        enemyManagerScript = enemyManager.GetComponent<EnemyManagerScript>();
        enemyManagerScript.gameStateManager = this.gameObject;

        selectedControls = "wasd";
        gameStarted = false;
        highScore = 0;
        chosenCharacterType = "acolyte";

        DisplayUI("main menu");
        checkpointCountdown = CountDownToNextCheckpoint(60f);

    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return)) // must be GetKeyUp or it will keep doing it
        {
            if (!gameStarted && MAIN_MENU_UI.activeInHierarchy) // these need te be Invoke or stuff gets weird (some bullshit that happens because it all happens on the same frame)
            {
                Debug.Log("enter pressed on main menu, starting game");
                StartNewGame();
                return;
                //Invoke("StartNewGame", 0.1f);
            }
            if (gameStarted && GAME_OVER_UI.activeInHierarchy)
            {
                Debug.Log("enter pressed on gameover, ending game");
                EndGame();
                return;
                //Invoke("EndGame", 0.1f);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!MAIN_MENU_UI.activeInHierarchy)
            {
                if (!gameStarted) // somewhere within the menus 
                {
                    DisplayUI("main menu");
                }
                if (gameStarted)
                {
                    EndGame();
                }
            }
        }
    }

    private void ResetCheckpointCountdown(float seconds)
    {
        if (checkpointCountdown != null)
        {
            StopCoroutine(checkpointCountdown);
        }
        checkpointCountdown = CountDownToNextCheckpoint(seconds);
    }

    private void StartCountingDownCheckpoint(float seconds)
    {
        ResetCheckpointCountdown(seconds);
        StartCoroutine(checkpointCountdown);
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
        gameStarted = true;
        StartNextLevel();
    }
    public void GameOver() // referenced on an event in FighterScript.Die() not sure why vscode thinks it has 0 references
    {
        ResetCheckpointCountdown(60f);

        DisplayUI("game over");
        gameOverText.text = "GAME OVER\nScore: " + score;
        enemyManagerScript.StopSpawningEnemies();
    }
    public void EndGame()// referenced by button objects, goes to main menu
    {
        enemyManagerScript.GameEnded();
        ResetCheckpointCountdown(60f);

        gameStarted = false;
        Destroy(currentPlayer);
        DisplayUI("main menu");
    }
    public void StartNextLevel()
    { // referenced by button objects
        DisplayUI("in game");

        // spawns enemies faster per level
        enemyManagerScript.IncreaseDifficulty();
        enemyManagerScript.StartSpawningEnemies();

        StartCountingDownCheckpoint(30f);

        currentPlayer.transform.position = new Vector3(0f, 0f, 0f);
        currentPFScript.ReplenishEnergy();
    }
    public void EndLevel()
    {
        currentPFScript.TakeHealing((int)Mathf.Ceil(((float)currentPFScript.maxhp) / 4f));
        enemyManagerScript.StopSpawningEnemies();
        enemyManagerScript.ClearAllEnemies();
        DisplayUI("checkpoint");

        StopCoroutine(checkpointCountdown);
    }

    public void SetControlsWASD()
    {
        selectedControls = "wasd";
    }
    public void SetControlsMouse()
    {
        selectedControls = "mouse";
    }
    public GameObject CreatePlayer()
    {
        GameObject newPlayerPrefab = Instantiate(playerPrefab, new Vector3(0f, 0f, 0f), transform.rotation);
        currentPlayer = newPlayerPrefab;
        PlayerScript newPlayerScript = currentPlayer.GetComponent<PlayerScript>();
        currentPFScript = newPlayerScript.playerFighter.GetComponent<FighterScript>();

        // updates the player fighter and makes reference to the fighter
        newPlayerScript.gameStateManager = transform.gameObject;
        currentPFScript.gameStateManager = transform.gameObject;
        currentPFScript.gameStateManagerScript = transform.gameObject.GetComponent<GameStateManagerScript>();
        currentPFScript.SetCharacterType(chosenCharacterType);

        // there might be more control schemes later idk
        switch (selectedControls.ToLower())
        {
            case "wasd":
                newPlayerScript.controlWithMouse = false;
                break;
            case "mouse":
                newPlayerScript.controlWithMouse = true;
                break;
        }

        return newPlayerPrefab;
    }

    private IEnumerator CountDownToNextCheckpoint(float secondsUntil)
    {
        int framesUntil = (int)(secondsUntil * 60f);
        Debug.Log(framesUntil + " frames until next checkpoint");
        for (int i = 0; i < framesUntil; i++)
        {
            yield return null;
            levelTimerTimeText.text = "" + ((framesUntil - i) / 60);
        }
        EndLevel();
    }

    public void SetChosenCharacter(string typeArg)
    {
        chosenCharacterType = typeArg;
        chosenCharacterText.text = "Chosen: " + chosenCharacterType;
    }

    public void CheckpointUpgrade(string upgradeArg)
    {
        switch (upgradeArg.ToLower())
        {
            case "health":
                currentPFScript.maxhp += 30;
                currentPFScript.hp += 30;
                currentPFScript.UpdateHealthBar();
                break;
            case "energy":
                currentPFScript.maxEnergy += 20;
                currentPFScript.UpdateEnergyBar();
                break;
            case "arm power":
                currentPFScript.ChangeMultiplier("arm power", "add", 0.1f);
                break;
            case "leg power":
                currentPFScript.ChangeMultiplier("leg power", "add", 0.1f);
                break;
            case "energyregen":
                currentPFScript.energyPerSecond += 5;
                break;
            case "movespeed":
                currentPFScript.ChangeMultiplier("speed", "add", 0.1f);
                break;
            case "heal":
                currentPFScript.ReplenishHealth();
                break;
        }
        StartNextLevel();
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
            case "options":
                OPTIONS_UI.SetActive(true);
                break;
            case "instructions":
                INSTRUCTIONS_UI.SetActive(true);
                break;
            case "checkpoint":
                UpdateStatsText();
                CHECKPOINT_UI.SetActive(true);
                break;
            case "update log":
                UPDATELOG_UI.SetActive(true);
                break;
        }
    }

    private void UpdateStatsText()
    {
        FighterScript cpfs = currentPFScript;

        characterStatsText.text = "Current Stats" + "\n"
            + "hp: " + cpfs.hp + "/" + cpfs.maxhp + "\n"
            + "max energy: " + cpfs.maxEnergy + "\n"
            + "energy/second: " + cpfs.energyPerSecond + "\n"
            + "arm power: " + cpfs.armPower + "\n"
            + "leg power: " + cpfs.legPower + "\n"
            + "speed: " + cpfs.speedMultiplier + "\n";
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
