using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    float speed;
    public int hp;
    public int nextAttackAvailableFrame;
    public int attackInterval; //frames between each attack
    bool facingRight;
    public GameObject playerAttackArea;

    public GameObject playerHead;
    public GameObject playerTorsoTop;
    public GameObject playerTorsoBottom;
    public GameObject playerArmTop1;
    public GameObject playerArmBottom1;
    private Transform playerArmBottom1Tran;
    public GameObject playerArmTop2;
    public GameObject playerArmBottom2;
    private Transform playerArmBottom2Tran;
    public GameObject playerThigh1;
    public GameObject playerCalf1;
    private Transform playerCalf1Tran;
    public GameObject playerThigh2;
    public GameObject playerCalf2;
    private Transform playerCalf2Tran;

    public GameObject stanceHand1;
    private Transform stanceHand1Tran;
    public GameObject stanceHand2;
    private Transform stanceHand2Tran;
    public GameObject stanceFoot1;
    private Transform stanceFoot1Tran;
    public GameObject stanceFoot2;
    private Transform stanceFoot2Tran;

    public GameObject spriteObject;
    SpriteRenderer playerSprite;

    // public List<string> movementQueue = new List<string>();

    public float reach;

    bool controlsEnabled;

    void InitTransforms()
    {
        playerArmBottom1Tran = playerArmBottom1.transform;
        playerArmBottom2Tran = playerArmBottom2.transform;
        playerCalf1Tran = playerCalf1.transform;
        playerCalf2Tran = playerCalf2.transform;
        stanceHand1Tran = stanceHand1.transform;
        stanceHand2Tran = stanceHand2.transform;
        stanceFoot1Tran = stanceFoot1.transform;
        stanceFoot2Tran = stanceFoot2.transform;
    }
    // Start is called before the first frame update
    void Start()
    {
        InitTransforms();
        controlsEnabled = true;
        playerSprite = spriteObject.gameObject.GetComponent<SpriteRenderer>();
        facingRight = true;
        hp = 100;
        Application.targetFrameRate = 60; // sets frame rate to 60fps, i will likely move this to another script later
        speed = 3f / 60f; // x units per 60 frames
        nextAttackAvailableFrame = Time.frameCount;
        attackInterval = 60; //once per x frames
        reach = 1f;
    }

    void MoveTowardsDefaultStance()
    {
        stanceHand1Tran.position = playerHead.transform.position + Vector3.right * 0.25f + Vector3.down * 1f;
        stanceHand2Tran.position = playerHead.transform.position + Vector3.right * 1.5f + Vector3.down * 0.5f;

        playerCalf1Tran.position = transform.position + Vector3.down * 4f - Vector3.right * 1f;
        playerCalf2Tran.position = transform.position + Vector3.down * 4f + Vector3.right * 1f;
    }

    // Update is called once per frame
    void Update()
    {

        playerArmBottom1Tran.position = new Vector3(
            stanceHand1Tran.position.x,
            stanceHand1Tran.position.y,
            0
        );

        playerArmBottom2Tran.position = new Vector3(
            stanceHand2Tran.position.x,
            stanceHand2Tran.position.y,
            0
        );

        if (!controlsEnabled)
        {
            return;
        }

        MoveTowardsDefaultStance();

        float playerX = transform.position.x;
        float playerY = transform.position.y;
        float playerHeadX = playerHead.transform.position.x;
        float playerHeadY = playerHead.transform.position.y;

        if (Input.GetKey("w"))
        {
            if (playerHeadY < playerY + reach)
            {
                playerHead.transform.position += Vector3.up * speed;
            }
        }
        if (Input.GetKey("s"))
        {
            if (playerHeadY > playerY - reach)
            {
                playerHead.transform.position += Vector3.down * speed;
            }
        }
        if (Input.GetKey("a"))
        {
            /* 
            if(facingRight){
                facingRight = false;
                playerSprite.flipX = true;
            }
            */
            if (playerHeadX > playerX - reach)
            {
                playerHead.transform.position += Vector3.left * speed;
            }
        }
        if (Input.GetKey("d"))
        {
            /*
            if(!facingRight){
                facingRight = true;
                playerSprite.flipX = false;
            }
            */
            if (playerHeadX < playerX + reach)
            {
                playerHead.transform.position += Vector3.right * speed;
            }
        }
        if (playerHead.transform.position.x > playerX + reach)
        {
            playerHead.transform.position -= Vector3.right * speed;
        }
        if (playerHead.transform.position.x < playerX - reach)
        {
            playerHead.transform.position += Vector3.right * speed;
        }
        if (playerHead.transform.position.y < playerX - reach)
        {
            playerHead.transform.position += Vector3.up * speed;
        }
        if (playerHead.transform.position.y > playerX + reach)
        {
            playerHead.transform.position -= Vector3.up * speed;
        }
        if (Input.GetKey("space"))
        {
            if (Time.frameCount > nextAttackAvailableFrame)
            {
                nextAttackAvailableFrame += attackInterval;
                controlsEnabled = false;
                Attack();
            }
        }
        /*
        //move to the center if no direction pressed
        if(!Input.GetKey("w") && !Input.GetKey("s")){
            if(playerHeadY != playerY){
                if(playerHeadY > playerY){
                    playerHead.transform.position += Vector3.down * speed;
                }
                else{
                    playerHead.transform.position += Vector3.up * speed;
                }
            }
        }
        if(!Input.GetKey("a") && !Input.GetKey("d")){
            if(playerHeadX != playerX){
                if(playerHeadX > playerX){
                    playerHead.transform.position += Vector3.left * speed;
                }
                else{
                    playerHead.transform.position += Vector3.right * speed;
                }
            }
        }
        */
    }

    void Attack()
    {

        controlsEnabled = false;

        string[] attacks = {
        "bottom left", "bottom", "bottom right",
        "center left", "true center", "center right",
        "top right", "top", "top right"
        };

        int xSector = 1;
        float playerHeadX = playerHead.transform.position.x;
        float playerX = transform.position.x;
        if (playerHeadX < playerX - reach / 3)
        {
            xSector = 0;
        }
        if (playerHeadX > reach / 3)
        {
            xSector = 2;
        }

        int ySector = 1;
        float playerHeadY = playerHead.transform.position.y;
        float playerY = transform.position.y;
        if (playerHeadY < playerY - reach / 3)
        {
            ySector = 0;
        }
        if (playerHeadY > playerY + reach / 3)
        {
            ySector = 2;
        }

        int sector = ySector * 3 + xSector;
        switch (sector)
        {
            case 0:
                StartCoroutine(Hook());
                break;
            case 1:
                StartCoroutine(Hook());
                break;
            case 2:
                StartCoroutine(Hook());
                break;
            case 3:
                StartCoroutine(Hook());
                break;
            case 4:
                StartCoroutine(Hook());
                break;
            case 5:
                StartCoroutine(Hook());
                break;
            case 6:
                StartCoroutine(Hook());
                break;
            case 7:
                StartCoroutine(Hook());
                break;
            case 8:
                StartCoroutine(Hook());
                break;
        }
        Debug.Log(attacks[sector] + " attack");
    }


    IEnumerator Hook()
    {
        float punchDistance = 4f;
        float timeTaken = 0.25f; //seconds
        int framesTaken = (int)(timeTaken * 60);
        float distancePerFrame = punchDistance / framesTaken;

        GameObject newPlayerAttack = Instantiate(
            playerAttackArea,
            new Vector3(
                stanceHand1Tran.position.x + punchDistance * 1f,
                stanceHand1Tran.position.y + punchDistance * 0.2f,
                0),
            playerHead.transform.rotation
        );

        newPlayerAttack.GetComponent<PlayerAttackAreaScript>().lifespan = timeTaken * 60f;

        for (int i = 0; i < framesTaken; i++)
        {
            playerHead.transform.position = new Vector3(
                playerHead.transform.position.x + distancePerFrame * .5f,
                playerHead.transform.position.y,
                0);
            stanceHand1Tran.position = new Vector3(
                stanceHand1Tran.position.x + distancePerFrame * 1f,
                stanceHand1Tran.position.y + distancePerFrame * 0.2f,
                0);
            yield return null;
        }

        controlsEnabled = true;
    }
}
