using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackAreaScript : MonoBehaviour
{
    int startFrame;
    int deathFrame;
    public float lifespan = 60f; // 60 frames = 1 second
    public GameObject incomingCircle; // declaring a public GameObject allows you to make a reference to any other GameObject
    SpriteRenderer incomingCircleSprite; // SpriteRenderer is a component, which means it is part of another GameObject
    public GameObject warning;
    SpriteRenderer warningSprite;
    bool despawnNextFrame;

    CircleCollider2D myCollider;

    void Start()
    {
        myCollider = this.gameObject.GetComponent<CircleCollider2D>();
        myCollider.enabled = false;
        despawnNextFrame = false;
        
            // using [GameObjectName].GetComponent<[ComponentName]>() lets you access the component's attributes. 
        warningSprite = warning.GetComponent<SpriteRenderer>();
        incomingCircleSprite = incomingCircle.GetComponent<SpriteRenderer>();

        startFrame = Time.frameCount; // sets startFrame to the current frame of the game
        deathFrame = startFrame + (int)lifespan;

            // [GameObjectName].transform.position is the xyz position
        incomingCircle.transform.position = transform.position;

        /* 
            [GameObjectName].transform.[either position or scale] can be changed by adding a new Vector3(x,y,z)
         
            changing rotation requires quaternion math, which is really complicated. 
            
            it is easier to change the rotation of someGameObject by creating a newGameObject at a new Vector3(x,y,z) and then using someGameObject.transform.LookAt(newGameObject.transform.position)
        */
        StartCoroutine(CircleShrink());
    }

    // Update is called once per frame
    void Update()
    {
        if (despawnNextFrame)
        {
            Destroy(this.gameObject, 2f/60f);
        }
        if (Time.frameCount > deathFrame)
        {
            myCollider.enabled = true;
            despawnNextFrame = true;
        }
    }

    IEnumerator CircleShrink(){
        float scale  = 0f;
        float transparency = 0f;
        for(int time = 0; time < lifespan; time++){
            scale -= 1f / lifespan;
            transparency += 0.5f / lifespan;
            incomingCircle.transform.localScale = new Vector3(scale, scale, scale);
            incomingCircleSprite.color = new Color(0f, 255f, 0f, transparency);
            yield return null;
        }
    }

    /* 
        checks if any other collider overlaps any collider of the GameObject, and if so, runs the code inside.

        at least one GameObject in the collision between 2 GameObjects requires a Rigidbody component for the collision to be detected

        there are other types of collision detection, such as OnTriggerEnter2D which only activates the first frame another collider overlaps. Check the link on discord for a more thorough explanation of collision detection.
    */
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.transform.parent.GetComponent<PlayerScript>().hp -= 5;
            Destroy(this.gameObject);
        }
    }
}
