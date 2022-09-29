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
    public GameObject spriteObject;
    SpriteRenderer playerSprite;
    public List<string> movementQueue = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        playerSprite = spriteObject.gameObject.GetComponent<SpriteRenderer>();
        facingRight = true;
        hp = 100;
        Application.targetFrameRate = 60; // sets frame rate to 60fps, i will likely move this to another script later
        speed = 3f / 60f; // x units per 60 frames
        nextAttackAvailableFrame = Time.frameCount;
        attackInterval = 60; //once per x frames
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("w"))
        {
            if(transform.position.y < 1f){
                transform.position += Vector3.up * speed;
            }
        }
        if (Input.GetKey("s"))
        {
            if(transform.position.y > -1f){
                transform.position += Vector3.down * speed;
            }
        }
        if (Input.GetKey("a"))
        {
            if(facingRight){
                facingRight = false;
                playerSprite.flipX = true;
            }
            if(transform.position.x > -10f){
                transform.position += Vector3.left * speed;
            }
        }
        if (Input.GetKey("d"))
        {
            if(!facingRight){
                facingRight = true;
                playerSprite.flipX = false;
            }
            if(transform.position.x < 10f){
                transform.position += Vector3.right * speed;
            }
        }
        if (Input.GetKey("space"))
        {
            if(Time.frameCount > nextAttackAvailableFrame){
                Debug.Log("attacking");
                nextAttackAvailableFrame += attackInterval;
                Attack();
            }
        }
        if(!Input.GetKey("w") && !Input.GetKey("s")){
            if(transform.position.y != 0f){
                if(transform.position.y > 0f){
                    transform.position += Vector3.down * speed;
                }
                else{
                    transform.position += Vector3.up * speed;
                }
            }
        }
    }

    void Attack(){
        for(int i = 0; i < 60; i++){
            movementQueue.Add("");
        }
    }
}
