using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanEnemyAI : MonoBehaviour
{
    public GameObject enemyGhost;
    public GameObject enemyFollower;
    Transform enemyTran;
    public GameObject stanceHead;
    Transform stanceHeadTran;
    public FighterScript thisGhostScript; // script of the ghost fighter
    public GameObject playerFighter;
    public Transform playerHeadTran;
    int attackTimer;
    int stateTimer;
    int attackInterval;
    int stateInterval;
    int rangeChangeInterval;
    float targetDistanceToPlayer;
    float distanceToPlayer;
    string enemyState;
    public void StopAll(){
        StopAllCoroutines();
    }
    IEnumerator Start()
    {
        for (int i = 0; i < 10; i++) // waits a short time to make sure ghost does not glitch out
        {
            yield return null;
        }
        enemyState = "keepDistance";
        EnemyStateRandomizer();

        thisGhostScript = enemyGhost.gameObject.GetComponent<FighterScript>();
        playerHeadTran = playerFighter.GetComponent<FighterScript>().stanceHeadTran;
        stanceHead = thisGhostScript.stanceHead;
        stanceHeadTran = stanceHead.transform;
        attackInterval = (int)Random.Range(45, 75);
        attackTimer = Time.frameCount + attackInterval;
        stateInterval = (int)Random.Range(150f, 200f);
        stateTimer = Time.frameCount + stateInterval;
        rangeChangeInterval = (int)Random.Range(60, 80);
        StartCoroutine(RealUpdateAfterSeconds(0.1f));
    }

    IEnumerator RealUpdateAfterSeconds(float time){
        int frames = (int) (60f * time);
        for(int i = 0; i < frames; i++){
            yield return null;
        }
        StartCoroutine(RealUpdate());
    }

    IEnumerator RealUpdate()
    {
        while (true)
        {
            switch (enemyState)
            {
                case "keepDistance":
                    MoveTowardsTargetDistance();
                    break;
                case "attack":
                    MoveTowardsTargetDistance();
                    InitiateAttack();
                    break;
            }

            stateTimer++;
            if (stateTimer >= stateInterval)
            {
                EnemyStateRandomizer();
            }

            if (Time.frameCount % rangeChangeInterval == 0)
            { // change target distance
                TargetDistanceUpdate();
            }
            yield return null;
        }
    }
    void EnemyStateRandomizer()
    {
        float randomized = Random.Range(0f, 1f);
        if (randomized < 0.75f) // chance to attack state
        {
            enemyState = "attack";
        }
        else
        {
            enemyState = "keepDistance";
        }
        stateTimer = 0;
        TargetDistanceUpdate();
    }

    void TargetDistanceUpdate()
    {
        switch (enemyState)
        {
            case "keepDistance":
                targetDistanceToPlayer = Random.Range(6f, 8f);
                break;
            case "attack":
                targetDistanceToPlayer = Random.Range(3f, 5f);
                break;
        }
        //Debug.Log("Enemystate = " + enemyState + ", targetDistance = " + targetDistanceToPlayer);
    }
    void FacePlayer()
    {
        if (playerFighter.transform.position.x < enemyFollower.transform.position.x)
        {
            thisGhostScript.TurnTo("left");
        }
        if (playerFighter.transform.position.x > enemyFollower.transform.position.x)
        {
            thisGhostScript.TurnTo("right");
        }
    }
    void MoveTowardsTargetDistance()
    {
        distanceToPlayer = Mathf.Abs(enemyFollower.transform.position.x - playerFighter.transform.position.x);
        FacePlayer();
        if (thisGhostScript.facingRight) // facing right
        {
            if (distanceToPlayer > targetDistanceToPlayer)
            {
                thisGhostScript.Move(Vector3.right);
            }
            if (distanceToPlayer < targetDistanceToPlayer)
            {
                thisGhostScript.Move(-1f * Vector3.right);
            }
        }
        if (!thisGhostScript.facingRight) // facing left
        {
            if (distanceToPlayer < targetDistanceToPlayer)
            {
                thisGhostScript.Move(Vector3.right);
            }
            if (distanceToPlayer > targetDistanceToPlayer)
            {
                thisGhostScript.Move(-1f * Vector3.right);
            }
        }
    }

    void InitiateAttack()
    {
        int randomSector = (int)Random.Range(0f, 9f);
        int randomArmOrLeg = Mathf.RoundToInt(Random.Range(0f, 1f));

        if (attackTimer > Time.frameCount)
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

        attackTimer = Time.frameCount + attackInterval;
        StartCoroutine(GoToSectorThenAttack(randomSector, attackWith));
    }

    IEnumerator GoToSectorThenAttack(int sector, string attackWith)
    {
        while (thisGhostScript.GetHeadSector() != sector && thisGhostScript.notInAttackAnimation)
        {
            thisGhostScript.MoveHeadTowardsSector(sector);
            yield return null;
        }
        if (thisGhostScript.notInAttackAnimation)
        {
            thisGhostScript.Attack(attackWith);
        }
    }
}
