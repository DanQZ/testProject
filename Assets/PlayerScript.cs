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
    public GameObject neckPosition;
    private Transform neckPositionTran;
    public GameObject playerTorsoTop;
    public GameObject torsoBottom;
    public GameObject upperArm1;
    private Transform upperArm1Tran;
    public GameObject lowerArm1;
    private Transform lowerArm1Tran;
    public GameObject upperArm2;
    private Transform upperArm2Tran;
    public GameObject lowerArm2;
    private Transform lowerArm2Tran;
    public GameObject thigh1;
    private Transform thigh1Tran;
    public GameObject calf1;
    private Transform calf1Tran;
    public GameObject thigh2;
    private Transform thigh2Tran;
    public GameObject calf2;
    private Transform calf2Tran;

    public GameObject stanceHand1;
    private Transform stanceHand1Tran;
    public GameObject stanceHand2;
    private Transform stanceHand2Tran;
    public GameObject stanceFoot1;
    private Transform stanceFoot1Tran;
    public GameObject stanceFoot2;
    private Transform stanceFoot2Tran;
    public GameObject stanceElbow1;
    private Transform stanceElbow1Tran;
    public GameObject stanceElbow2;
    private Transform stanceElbow2Tran;

    public GameObject spriteObject;
    SpriteRenderer playerSprite;

    // public List<string> movementQueue = new List<string>();

    public float reach;

    bool controlsEnabled;

    float expectedElbowDistanceToNeck1;
    float expectedElbowDistanceToNeck2;

    void InitTransforms()
    {
        neckPositionTran = neckPosition.transform;
        upperArm1Tran = upperArm1.transform;
        upperArm2Tran = upperArm2.transform;
        lowerArm1Tran = lowerArm1.transform;
        lowerArm2Tran = lowerArm2.transform;
        calf1Tran = calf1.transform;
        calf2Tran = calf2.transform;
        stanceHand1Tran = stanceHand1.transform;
        stanceHand2Tran = stanceHand2.transform;
        stanceFoot1Tran = stanceFoot1.transform;
        stanceFoot2Tran = stanceFoot2.transform;
        stanceElbow1Tran = stanceElbow1.transform;
        stanceElbow2Tran = stanceElbow2.transform;
    }
    // Start is called before the first frame update
    void Start()
    {
        InitTransforms();

        expectedElbowDistanceToNeck1 = Vector3.Distance(stanceElbow1Tran.position, neckPositionTran.position);
        expectedElbowDistanceToNeck2 = Vector3.Distance(stanceElbow2Tran.position, neckPositionTran.position);

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
        float hand1Dist = Vector3.Distance(stanceHand1Tran.position, playerHead.transform.position + Vector3.right * 0.25f + Vector3.down * 1f);
        if (hand1Dist > 0.1f)
        {
            stanceHand1Tran.LookAt(playerHead.transform.position + Vector3.right * 0.25f + Vector3.down * 1f);
            stanceHand1Tran.position += stanceHand1Tran.forward * Mathf.Max(speed * hand1Dist * 2, speed);
        }

        float hand2Dist = Vector3.Distance(stanceHand2Tran.position, playerHead.transform.position + Vector3.right * 1.5f + Vector3.down * 0.5f);
        if (hand2Dist > 0.1f)
        {
            stanceHand2Tran.LookAt(playerHead.transform.position + Vector3.right * 1.5f + Vector3.down * 0.5f);
            stanceHand2Tran.position += stanceHand2Tran.forward * Mathf.Max(speed * hand2Dist * 2, speed);
        }
/*
        // moves elbows to correct distance (currently broken)
        if (Vector3.Distance(stanceElbow1Tran.position, neckPositionTran.position) > expectedElbowDistanceToNeck1)
        {
            stanceElbow1Tran.LookAt(neckPositionTran.position);
            stanceElbow1Tran.position += stanceElbow1Tran.forward * speed;
                   }
        if (Vector3.Distance(stanceElbow2Tran.position, neckPositionTran.position) > expectedElbowDistanceToNeck2)
        {
            stanceElbow2Tran.LookAt(neckPositionTran.position);
            stanceElbow2Tran.position += stanceElbow2Tran.forward * speed;
        }
*/
        GroundedFeet();
    }

    //keeps feet on the ground 
    void GroundedFeet()
    {
        calf1Tran.position = transform.position + Vector3.down * 4f - Vector3.right * 1f;
        calf2Tran.position = transform.position + Vector3.down * 4f + Vector3.right * 1f;
    }

    // Update is called once per frame
    void Update()
    {

        lowerArm1Tran.position = stanceHand1Tran.position;
        lowerArm2Tran.position = stanceHand2Tran.position;
        upperArm1Tran.position = stanceElbow1Tran.position;
        upperArm2Tran.position = stanceElbow2Tran.position;


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
        float timeTaken = .2f; //seconds
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
            stanceHand1Tran.LookAt(newPlayerAttack.transform.position);
            stanceHand1Tran.position = stanceHand1Tran.position + stanceHand1Tran.forward * distancePerFrame;

            stanceHand2Tran.LookAt(playerHead.transform.position + Vector3.right * 1f);
            stanceHand2Tran.position += stanceHand2Tran.forward * distancePerFrame * 0.5f;
            upperArm1Tran.position = stanceElbow1Tran.position;
            GroundedFeet();
            yield return null;
        }

        controlsEnabled = true;
    }
}
