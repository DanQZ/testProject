using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    float speed;
    public int hp;
    // Start is called before the first frame update
    void Start()
    {
        hp = 100;
        Application.targetFrameRate = 60; // sets frame rate to 60fps, i will likely move this to another script later
        speed = 3f / 60f; // x units per 60 frames
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
            if(transform.position.x > -1f){
                transform.position += Vector3.left * speed;
            }
        }
        if (Input.GetKey("d"))
        {
            if(transform.position.x < 1f){
                transform.position += Vector3.right * speed;
            }
        }
        if(!Input.anyKey){
            if(transform.position.x != 0f){
                if(transform.position.x > 0f){
                    transform.position += Vector3.left * speed/2;
                }
                else{
                    transform.position += Vector3.right * speed/2;
                }
            }
            if(transform.position.y != 0f){
                if(transform.position.y > 0f){
                    transform.position += Vector3.down * speed/2;
                }
                else{
                    transform.position += Vector3.up * speed/2;
                }
            }
        }
    }
}
