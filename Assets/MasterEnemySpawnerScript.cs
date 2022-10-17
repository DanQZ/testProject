using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterEnemySpawnerScript : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject player;

    bool canSpawnEnemy = true;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("p") && canSpawnEnemy)
        {
            SpawnEnemyToTheRight();
            canSpawnEnemy = false;
            Invoke("CanSpawnEnemyTrue", 1f);
        }
    }
    void SpawnEnemyToTheRight()
    {
        GameObject newEnemy = Instantiate(enemyPrefab,
            player.transform.position + Vector3.right * 10f,
            transform.rotation
            );
        EnemyScript newEnemyScript = newEnemy.GetComponent<EnemyScript>();
        newEnemyScript.player = player;
    }
    void CanSpawnEnemyTrue()
    {
        canSpawnEnemy = true;
    }
}
