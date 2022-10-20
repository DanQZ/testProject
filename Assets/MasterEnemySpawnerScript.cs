using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterEnemySpawnerScript : MonoBehaviour
{
    public GameObject enemyPrefab; // reference to the enemy prefab
    FighterScript EnemyCharScript; // reference to the EnemyScript inside the enemy prefab
    
    public GameObject player; // reference to player prefab
    GameObject playerCharacter; // reference to the fighter prefab inside the player prefab
    PlayerScript PCScript; // reference to the player script

    bool canSpawnEnemy = true;
    // Start is called before the first frame update
    void Start()
    {
        PCScript = player.GetComponent<PlayerScript>(); // creates a reference
        playerCharacter = PCScript.playerCharacter; // creates a reference to the fighter inside the player
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
        GameObject newEnemy = Instantiate(enemyPrefab,
            playerCharacter.transform.position + Vector3.right * 10f,
            transform.rotation
            );
        
        // properly references the player
        EnemyScript newEnemyScript = newEnemy.GetComponent<EnemyScript>();
        newEnemyScript.player = playerCharacter;
    }
    void CanSpawnEnemyTrue()
    {
        canSpawnEnemy = true;
    }
}
