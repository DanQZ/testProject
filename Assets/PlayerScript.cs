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
    public GameObject playerHead;
    public GameObject spriteObject;
    SpriteRenderer playerSprite;

    // public List<string> movementQueue = new List<string>();

    public float reach;

    bool controlsEnabled;

    // Start is called before the first frame update
    void Start()
    {
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

    // Update is called once per frame
    void Update()
    {
        if(!controlsEnabled){
            return;
        }
        float playerX = transform.position.x;
        float playerY = transform.position.y;
        float playerHeadX = playerHead.transform.position.x;
        float playerHeadY = playerHead.transform.position.y;

        if (Input.GetKey("w"))
        {
            if(playerHeadY < playerY + reach){
                playerHead.transform.position += Vector3.up * speed;
            }
        }
        if (Input.GetKey("s"))
        {
            if(playerHeadY > playerY - reach){
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
            if(playerHeadX > playerX - reach){
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
            if(playerHeadX < playerX + reach){
                playerHead.transform.position += Vector3.right * speed;
            }
        }
        if (Input.GetKey("space"))
        {
            if(Time.frameCount > nextAttackAvailableFrame){
                nextAttackAvailableFrame += attackInterval;
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

    void Attack(){

        controlsEnabled = false;

        string[] attacks = {
        "bottom left", "bottom", "bottom right", 
        "center left", "true center", "center right", 
        "bottom right", "top", "top right"
        };

        int xSector = 1;
        float playerHeadX = playerHead.transform.position.x;
        float playerX = transform.position.x; 
        if(playerHeadX < playerX - reach/3){
            xSector = 0;
        }
        if(playerHeadX > reach/3){
            xSector = 2;
        }

        int ySector = 1;
        float playerHeadY = playerHead.transform.position.y; 
        float playerY = transform.position.y; 
        if(playerHeadY < playerY - reach/3){
            ySector = 0;
        }
        if(playerHeadY > playerY + reach/3){
            ySector = 2;
        }

        int sector = ySector * 3 + xSector;

        Debug.Log(attacks[sector]);
    }

    void FrontAttack(){

    }
}
