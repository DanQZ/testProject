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
    public Text scoreCounter;

    public GameObject MAIN_MENU_UI;
    public GameObject IN_GAME_UI;
    public GameObject GAME_OVER_UI;
    public List<GameObject> ALL_UI_LIST = new List<GameObject>();

    void Awake()
    {
        ALL_UI_LIST.Add(MAIN_MENU_UI);
        ALL_UI_LIST.Add(IN_GAME_UI);
        ALL_UI_LIST.Add(GAME_OVER_UI);


        transform.position = new Vector3(0f, 0f, 0f);

        Application.targetFrameRate = 60; // sets frame rate to 60fps, i will likely move this to another script later
        QualitySettings.vSyncCount = 0; // if vSync is on, it puts frame rate to the monitor's frame rate, ignoring the line above

        // sets up the EnemyManager
        enemyManagerScript = enemyManager.GetComponent<EnemyManagerScript>();
        enemyManagerScript.gameStateManager = this.gameObject;

        allEnemies = enemyManagerScript.allEnemiesList;

        DisplayUI("main menu");
    }

    public void StartNewGame()
    {
        DisplayUI("in game");
        // makes new player object
        GameObject newPlayerObject = Instantiate(playerPrefab, transform.position, transform.rotation);
        currentPlayer = newPlayerObject;
        PlayerScript newPlayerScript = playerPrefab.GetComponent<PlayerScript>();

        // updates the player fighter and makes reference to the fighter
        newPlayerScript.gameStateManager = transform.gameObject;
        newPlayerScript.PCScript = newPlayerScript.playerFighter.GetComponent<FighterScript>();
        newPlayerScript.PCScript.gameStateManager = transform.gameObject;

        // sets up enemyManager and starts new gam
        enemyManagerScript.player = newPlayerObject;
        enemyManagerScript.NewGame();

        // start counting score
        score = 0;
        scoreCounter.text = "Score: 0";
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
        enemyManagerScript.StopSpawningEnemies();
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
        GameObject buttonSet = null;

        switch (buttonSetName)
        {
            case "main menu":
                buttonSet = MAIN_MENU_UI;
                break;
            case "in game":
                buttonSet = IN_GAME_UI;
                break;
            case "game over":
                buttonSet = GAME_OVER_UI;
                break;
        }

        if (buttonSet != null)
        {
            buttonSet.SetActive(true);
        }
    }

    public void AddScore(int toAdd)
    {
        score += toAdd;
        UpdateScore();
    }

    void UpdateScore()
    {
        scoreCounter.text = "Score: " + score;
    }
}
