using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateManagerScript : MonoBehaviour
{
    public GameObject playerPrefab;
    private GameObject currentPlayer;
    public GameObject enemyManager;

    private EnemyManagerScript enemyManagerScript;
    public List<GameObject> allEnemies;
    public int score;
    public int highScore;
    public Text scoreCounterText;
    public Text gameOverText;
    public Text highScoreText;
    public Text chosenCharacterText;

    public GameObject MAIN_MENU_UI;
    public GameObject IN_GAME_UI;
    public GameObject GAME_OVER_UI;
    public GameObject CREDITS_UI;
    public List<GameObject> ALL_UI_LIST = new List<GameObject>();

    public string chosenCharacterType;

    void Awake()
    {
        ALL_UI_LIST.Add(MAIN_MENU_UI);
        ALL_UI_LIST.Add(IN_GAME_UI);
        ALL_UI_LIST.Add(GAME_OVER_UI);
        ALL_UI_LIST.Add(CREDITS_UI);


        transform.position = new Vector3(0f, 0f, 0f);

        Application.targetFrameRate = 60; // sets frame rate to 60fps, i will likely move this to another script later
        QualitySettings.vSyncCount = 0; // if vSync is on, it puts frame rate to the monitor's frame rate, ignoring the line above

        // sets up the EnemyManager
        enemyManagerScript = enemyManager.GetComponent<EnemyManagerScript>();
        enemyManagerScript.gameStateManager = this.gameObject;

        allEnemies = enemyManagerScript.allEnemiesList;

        highScore = 0;
        chosenCharacterType = "acolyte";
        DisplayUI("main menu");
    }

    public void StartNewGame()
    {
        DisplayUI("in game");
        // makes and returns new player object

        GameObject newPlayerPrefab = CreatePlayer();
        // sets up enemyManager and starts new game
        enemyManagerScript.playerPrefab = newPlayerPrefab;
        enemyManagerScript.NewGame();

        // start counting score
        score = 0;
        scoreCounterText.text = "Score: 0";
        chosenCharacterText.text = "Chosen: " + chosenCharacterType;
    }

    public GameObject CreatePlayer(){
        
        GameObject newPlayerPrefab = Instantiate(playerPrefab, transform.position, transform.rotation);
        currentPlayer = newPlayerPrefab;
        PlayerScript newPlayerScript = playerPrefab.GetComponent<PlayerScript>();
        FighterScript PFScript = newPlayerScript.playerFighter.GetComponent<FighterScript>();

        // updates the player fighter and makes reference to the fighter
        newPlayerScript.gameStateManager = transform.gameObject;
        PFScript.gameStateManager = transform.gameObject;
        PFScript.SetCharacterType(chosenCharacterType);
        
        return newPlayerPrefab;
    }
    

    public void SetChosenCharacter(string typeArg)
    {
        chosenCharacterType = typeArg;
        Debug.Log(chosenCharacterType + " class chosen");
        chosenCharacterText.text = "Chosen: " + chosenCharacterType;
    }

    public void UpdateHighScore()
    {
        if (score > highScore)
        {
            highScore = score;
        }
        highScoreText.text = "High Score: " + highScore;
    }

    public void ExitApplication()
    {
        Application.Quit();
    }

    public void EndGame()
    {
        enemyManagerScript.EndGame();
        Destroy(currentPlayer);
        DisplayUI("main menu");
    }

    public void GameOver()
    {
        DisplayUI("game over");
        gameOverText.text = "GAME OVER\nScore: " + score;
        enemyManagerScript.StopSpawningEnemies();
    }

    public void ShowCredits()
    {
        DisplayUI("credits");
    }

    public void HideAllUI()
    {
        foreach (GameObject UIGameObject in ALL_UI_LIST)
        {
            UIGameObject.SetActive(false);
        }
    }

    void DisplayUI(string buttonSetNameArg)
    {
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
        }
    }

    public void AddScore(int toAdd)
    {
        score += toAdd;
        UpdateScore();
    }

    void UpdateScore()
    {
        scoreCounterText.text = "Score: " + score;
    }
}
