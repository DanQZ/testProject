using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public GameObject enemyCharacter;
    Transform EnemyTran;
    public GameObject stanceHead;
    Transform stanceHeadTran;
    public FighterScript thisFighterScript;
    public GameObject player;
    // Start is called before the first frame update
    int timer;
    int attackInterval;
    float targetDistanceToPlayer;
    float distanceToPlayer;
    void Start()
    {
        targetDistanceToPlayer = 5f;
        thisFighterScript = enemyCharacter.gameObject.GetComponent<FighterScript>();
        attackInterval = 180;
        timer = Time.frameCount + attackInterval;
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsPlayer();
        if (timer < Time.frameCount)
        {
            thisFighterScript.Attack("arms");
            timer += attackInterval;
        }
    }
    void MoveTowardsPlayer()
    {
        targetDistanceToPlayer = 5f;
        distanceToPlayer = enemyCharacter.transform.position.x - player.transform.position.x;

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

    }
}
