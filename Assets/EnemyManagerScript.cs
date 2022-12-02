using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManagerScript : MonoBehaviour
{
    public GameObject gameStateManager;
    public GameObject enemyWithGhostPrefab; // reference to the enemy prefab
    public int numOfEnemiesSpawnedThisGame;
    EnemyWithGhostScript EGScript;
    GameObject ghostFighter;
    GameObject enemyFighter;
    FighterScript ghostScript; // reference to the FighterScript inside the ghost inside enemyprefab
    FighterScript enemyFighterScript; // reference to the FighterScript inside the ghost inside enemyprefab

    public GameObject playerPrefab; // reference to player prefab in the scene
    GameObject playerFighter; // reference to the fighter prefab inside the player prefab
    PlayerScript PCScript; // reference to the player script
    public List<GameObject> allEnemiesList = new List<GameObject>();

    private IEnumerator spawnEnemiesCoroutine;
    private int enemiesSpawnedThisFrame;

    public int difficultyLevel;

    // Start is called before the first frame update

    void Start()
    {
        difficultyLevel = 0;
        spawnEnemiesCoroutine = KeepSpawningEnemies(5f, 10f); // begins with long intervals
        numOfEnemiesSpawnedThisGame = 0;
    }

    public void NewGame()
    {
        difficultyLevel = 0;
        numOfEnemiesSpawnedThisGame = 0;
        PCScript = playerPrefab.GetComponent<PlayerScript>(); // creates a reference
        playerFighter = PCScript.playerFighter; // creates a reference to the fighter inside the player
    }
    public List<GameObject> GetAllEnemiesList()
    {
        RemoveDestroyedEnemiesFromList();
        return allEnemiesList;
    }
    public void RemoveDestroyedEnemiesFromList()
    {
        allEnemiesList.RemoveAll(enemy => enemy == null);
    }

    public void ClearAllEnemies()
    {
        foreach (GameObject enemy in allEnemiesList)
        {
            Destroy(enemy);
        }
    }
    public void EndGame()
    {
        StopSpawningEnemies();
        ClearAllEnemies();
    }

    public void IncreaseDifficulty()
    {
        // in case it is still running
        StopCoroutine(spawnEnemiesCoroutine);

        // every level, average enemy spawn interval goes down 1 second
        float rangeMinSeconds = 5f - ((float)difficultyLevel);
        float rangeMaxSeconds = 10f - ((float)difficultyLevel);
        
        // prevents enemies from spawning more than once per second
        if (rangeMinSeconds < 1f)
        {
            rangeMinSeconds = 1f;
        }
        if (rangeMaxSeconds < 1f)
        {
            rangeMaxSeconds = 1f;
        }
        // new spawn intervals
        spawnEnemiesCoroutine = KeepSpawningEnemies(rangeMinSeconds, rangeMaxSeconds);
        difficultyLevel++;
    }

    public void StartSpawningEnemies()
    {
        // in case it is still running
        StopCoroutine(spawnEnemiesCoroutine);

        StartCoroutine(spawnEnemiesCoroutine);
    }

    public void StopSpawningEnemies()
    {
        StopCoroutine(spawnEnemiesCoroutine);
    }

    // spawn 1 enemy 10 units to the right
    void SpawnEnemyToTheRightOrLeft()
    {
        float leftRight = 10f;
        if (Random.Range(0f, 1f) > 0.5f)
        {
            leftRight = -10f;
        }
        GameObject newEnemyWithGhost = Instantiate(enemyWithGhostPrefab,
            new Vector3(0, 0, 0) + Vector3.right * leftRight,
            transform.rotation
            );

        // properly references the player for the ghost/enemyFighter object
        EnemyWithGhostScript newEGScript = newEnemyWithGhost.GetComponent<EnemyWithGhostScript>();
        newEGScript.playerFighter = playerFighter;
        newEGScript.playerHead = playerFighter.GetComponent<FighterScript>().stanceHead;
        newEGScript.myManagerObject = transform.gameObject;
        newEGScript.myManagerScript = transform.gameObject.GetComponent<EnemyManagerScript>();

        // the enemy AI referencing 
        DanEnemyScript newEnemyScript = newEnemyWithGhost.GetComponent<DanEnemyScript>();
        newEnemyScript.playerFighter = playerFighter;
        newEnemyScript.playerHeadTran = playerFighter.GetComponent<FighterScript>().stanceHead.transform;

        // the non-ghost part 
        FighterScript newEGSFighterScript = newEGScript.enemyFighter.GetComponent<FighterScript>();

        newEGSFighterScript.gameStateManager = gameStateManager;
        newEGSFighterScript.gameStateManagerScript = gameStateManager.GetComponent<GameStateManagerScript>();

        allEnemiesList.Add(newEnemyWithGhost);
        numOfEnemiesSpawnedThisGame++;
    }

    IEnumerator KeepSpawningEnemies(float rangeMinArgSeconds, float rangeMaxArgSeconds) // spawns an enemy every x to y seconds
    {
        float rangeMin = rangeMinArgSeconds;
        float rangeMax = rangeMaxArgSeconds;
        int nextFrame = Time.frameCount + 3 * 60;
        while (true)
        {
            if (Time.frameCount >= nextFrame)
            {
                SpawnEnemyToTheRightOrLeft();
                nextFrame = Time.frameCount
                    + (int)(
                        60f * Random.Range(rangeMin, rangeMax)
                        );
            }
            yield return null;
        }
    }
}
