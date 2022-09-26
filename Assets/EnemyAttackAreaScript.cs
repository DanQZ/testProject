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
    public GameObject incomingCircle;
    SpriteRenderer incomingCircleSprite;
    public GameObject warning;
    SpriteRenderer warningSprite;
    bool despawnNextFrame;

    void Start()
    {
        despawnNextFrame = false;
        warningSprite = warning.GetComponent<SpriteRenderer>();
        incomingCircleSprite = incomingCircle.GetComponent<SpriteRenderer>();
        //       warningSprite.GetComponent<CircleCollider2D>().enabled = false; 
        // incomingCircleColor = incomingCircle.GetComponent<SpriteRenderer>().color;
        startFrame = Time.frameCount;
        deathFrame = startFrame + lifespan;
        incomingCircle.transform.position = transform.position;
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
            //            warningSprite.GetComponent<CircleCollider2D>().enabled = true;
            despawnNextFrame = true;
        }
        scale -= 2f / lifespan;
        transparency += 0.5f / lifespan;
        incomingCircle.transform.localScale = new Vector3(scale, scale, scale);
        incomingCircleSprite.color = new Color(255f, 0f, 0f, transparency);
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && despawnNextFrame)
        {
            Debug.Log("collision");
            collision.gameObject.GetComponent<PlayerScript>().hp -= 5;
        }
    }
}
