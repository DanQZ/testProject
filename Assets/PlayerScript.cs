using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameObject playerFighter;
    Transform PCTran;
    public GameObject stanceHead;
    Transform stanceHeadTran;
    public FighterScript PFScript;
    public GameObject gameStateManager;

    void Awake()
    {
        PCTran = playerFighter.transform;
        stanceHeadTran = stanceHead.transform;
        PFScript = playerFighter.GetComponent<FighterScript>();
        PFScript.isPlayer = true;
        PFScript.isGhost = false;
        StartCoroutine(PFScript.InitBasedOnCharSettings());
    }

    void MoveHeadIfInput()
    //WASD moves head within allowed boundaries of size range
    {
        float playerX = PCTran.position.x;
        float playerY = PCTran.position.y;
        float playerHeadX = stanceHeadTran.position.x;
        float playerHeadY = stanceHeadTran.position.y;

        if (Input.GetKey("w") || Input.GetKey("up"))
        {
            PFScript.MoveHead(1);
        }
        if (Input.GetKey("s") || Input.GetKey("down"))
        {
            PFScript.MoveHead(2);
        }
        if (Input.GetKey("a"))
        {
            PFScript.MoveHead(3);
        }
        if (Input.GetKey("d"))
        {
            PFScript.MoveHead(4);
        }

        if (Input.GetKey("left"))
        {
            PFScript.Move(transform.right * -1f);
        }
        if (Input.GetKey("right"))
        {
            PFScript.Move(transform.right);
        }
        if (Input.GetKey("q")) // face left
        {
            PFScript.TurnTo("left");
            return;
        }
        if (Input.GetKey("e")) // face right
        {
            PFScript.TurnTo("right");
            return;
        }
    }

    void AttackIfInput()
    {
        if (PFScript.IsHeadWithinSectors() && PFScript.controlsEnabled)
        {
            if (Input.GetKey("space"))
            {
                PFScript.controlsEnabled = false;
                PFScript.Attack("arms");
                return;
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                PFScript.controlsEnabled = false;
                PFScript.Attack("legs");
                return;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PFScript.controlsEnabled)
        {
            AttackIfInput();
            MoveHeadIfInput();
        }
    }
}
