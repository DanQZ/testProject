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
            SpawnEnemyToTheRight();
            canSpawnEnemy = false;
            Invoke("CanSpawnEnemyTrue", 1f);
        }
    }
    // spawn 1 enemy 10 units to the right
    void SpawnEnemyToTheRight()
    {
        GameObject newEnemyWithGhost = Instantiate(enemyWithGhostPrefab,
            playerFighter.transform.position + Vector3.right * -10f,
            transform.rotation
            );
        
        // properly references the player
        EnemyWithGhostScript newEGScript = newEnemyWithGhost.GetComponent<EnemyWithGhostScript>();
        newEGScript.playerFighter = playerFighter;
        newEGScript.playerHead = playerFighter.GetComponent<FighterScript>().stanceHead;

        DanEnemyScript newEnemyScript = newEnemyWithGhost.GetComponent<DanEnemyScript>();
        newEnemyScript.playerFighter = playerFighter;
        newEnemyScript.playerHeadTran = playerFighter.GetComponent<FighterScript>().stanceHead.transform;
    }
    void CanSpawnEnemyTrue()
    {
        canSpawnEnemy = true;
    }
}
