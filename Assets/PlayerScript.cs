using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameObject playerCharacter;
    Transform PCTran;
    public GameObject playerHead;
    Transform playerHeadTran;
    FighterScript PCScript;
    // Start is called before the first frame update
    void Start()
    {
        PCTran = playerCharacter.transform;
        playerHeadTran = playerHead.transform;
        PCScript = playerCharacter.GetComponent<FighterScript>();
    }

    void MoveHeadIfInput()
    //WASD moves head within allowed boundaries of size range
    {
        float playerX = PCTran.position.x;
        float playerY = PCTran.position.y;
        float playerHeadX = playerHead.transform.position.x;
        float playerHeadY = playerHead.transform.position.y;

        if (Input.GetKey("w"))
        {
            if (playerHeadY < playerY + PCScript.reach)
            {
                playerHead.transform.position += Vector3.up * PCScript.speed;
            }
        }
        if (Input.GetKey("s"))
        {
            if (playerHeadY > playerY - PCScript.reach)
            {
                playerHead.transform.position += Vector3.down * PCScript.speed;
            }
        }
        if (Input.GetKey("a"))
        {
            if (playerHeadX > playerX - PCScript.reach)
            {
                playerHead.transform.position += Vector3.left * PCScript.speed;
            }
        }
        if (Input.GetKey("d"))
        {
            if (playerHeadX < playerX + PCScript.reach)
            {
                playerHead.transform.position += Vector3.right * PCScript.speed;
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
        if (PCScript.controlsEnabled && PCScript.IsHeadWithinSectors())
        {
            if (Input.GetKey("space"))
            {
                PCScript.controlsEnabled = false;
                PCScript.Attack("arms");
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                PCScript.controlsEnabled = false;
                PCScript.Attack("legs");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        MoveHeadIfInput();
        AttackIfInput();
    }
}
