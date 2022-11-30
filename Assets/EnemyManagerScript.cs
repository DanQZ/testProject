using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManagerScript : MonoBehaviour
{
    public GameObject gameStateManager;
    public GameObject enemyWithGhostPrefab; // reference to the enemy prefab
    EnemyWithGhostScript EGScript;
    GameObject ghostFighter;
    GameObject enemyFighter;
    FighterScript ghostScript; // reference to the FighterScript inside the ghost inside enemyprefab
    FighterScript enemyFighterScript; // reference to the FighterScript inside the ghost inside enemyprefab

    public GameObject player; // reference to player prefab in the scene
    GameObject playerFighter; // reference to the fighter prefab inside the player prefab
    PlayerScript PCScript; // reference to the player script
    public List<GameObject> allEnemiesList = new List<GameObject>();

    private IEnumerator spawnEnemiesCoroutine;

    // Start is called before the first frame update

    void Start()
    {
        spawnEnemiesCoroutine = KeepSpawningEnemies();
    }
    public void NewGame()
    {
        PCScript = player.GetComponent<PlayerScript>(); // creates a reference
        playerFighter = PCScript.playerFighter; // creates a reference to the fighter inside the player
        StartCoroutine(spawnEnemiesCoroutine);
    }

    private void ClearAllEnemies()
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

        // properly references the player
        EnemyWithGhostScript newEGScript = newEnemyWithGhost.GetComponent<EnemyWithGhostScript>();
        newEGScript.playerFighter = playerFighter;
        newEGScript.playerHead = playerFighter.GetComponent<FighterScript>().stanceHead;

        DanEnemyScript newEnemyScript = newEnemyWithGhost.GetComponent<DanEnemyScript>();
        newEnemyScript.playerFighter = playerFighter;
        newEnemyScript.playerHeadTran = playerFighter.GetComponent<FighterScript>().stanceHead.transform;

        newEGScript.enemyFighter.GetComponent<FighterScript>().gameStateManager = gameStateManager;

        allEnemiesList.Add(newEnemyWithGhost);
    }

    IEnumerator KeepSpawningEnemies() // spawns an enemy every 3-6 seconds
    {
        int nextFrame = Time.frameCount;
        while (true)
        {
            if (Time.frameCount >= nextFrame)
            {
                SpawnEnemyToTheRightOrLeft();
                nextFrame = Time.frameCount + (int)(60f * Random.Range(3f, 6f));
            }
            yield return null;
        }
    }
}
