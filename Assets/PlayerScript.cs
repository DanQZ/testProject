using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameObject playerCharacter;
    Transform PCTran;
    public GameObject stanceHead;
    Transform stanceHeadTran;
    public FighterScript PCScript;

    // Start is called before the first frame update
    void Start()
    {
        PCTran = playerCharacter.transform;
        stanceHeadTran = stanceHead.transform;
        PCScript = playerCharacter.GetComponent<FighterScript>();
        PCScript.isPlayer = true;
        PCScript.isGhost = false;
        StartCoroutine(FixTheFuckingTagsHolyShitWhyTheFuckDoINeedThis());
    }
    IEnumerator FixTheFuckingTagsHolyShitWhyTheFuckDoINeedThis(){
        for(int i = 0; i < 2; i++){
            yield return null;
        }
        PCScript.UpdateBasedOnBools();
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
            PCScript.MoveHead(1);
        }
        if (Input.GetKey("s") || Input.GetKey("down"))
        {
            PCScript.MoveHead(2);
        }
        if (Input.GetKey("a"))
        {
            PCScript.MoveHead(3);
        }
        if (Input.GetKey("d"))
        {
            PCScript.MoveHead(4);
        }

        if (Input.GetKey("left"))
        {
            PCScript.Move(transform.right * -1f);
        }
        if (Input.GetKey("right"))
        {
            PCScript.Move(transform.right);
        }
        if (Input.GetKey("q")) // face left
        {
            PCScript.TurnTo("left");
            return;
        }
        if (Input.GetKey("e")) // face right
        {
            PCScript.TurnTo("right");
            return;
        }
    }

    void AttackIfInput()
    {
        if (PCScript.IsHeadWithinSectors() && PCScript.controlsEnabled)
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
        if (PCScript.controlsEnabled)
        {
            AttackIfInput();
            MoveHeadIfInput();
        }
    }
}
