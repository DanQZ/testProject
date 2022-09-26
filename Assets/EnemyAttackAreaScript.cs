using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackAreaScript : MonoBehaviour
{
    int startFrame;
    int deathFrame;
    int lifespan = 60; // 60 frames = 1 second
    float scale = 3f;
    float transparency = 0f;
    public GameObject incomingCircle; // declaring a public GameObject allows you to make a reference to any other GameObject
    SpriteRenderer incomingCircleSprite; // SpriteRenderer is a component, which means it is part of another GameObject
    public GameObject warning;
    SpriteRenderer warningSprite;
    bool despawnNextFrame;

    void Start()
    {
        despawnNextFrame = false;
        
            // using [GameObjectName].GetComponent<[ComponentName]>() lets you access the component's attributes. 
        warningSprite = warning.GetComponent<SpriteRenderer>();
        incomingCircleSprite = incomingCircle.GetComponent<SpriteRenderer>();

        startFrame = Time.frameCount; // sets startFrame to the current frame of the game
        deathFrame = startFrame + lifespan;

            // [GameObjectName].transform.position is the xyz position
        incomingCircle.transform.position = transform.position;

        /* 
            [GameObjectName].transform.[either position or scale] can be changed by adding a new Vector3(x,y,z)
         
            changing rotation requires quaternion math, which is really complicated. 
            
            it is easier to change the rotation of someGameObject by creating a newGameObject at a new Vector3(x,y,z) and then using someGameObject.transform.LookAt(newGameObject.transform.position)
        */
        incomingCircle.transform.localScale = new Vector3(scale, scale, scale);
    }

    // Update is called once per frame
    void Update()
    {
        if (despawnNextFrame)
        {
            Destroy(this.gameObject);
        }
        if (Time.frameCount > deathFrame)
        {
            despawnNextFrame = true;
        }
        scale -= 2f / lifespan;
        transparency += 0.5f / lifespan;
        incomingCircle.transform.localScale = new Vector3(scale, scale, scale);
        incomingCircleSprite.color = new Color(255f, 0f, 0f, transparency);
    }

    /* 
        checks if any other collider overlaps any collider of the GameObject, and if so, runs the code inside.

        at least one GameObject in the collision between 2 GameObjects requires a Rigidbody component for the collision to be detected

        there are other types of collision detection, such as OnTriggerEnter2D which only activates the first frame another collider overlaps. Check the link on discord for a more thorough explanation of collision detection.
    */
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && despawnNextFrame)
        {
            Debug.Log("collision");
            collision.gameObject.GetComponent<PlayerScript>().hp -= 5;
        }
    }
}
