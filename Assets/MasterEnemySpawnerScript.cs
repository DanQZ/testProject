using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterEnemySpawnerScript : MonoBehaviour
{
    public GameObject enemyWithGhostPrefab; // reference to the enemy prefab
    EnemyWithGhostScript EGScript;
    GameObject ghostFighter;
    GameObject enemyFighter;
    FighterScript ghostScript; // reference to the FighterScript inside the ghost inside enemyprefab
    FighterScript enemyFighterScript; // reference to the FighterScript inside the ghost inside enemyprefab

    public GameObject player; // reference to player prefab in the scene
    GameObject playerFighter; // reference to the fighter prefab inside the player prefab
    PlayerScript PCScript; // reference to the player script

    bool canSpawnEnemy = true;
    bool spawningEnemies = false;
    public List<GameObject> allEnemies = new List<GameObject>();

    // Start is called before the first frame update

    void Start()
    {
        PCScript = player.GetComponent<PlayerScript>(); // creates a reference
        playerFighter = PCScript.playerCharacter; // creates a reference to the fighter inside the player
    }

    // Update is called once per frame
    void Update()
    {
        // on pressing P, spawn 1 enemy per second to the right
        if (Input.GetKey("p") && canSpawnEnemy)
        {
            SpawnEnemyToTheRightOrLeft();
            canSpawnEnemy = false;
            Invoke("CanSpawnEnemyTrue", 1f);
        }
        if (Input.GetKeyDown("o") && canSpawnEnemy)
        {
            if (spawningEnemies)
            {
                StopAllCoroutines();
                spawningEnemies = false;
            }
            else
            {
                StartCoroutine(KeepSpawningEnemies());
                spawningEnemies = true;
            }

        }
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

        allEnemies.Add(newEnemyWithGhost);
    }
    void CanSpawnEnemyTrue()
    {
        canSpawnEnemy = true;
    }
    IEnumerator KeepSpawningEnemies()
    {
        int i = 0;
        int nextFrame = 0;
        while (true)
        {
            i++;
            if (i >= nextFrame)
            {
                SpawnEnemyToTheRightOrLeft();
                nextFrame = i + (int)(60f * Random.Range(3f, 6f));
            }
            yield return null;
        }
    }
}
