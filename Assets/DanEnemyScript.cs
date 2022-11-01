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
        if(Time.frameCount % 70 == 0){ // change target distance
            targetDistanceToPlayer = Random.Range(3.5f,5.5f);
        }
    }
    void FacePlayer()
    {
        if (playerFighter.transform.position.x < enemyCharacter.transform.position.x)
        {
            thisFighterScript.TurnTo("left");
        }
        if (playerFighter.transform.position.x > enemyCharacter.transform.position.x)
        {
            thisFighterScript.TurnTo("right");
        }
    }
    void MoveTowardsPlayer()
    {
        distanceToPlayer = enemyCharacter.transform.position.x - playerFighter.transform.position.x;
        FacePlayer();
        if (distanceToPlayer > targetDistanceToPlayer || distanceToPlayer > 0 - targetDistanceToPlayer)
        {
            thisFighterScript.Move(-1f * transform.right);
        }
        if (distanceToPlayer < 0 - targetDistanceToPlayer || distanceToPlayer < targetDistanceToPlayer)
        {
            thisFighterScript.Move(transform.right);
        }
    }

    void InitiateAttack()
    {
        int randomSector = (int)Random.Range(0f, 9f);
        int randomArmOrLeg = Mathf.RoundToInt(Random.Range(0f, 1f));

        if (timer > Time.frameCount)
        {
            return;
        }

        string attackWith = "";
        if (Random.Range(0f, 1f) > 0.5f)
        {
            attackWith = "arms";
        }
        else
        {
            attackWith = "legs";
        }
        timer += attackInterval;
        StartCoroutine(GoToSectorThenAttack(randomSector, attackWith));
    }

    IEnumerator GoToSectorThenAttack(int sector, string attackWith)
    {
        while (thisFighterScript.GetHeadSector() != sector && thisFighterScript.controlsEnabled)
        {
            thisFighterScript.MoveHeadTowardsSector(sector);
            yield return null;
        }
        if(thisFighterScript.controlsEnabled){
            thisFighterScript.Attack(attackWith);
        }
    }
}
