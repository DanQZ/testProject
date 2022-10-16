using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameObject playerCharacter;
    Transform PCTran;
    public GameObject stanceHead;
    Transform stanceHeadTran;
    FighterScript PCScript;
    // Start is called before the first frame update
    void Start()
    {
        PCTran = playerCharacter.transform;
        stanceHeadTran = stanceHead.transform;
        PCScript = playerCharacter.GetComponent<FighterScript>();
    }

    void MoveHeadIfInput()
    //WASD moves head within allowed boundaries of size range
    {
        float playerX = PCTran.position.x;
        float playerY = PCTran.position.y;
        float playerHeadX = stanceHeadTran.position.x;
        float playerHeadY = stanceHeadTran.position.y;

        if (Input.GetKey("w"))
        {
            if (playerHeadY < playerY + PCScript.reach)
            {
                stanceHeadTran.position += Vector3.up * PCScript.speed;
            }
        }
        if (Input.GetKey("s"))
        {
            if (playerHeadY > playerY - PCScript.reach)
            {
                stanceHeadTran.position += Vector3.down * PCScript.speed;
            }
        }
        if (Input.GetKey("a"))
        {
            if (playerHeadX > playerX - PCScript.reach)
            {
                stanceHeadTran.position += Vector3.left * PCScript.speed;
            }
        }
        if (Input.GetKey("d"))
        {
            if (playerHeadX < playerX + PCScript.reach)
            {
                stanceHeadTran.position += Vector3.right * PCScript.speed;
            }
        }
        if (Input.GetKey("up"))
        {
            if (PCScript.controlsEnabled)
            {
                StartCoroutine(PCScript.Jump(10f));
            }
        }

        if (Input.GetKey("left"))
        {
            if (PCScript.facingRight)
            {
                PCScript.TurnTo("left");
            }
        }
        if (Input.GetKey("right"))
        {
            if (!PCScript.facingRight)
            {
                PCScript.TurnTo("right");
            }
        }
    }

    void AttackIfInput()
    {
        if (PCScript.IsHeadWithinSectors())
        {
            if (Input.GetKey("space"))
            {
                PCScript.controlsEnabled = false;
                PCScript.Attack("arms");
                return;
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                PCScript.controlsEnabled = false;
                PCScript.Attack("legs");
                return;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(PCScript.controlsEnabled){
            AttackIfInput();
            MoveHeadIfInput();
        }
    }
}
