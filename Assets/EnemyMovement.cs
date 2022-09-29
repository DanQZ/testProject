using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public GameObject player;
    float speed;
    public int hp;
    // Start is called before the first frame update
    void Start()
    {
        hp = 50; //want to be 50% of starter hp
        speed= 3f/60f;
        //Triggered when EnemySpawner
    }

    // Update is called once per frame
    void Update()
    {
        //change to transform.position
     if(transform.position.x !=player.transform.position.x){
        if(transform.position.x > player.transform.position.x){
             transform.position += Vector3.left *(speed/2);
        }else if(transform.position.x < player.transform.position.x){
            transform.position += Vector3.right *(speed/2);
        }
        // transform.position += Vector3.left *speed;
     }   
    }
}
