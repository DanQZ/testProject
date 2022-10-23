using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public GameObject enemyCharacter;
    Transform enemyTran;
    public GameObject stanceHead;
    Transform stanceHeadTran;
    public FighterScript thisFighterScript;
    public GameObject playerFighter;
    public Transform playerHeadTran;
    // Start is called before the first frame update
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
        if (timer < Time.frameCount)
        {
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
    void MoveTowardsPlayer()
    {
        targetDistanceToPlayer = 5f;
        distanceToPlayer = enemyCharacter.transform.position.x - playerFighter.transform.position.x;

        if (distanceToPlayer > targetDistanceToPlayer)
        {
            thisFighterScript.TurnTo("left");
            thisFighterScript.Move(-1f * transform.right);
        }

        if (distanceToPlayer < 0 - targetDistanceToPlayer)
        {
            thisFighterScript.TurnTo("right");
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
}
