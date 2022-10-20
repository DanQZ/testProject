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
    PlayerScript PCScript;
    public GameObject playerCharacter;
    // Start is called before the first frame update

    // transform.position = vector of (x,y,z) of the object this script is attached to
    // gameObjectName.transform.position = a vector of (x,y,z)
    // gameObjectName.transform.position.x = float of x value of that vector

    int timer;
    int attackInterval;
    void Start()
    {
        thisFighterScript = enemyCharacter.GetComponent<FighterScript>();
        attackInterval = 180;
        timer = Time.frameCount + attackInterval;
        PCScript = playerCharacter.GetComponent<PlayerScript>();
        playerCharacter = PCScript.playerCharacter;
    }

    // Update is called once per frame
    void Update()
    {
        float playerX = playerCharacter.transform.position.x;
        float playerY = playerCharacter.transform.position.y;

        if(timer < Time.frameCount){
            thisFighterScript.Attack("arms");
            timer += attackInterval;
        }

        if(playerCharacter.transform.position.x-3>enemyCharacter.transform.position.x){
        thisFighterScript.Move(transform.right );
        thisFighterScript.TurnTo("right");
            return;
        }


        if(playerCharacter.transform.position.x+3<enemyCharacter.transform.position.x){
        thisFighterScript.Move(transform.right *-1f);
        thisFighterScript.TurnTo("left");
            return;
     }
    }
}
