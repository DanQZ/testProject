using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateManagerScript : MonoBehaviour {
    public int skillPoints = 0;
    public GameObject checkpointUpgradeButtons;
    public int currentFramesToCheckpoint = 0;
    float defaultSecondsToCheckpoint = 30f;

    // moveset editor ui
    public GameObject armMovesetButtons;
    public GameObject legMovesetButtons;
    public GameObject sectorTextParent;
    public GameObject sectorHighlightParent;
    public GameObject sectorButtonOutline;
    public string selectedPlayerFightingStyle;
    private int selectedSectorToChange;
    private string editingArmsOrLegs;
    private string selectedMoveToAdd;

    // stuff for in game
    public GameObject background;
    public GameObject playerPrefab;
    private GameObject currentPlayer;
    public FighterScript currentPFScript;
    public GameObject enemyManager;
    public EnemyManagerScript enemyManagerScript;
    public bool inGame;

    //tracking stuff
    public int currentScore;
    public int highScore;
    public int enemiesKilledCurrent;
    public float damageDealtCurrent;
    public float damageTakenCurrent;
    public float checkpointHealingCurrent;
    public float colossusDamageReduced;
    public float vampirismHealingCurrent;
    public float poisonDamageDealtCurrent;
    public float explosiveDamageCurrent;
    public int armAttacksUsedCurrent;
    public int legAttacksUsedCurrent;
    public int specialAttacksUsedCurrent;

    public Text randomTextObject; // used as intantiated
    public Text highScoreText;


    // UI objects that have all the necessary UI inside of them
    public GameObject UI_PARENT;
    public GameObject MAIN_MENU_UI;
    public GameObject IN_GAME_UI;
    public InGameUIScript inGameUIScript;
    public GameObject GAME_OVER_UI;
    public GameOverUIScript gameOverUIScript;
    public GameObject CREDITS_UI;
    public CreditsUIScript creditsUIScript;
    public GameObject INSTRUCTIONS_UI;
    public InstructionsUIScript instructionsUIScript;
    public GameObject OPTIONS_UI;
    public OptionsUIScript optionsUIScript;
    public GameObject CHECKPOINT_UI;
    public CheckpointScript checkpointScript;
    public GameObject UPDATELOG_UI;
    public GameObject SELECTSTYLE_UI;
    public List<GameObject> ALL_UI_LIST = new List<GameObject>();

    // choosing character stuff
    public string chosenCharacterType;
    public Text chosenCharacterText;
    public Text selectedMovesetText;

    public string selectedControls;

    IEnumerator pressEnterToContinue;
    IEnumerator checkpointCountdownCoroutine;

    string[] currentMovesetLegs = new string[9];
    string[] currentMovesetArms = new string[9];
    public List<GameObject> sectorButtons = new List<GameObject>();

    public List<Move> allMoves = new List<Move>();
    public List<SpecialAbility> allSpecialAiblities = new List<SpecialAbility>();

    void Awake() {
        Debug.Log("screen width/height = " + Screen.width + ", " + Screen.height);
        ALL_UI_LIST.Add(MAIN_MENU_UI);
        ALL_UI_LIST.Add(IN_GAME_UI);
        ALL_UI_LIST.Add(GAME_OVER_UI);
        ALL_UI_LIST.Add(CREDITS_UI);
        ALL_UI_LIST.Add(INSTRUCTIONS_UI);
        ALL_UI_LIST.Add(OPTIONS_UI);
        ALL_UI_LIST.Add(CHECKPOINT_UI);
        ALL_UI_LIST.Add(UPDATELOG_UI);
        ALL_UI_LIST.Add(SELECTSTYLE_UI);

        transform.position = new Vector3(0f, 0f, 0f);

        Application.targetFrameRate = 60; // sets frame rate to 60fps, i will likely move this to another script later
        QualitySettings.vSyncCount = 0; // if vSync is on, it puts frame rate to the monitor's frame rate, ignoring the line above

        // sets up the EnemyManager
        enemyManagerScript = enemyManager.GetComponent<EnemyManagerScript>();
        enemyManagerScript.gameStateManager = this.gameObject;

        selectedControls = "wasd";
        inGame = false;
        highScore = 0;
        chosenCharacterType = "acolyte";
        selectedPlayerFightingStyle = "muaythai";
        selectedSectorToChange = -1;
        editingArmsOrLegs = "legs";
        selectedMoveToAdd = "none";

        UpdateAvailableMoveList();
        SetDefaultMoveset();
        ChangeWhatMovesetToEdit();
        SelectFightingStyle("muaythai");

        ShowBackground(false);
        DisplayUI("main menu");
        checkpointCountdownCoroutine = CountDownToNextCheckpoint();

    }
    void UpdateAvailableMoveList() {

        allMoves.Clear();

        // available everywhere
        List<int> hookSectors = new List<int>();
        for (int i = 0; i < 9; i++) {
            hookSectors.Add(i);
        }
        allMoves.Add(new Move("hook", "arms", hookSectors));

        //available everywhere
        List<int> jabFastSectors = new List<int>();
        for (int i = 0; i < 9; i++) {
            jabFastSectors.Add(i);
        }
        allMoves.Add(new Move("fast jab", "arms", jabFastSectors));

        // only top middle and top right
        List<int> jabComboSectors = new List<int>();
        jabComboSectors.Add(7);
        jabComboSectors.Add(8);
        allMoves.Add(new Move("jab combo", "arms", jabComboSectors));

        // only lower sectors
        List<int> uppercutSectors = new List<int>();
        uppercutSectors.Add(0); uppercutSectors.Add(1); uppercutSectors.Add(2);
        allMoves.Add(new Move("uppercut", "arms", uppercutSectors));


        //KICKS


        // top front 2x2
        List<int> roundhouseSectors = new List<int>();
        for (int i = 3; i < 8; i++) {
            roundhouseSectors.Add(i);
        }
        allMoves.Add(new Move("roundhousekick", "legs", roundhouseSectors));

        // top front 2
        List<int> roundhouseHighSectors = new List<int>();
        roundhouseHighSectors.Add(7);
        roundhouseHighSectors.Add(8);
        allMoves.Add(new Move("roundhousekickhigh", "legs", roundhouseHighSectors));


        // top back 2x2
        List<int> pushKickSectors = new List<int>();
        pushKickSectors.Add(3);
        pushKickSectors.Add(4);
        pushKickSectors.Add(6);
        pushKickSectors.Add(7);
        allMoves.Add(new Move("pushkick", "legs", pushKickSectors));

        // bottom
        List<int> flyingKickSectors = new List<int>();
        flyingKickSectors.Add(0);
        flyingKickSectors.Add(1);
        flyingKickSectors.Add(2);
        allMoves.Add(new Move("flyingkick", "legs", flyingKickSectors));


        // front low 2
        List<int> kneeSectors = new List<int>();
        kneeSectors.Add(2);
        kneeSectors.Add(5);
        allMoves.Add(new Move("knee", "legs", kneeSectors));

    }

    // 1. check which moves are available for both selected sector and selected moveset
    public struct Move {
        public string moveName;
        public string type;
        public List<int> availableSectors;
        public Move(string nameArg, string typeArg, List<int> availableSectorsArg) {
            moveName = nameArg; type = typeArg; availableSectors = availableSectorsArg;
        }
    }

    public struct SpecialAbility {

        public string abilityName;
        public int abilityLevel;
        public SpecialAbility(string nameArg) {
            abilityName = nameArg; abilityLevel = 0;
        }
    }
    void Update() {
        if (Input.GetKeyUp(KeyCode.Return)) // must be GetKeyUp or it will keep doing it
        {
            if (!inGame && MAIN_MENU_UI.activeInHierarchy) // these need te be Invoke or stuff gets weird (some bullshit that happens because it all happens on the same frame)
            {
                Debug.Log("enter pressed on main menu, starting game");
                StartNewGame();
                return;
                //Invoke("StartNewGame", 0.1f);
            }
            if (inGame && GAME_OVER_UI.activeInHierarchy) {
                Debug.Log("enter pressed on gameover, ending game");
                EndGame();
                return;
                //Invoke("EndGame", 0.1f);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (!MAIN_MENU_UI.activeInHierarchy) {
                if (!inGame) // somewhere within the menus 
                {
                    DisplayUI("main menu");
                }
                if (inGame) {
                    EndGame();
                }
            }
        }
    }
    public void StartNewGame() {
        DisplayUI("in game");
        // makes and returns new player object
        currentPlayer = CreatePlayer();

        enemyManagerScript.playerPrefab = currentPlayer;
        enemyManagerScript.NewGame();

        // start counting score and other shit
        currentScore = 0;
        skillPoints = 0;


        enemiesKilledCurrent = 0;
        damageDealtCurrent = 0f;
        damageTakenCurrent = 0f;
        checkpointHealingCurrent = 0f;
        colossusDamageReduced = 0f;
        vampirismHealingCurrent = 0f;
        poisonDamageDealtCurrent = 0f;
        explosiveDamageCurrent = 0f;
        armAttacksUsedCurrent = 0;
        legAttacksUsedCurrent = 0;
        specialAttacksUsedCurrent = 0;

        inGameUIScript.UpdateScore();
        inGame = true;
        StartNextLevel();
    }
    public void GameOver() // referenced on an event in FighterScript.Die() not sure why vscode thinks it has 0 references
    {
        DisplayUI("game over");
        StopCoroutine(checkpointCountdownCoroutine);
        gameOverUIScript.UpdateAll();
        enemyManagerScript.StopSpawningEnemies();
    }
    public void EndGame()// referenced by button objects, goes to main menu
    {
        ShowBackground(false);
        enemyManagerScript.GameEnded();

        inGame = false;
        Destroy(currentPlayer);
        DisplayUI("main menu");
    }
    public void StartNextLevel() { // referenced by button objects
        ShowBackground(true);
        DisplayUI("in game");

        // spawns enemies faster per level
        enemyManagerScript.IncreaseDifficulty();
        enemyManagerScript.StartSpawningEnemies();

        StartCountingDownCheckpoint();

        currentPlayer.transform.position = new Vector3(0f, 0f, 0f);
        currentPFScript.ReplenishEnergy();
    }
    private void StartCountingDownCheckpoint() {
        if (checkpointCountdownCoroutine != null) {
            StopCoroutine(checkpointCountdownCoroutine);
        }
        // resets timer
        currentFramesToCheckpoint = (int)((defaultSecondsToCheckpoint + enemyManagerScript.difficultyLevel) * 60f);
        checkpointCountdownCoroutine = CountDownToNextCheckpoint();
        StartCoroutine(checkpointCountdownCoroutine);
    }

    public void EndLevel() {
        currentPFScript.TakeHealing((int)Mathf.Ceil(currentPFScript.maxhp / 4f), "checkpoint");
        enemyManagerScript.StopSpawningEnemies();
        enemyManagerScript.ClearAllEnemies();
        skillPoints++;
        DisplayUI("checkpoint");

        StopCoroutine(checkpointCountdownCoroutine);
    }
    public void EnterTrainingLevel() {
        DisplayUI("in game");
        // makes and returns new player object
        currentPlayer = CreatePlayer();

        // start counting score
        currentScore = 0;
        inGameUIScript.SetTrainingText();
        inGame = true;
    }
    public void SetControlsWASD() {
        selectedControls = "wasd";
    }
    public void SetControlsMouse() {
        selectedControls = "mouse";
    }
    public GameObject CreatePlayer() {
        GameObject newPlayerPrefab = Instantiate(playerPrefab, new Vector3(0f, 0f, 0f), transform.rotation);
        currentPlayer = newPlayerPrefab;
        PlayerScript newPlayerScript = currentPlayer.GetComponent<PlayerScript>();
        currentPFScript = newPlayerScript.playerFighter.GetComponent<FighterScript>();

        // updates the player fighter and makes reference to the fighter
        newPlayerScript.gameStateManager = transform.gameObject;
        currentPFScript.gameStateManager = transform.gameObject;
        currentPFScript.gameStateManagerScript = transform.gameObject.GetComponent<GameStateManagerScript>();
        currentPFScript.SetCharacterType(chosenCharacterType);
        currentPFScript.fightingStyle = selectedPlayerFightingStyle;

        ApplyNewMovesetToPlayer();
        // there might be more control schemes later idk
        switch (selectedControls.ToLower()) {
            case "wasd":
                newPlayerScript.controlWithMouse = false;
                break;
            case "mouse":
                newPlayerScript.controlWithMouse = true;
                break;
        }

        return newPlayerPrefab;
    }

    public void ApplyNewMovesetToPlayer() {
        currentPFScript.ApplyNewMoveset(currentMovesetArms, currentMovesetLegs);
    }

    private IEnumerator CountDownToNextCheckpoint() {
        while (currentFramesToCheckpoint > 0) {
            currentFramesToCheckpoint--;
            inGameUIScript.UpdateTimer();
            yield return null;
        }
        EndLevel();
    }

    public void SetChosenCharacter(string typeArg) {
        chosenCharacterType = typeArg;
        chosenCharacterText.text = "Chosen: " + chosenCharacterType;
    }

    public void CheckpointUpgrade(string upgradeArg) {
        if (skillPoints <= 0) {
            return;
        }
        skillPoints--;
        switch (upgradeArg.ToLower()) {
            case "health":
                currentPFScript.maxhp += 40f;
                currentPFScript.hp += 40f;
                currentPFScript.UpdateHealthBar();
                break;
            case "energy":
                currentPFScript.maxEnergy += 20f;
                currentPFScript.UpdateEnergyBar();
                break;
            case "arm power":
                currentPFScript.ChangeMultiplier("arm power", "add", 0.05f);
                break;
            case "leg power":
                currentPFScript.ChangeMultiplier("leg power", "add", 0.05f);
                break;
            case "energyregen":
                currentPFScript.energyPerSecond += 5f;
                break;
            case "movespeed":
                currentPFScript.ChangeMultiplier("speed", "add", 0.08f);
                break;
            case "heal":
                currentPFScript.ReplenishHealth();
                break;
            case "vampirism":
                currentPFScript.vampirismLevel++;
                break;
            case "poisoner":
                currentPFScript.poisonerLevel++;
                break;
            case "explosive":
                currentPFScript.explosiveLevel++;
                break;
            case "lightning":
                currentPFScript.lightningLevel++;
                break;
            case "colossus":
                currentPFScript.colossusLevel++;
                break;
        }
        checkpointScript.UpdateStatsText();
    }


    public void UpdateHighScore() {
        if (currentScore > highScore) {
            highScore = currentScore;
        }
        highScoreText.text = "High Score: " + highScore;
    }

    public void ExitApplication()// referenced by button objects
    {
        Application.Quit();
    }

    public void HideAllUI() {
        foreach (GameObject UIGameObject in ALL_UI_LIST) {
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
        if (ratio > 1.777f) {
            UIMult = UIHeightMult;
        }
        else {
            UIMult = UIWidthMult;
        }

        UI_PARENT.transform.localScale = new Vector3(UIMult, UIMult, 1f);

    }
    private void ShowBackground(bool trueOrFalse) {
        background.GetComponent<SpriteRenderer>().enabled = true;
    }
    public void DisplayUI(string buttonSetNameArg) {
        ScaleUIToScreen();
        HideAllUI();
        string buttonSetName = buttonSetNameArg.ToLower();

        switch (buttonSetName) {
            case "main menu":
                MAIN_MENU_UI.SetActive(true);
                UpdateHighScore();
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
                CHECKPOINT_UI.SetActive(true);
                checkpointScript.UpdateStatsText();
                ShowXCheckpointUpgrades(3);
                break;
            case "update log":
                UPDATELOG_UI.SetActive(true);
                break;
            case "select style":
                SELECTSTYLE_UI.SetActive(true);
                selectedMoveToAdd = "none";
                PurgeOldHighlights();
                UpdateCurrentSectorMovesText();
                break;
        }
    }
    public void ShowXCheckpointUpgrades(int x) {
        checkpointUpgradeButtons.SetActive(true);
        int numOfButtons = checkpointUpgradeButtons.transform.childCount;
        bool[] shownButtons = new bool[numOfButtons];

        for (int i = 0; i < numOfButtons; i++) {
            shownButtons[i] = false;
        }

        int buttonsAdded = 0;
        int numberLeft = numOfButtons;
        for (int i = 0; i < numOfButtons; i++) {
            int numberNeeded = x - buttonsAdded;
            if (Random.Range(0f, 1f) <= (float)numberNeeded / (float)numberLeft) {
                shownButtons[i] = true;
                buttonsAdded++;
            }
            numberLeft--;
        }


        for (int i = 0; i < numOfButtons; i++) {
            if (!shownButtons[i]) {
                checkpointUpgradeButtons.transform.GetChild(i).gameObject.SetActive(false);
            }
            else {
                checkpointUpgradeButtons.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
    public void AddScore(int toAdd) // referenced by FighterScript.Die()
    {
        currentScore += toAdd;
        inGameUIScript.UpdateScore();
    }

    // MOVESET EDITING CODE 
    private void SetDefaultMoveset() {
        currentMovesetLegs[0] = "flyingkick";
        currentMovesetLegs[1] = "flyingkick";
        currentMovesetLegs[2] = "knee";
        currentMovesetLegs[3] = "pushkick";
        currentMovesetLegs[4] = "roundhousekick";
        currentMovesetLegs[5] = "roundhousekick";
        currentMovesetLegs[6] = "pushkick";
        currentMovesetLegs[7] = "roundhousekick";
        currentMovesetLegs[8] = "roundhousekickhigh";

        currentMovesetArms[0] = "uppercut";
        currentMovesetArms[1] = "uppercut";
        currentMovesetArms[2] = "uppercut";
        currentMovesetArms[3] = "hook";
        currentMovesetArms[4] = "hook";
        currentMovesetArms[5] = "fast jab";
        currentMovesetArms[6] = "hook";
        currentMovesetArms[7] = "hook";
        currentMovesetArms[8] = "jab combo";
    }

    public void SelectFightingStyle(string style) {
        selectedPlayerFightingStyle = style;
        SELECTSTYLE_UI.GetComponent<SelectStyleUIScript>().DisplayOnlyMovesOfStyle(style, editingArmsOrLegs);
        DisplayUI("select style");
    }
    // a bunch of code to change the attacks of the player object
    public void SelectSector(int sectorArg) // 0 = bot back, 1 = bot center, bot forward, center back etc.
    {
        selectedSectorToChange = sectorArg;
        Debug.Log("selected to change " + GetSectorName(sectorArg));
        if (selectedMoveToAdd == "none") {
            return;
        }

        // checking if sector is valid for selected move
        bool validSector = false;
        foreach (Move move in allMoves) {
            if (move.moveName == selectedMoveToAdd) {
                foreach (int sectorItCanGoIn in move.availableSectors) {
                    if (sectorItCanGoIn == sectorArg && editingArmsOrLegs == move.type)
                        validSector = true;
                    break;
                }
            }
        }
        if (!validSector) {
            return;
        }

        switch (editingArmsOrLegs) {
            case "arms":
                currentMovesetArms[selectedSectorToChange] = selectedMoveToAdd;
                break;
            case "legs":
                currentMovesetLegs[selectedSectorToChange] = selectedMoveToAdd;
                break;
            case "special":
                break;
        }
        UpdateCurrentSectorMovesText();
    }
    public void SelectMove(string moveNameArg) // used by buttons 
    {
        selectedMoveToAdd = moveNameArg;
        HighlightAvailableSectors(GetMoveStruct(moveNameArg));
    }
    private void PurgeOldHighlights() {
        // purge old highlights
        foreach (Transform child in sectorHighlightParent.transform) {
            Destroy(child.gameObject);
        }
    }
    private void HighlightAvailableSectors(Move moveInput) {
        PurgeOldHighlights();
        string availableSectorString = "available sectors: ";
        foreach (int sector in moveInput.availableSectors) {
            availableSectorString += sector + ", ";
        }
        Debug.Log(availableSectorString);
        foreach (int sector in moveInput.availableSectors) {
            GameObject newHighlight = Instantiate(
                sectorButtonOutline,
                sectorButtons[sector].transform.position,
                transform.rotation);
            newHighlight.transform.SetParent(sectorHighlightParent.transform);
            newHighlight.transform.localScale = new Vector3(1.51f, 1.42f, 1.85f); // weird canvas bullshittery
        }
    }

    private void UpdateCurrentSectorMovesText() {
        selectedMovesetText.text = "Editing moveset: " + editingArmsOrLegs;
        foreach (Transform child in sectorTextParent.transform) {
            Destroy(child.gameObject);
        }
        int i = 0;
        foreach (GameObject button in sectorButtons) {
            GameObject newText = Instantiate(randomTextObject.gameObject, button.transform.position, transform.rotation);
            newText.transform.SetParent(sectorTextParent.transform);
            newText.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

            switch (editingArmsOrLegs) {
                case "arms":
                    newText.GetComponent<Text>().text = currentMovesetArms[i];
                    break;
                case "legs":
                    newText.GetComponent<Text>().text = currentMovesetLegs[i];
                    break;
                case "special":
                    break;
            }
            i++;
        }
    }

    private Move GetMoveStruct(string moveNameToFind) {
        for (int i = 0; i < allMoves.Count; i++) {
            if (allMoves[i].moveName == moveNameToFind) {
                Debug.Log("found " + allMoves[i].moveName);
                return allMoves[i];
            }
        }
        Debug.Log("NO MOVE OF NAME FOUND");
        return allMoves[0];
    }

    public void ChangeWhatMovesetToEdit() {
        PurgeOldHighlights();
        selectedMoveToAdd = "none";
        armMovesetButtons.SetActive(false);
        legMovesetButtons.SetActive(false);
        switch (editingArmsOrLegs) {
            case "arms":
                editingArmsOrLegs = "legs";
                legMovesetButtons.SetActive(true);
                break;
            case "legs":
                editingArmsOrLegs = "arms";
                armMovesetButtons.SetActive(true);
                break;
            case "special":
                editingArmsOrLegs = "special";
                break;
        }


        SelectFightingStyle(selectedPlayerFightingStyle);

        UpdateCurrentSectorMovesText();
    }

    private string GetSectorName(int sector) {
        string output = sector + ", INVALID SECTOR";
        switch (sector) {
            case 0:
                output = sector + ", low back";
                break;
            case 1:
                output = sector + ", low center";
                break;
            case 2:
                output = sector + ", low front";
                break;
            case 3:
                output = sector + ", center back";
                break;
            case 4:
                output = sector + ", center center";
                break;
            case 5:
                output = sector + ", center forward";
                break;
            case 6:
                output = sector + ", high back";
                break;
            case 7:
                output = sector + ", high center";
                break;
            case 8:
                output = sector + ", high forward";
                break;
        }
        return output;
    }

}
