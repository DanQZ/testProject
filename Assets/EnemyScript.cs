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
    void Start()
    {
        thisFighterScript = enemyCharacter.gameObject.GetComponent<FighterScript>();
        attackInterval = 180;
        timer = Time.frameCount + attackInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if(timer < Time.frameCount){
            thisFighterScript.Attack("arms");
            timer += attackInterval;
        }
    }
}
