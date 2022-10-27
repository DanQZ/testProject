using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanEnemyScript : MonoBehaviour
{
    public GameObject enemyCharacter;
    Transform enemyTran;
    public GameObject stanceHead;
    Transform stanceHeadTran;
    public FighterScript thisFighterScript;
    public GameObject playerFighter;
    public Transform playerHeadTran;
    int timer;
    int attackInterval;
    float targetDistanceToPlayer;
    float distanceToPlayer;
    void Start()
    {
        targetDistanceToPlayer = 5f;
        thisFighterScript = enemyCharacter.gameObject.GetComponent<FighterScript>();
        playerHeadTran = playerFighter.GetComponent<FighterScript>().stanceHeadTran;
        stanceHead = thisFighterScript.stanceHead;
        stanceHeadTran = stanceHead.transform;
        attackInterval = 180;
        timer = Time.frameCount + attackInterval;
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsPlayer();
        InitiateAttack();
    }
    void FacePlayer(){
        if(playerFighter.transform.position.x < enemyCharacter.transform.position.x){
            thisFighterScript.TurnTo("left");
        }
        if(playerFighter.transform.position.x > enemyCharacter.transform.position.x){
            thisFighterScript.TurnTo("right");
        }
    }
    void MoveTowardsPlayer()
    {
        targetDistanceToPlayer = 5f;
        distanceToPlayer = enemyCharacter.transform.position.x - playerFighter.transform.position.x;
        FacePlayer();
        if (distanceToPlayer > targetDistanceToPlayer)
        {
            thisFighterScript.Move(-1f * transform.right);
        }
        if (distanceToPlayer < 0 - targetDistanceToPlayer)
        {
            thisFighterScript.Move(transform.right);
        }

        if (playerHeadTran.position.y > stanceHeadTran.position.y)
        {
            thisFighterScript.MoveHead(1);
        }
        if (playerHeadTran.position.y < stanceHeadTran.position.y)
        {
            thisFighterScript.MoveHead(2);
        }
    }

    void InitiateAttack()
    {
        int randomSector = (int) Random.Range(0f,9f);
        int randomArmOrLeg = Mathf.RoundToInt(Random.Range(0f,1f));

        if (timer > Time.frameCount)
        {
            return;
        }

        if (Random.Range(0f, 1f) > 0.5f)
        {
            thisFighterScript.Attack("arms");
        }
        else
        {
            thisFighterScript.Attack("legs");
        }
        timer += attackInterval;

    }
}
