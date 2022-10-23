using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackControllerScript : MonoBehaviour
{
    int nextTry;
    public GameObject enemyAttackPrefab;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        nextTry = Time.frameCount + 6;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.frameCount > nextTry){
            if(Random.Range(0f, 1f) < .25f){
                SpawnEnemyAttack();
            }
            nextTry = Time.frameCount + 6;
        }
    }

    void SpawnEnemyAttack(){
        GameObject newEnemyAttack = Instantiate(
            enemyAttackPrefab, 
            player.transform.position + new Vector3(
                Random.Range(-3f, 3f),
                Random.Range(-1.5f, 1.5f),
                0
            ), 
            transform.rotation);
            newEnemyAttack.GetComponent<AttackAreaScript>().creatorType = "enemy";
    }
}
