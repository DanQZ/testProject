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
    public bool controlWithMouse;
    void Awake()
    {
        PCTran = playerFighter.transform;
        stanceHeadTran = stanceHead.transform;
        PFScript = playerFighter.GetComponent<FighterScript>();
        PFScript.isPlayer = true;
        PFScript.isGhost = false;
        StartCoroutine(PFScript.InitBasedOnCharSettings());
        controlWithMouse = false; // defaults to one
    }
    void MoveIfInputWASD()
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
            PFScript.MoveBody(transform.right * -1f);
        }
        if (Input.GetKey("right"))
        {
            PFScript.MoveBody(transform.right);
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

    void AttackIfInputWASD()
    {
        if (PFScript.IsHeadWithinSectors() && PFScript.notInAttackAnimation)
        {
            if (Input.GetKey("space"))
            {
                PFScript.notInAttackAnimation = false;
                PFScript.Attack("arms");
                return;
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                PFScript.notInAttackAnimation = false;
                PFScript.Attack("legs");
                return;
            }
            if (Input.GetKey("f"))
            {
                PFScript.notInAttackAnimation = false;
                PFScript.Attack("groundslam");
                return;
            }
        }
    }

    void MoveIfInputMouse()
    //WASD moves head within allowed boundaries of size range
    {
        Vector3 mouseVector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        PFScript.MoveHeadAtPosition(mouseVector);

        if (Input.GetKey("a"))
        {
            PFScript.MoveBody(transform.right * -1f);
        }
        if (Input.GetKey("d"))
        {
            PFScript.MoveBody(transform.right);
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
    void AttackIfInputMouse()
    {
        if (PFScript.IsHeadWithinSectors() && PFScript.notInAttackAnimation)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                PFScript.notInAttackAnimation = false;
                PFScript.Attack("arms");
                return;
            }
            if (Input.GetKey(KeyCode.Mouse1))
            {
                PFScript.notInAttackAnimation = false;
                PFScript.Attack("legs");
                return;
            }
            if (Input.GetKey("f"))
            {
                PFScript.notInAttackAnimation = false;
                PFScript.Attack("groundslam");
                return;
            }
        }
    }
    void MouseControls()
    {
        AttackIfInputMouse();
        MoveIfInputMouse();
    }
    void WASDControls()
    {
        AttackIfInputWASD();
        MoveIfInputWASD();
    }
    // Update is called once per frame
    void Update()
    {
        //if (PFScript.controlsEnabled)
        {

            // turn these into coroutines later
            if (controlWithMouse)
            {
                MouseControls();
            }
            else
            {
                WASDControls();
            }
        }
    }
}
