using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAreaScript : MonoBehaviour
{
    int startFrame;
    int deathFrame;
    public int lifespan = 60; // 60 frames = 1 second
    public GameObject particleEffectController;
    public ParticleEffectsController particleControllerScript;
    public GameObject incomingCircle; // declaring a public GameObject allows you to make a reference to any other GameObject
    SpriteRenderer incomingCircleSprite; // SpriteRenderer is a component, which means it is part of another GameObject
    public GameObject warning;
    SpriteRenderer warningSprite;

    public Vector3 strikeForceVector;

    public Vector3 thingHittingPos; // these are used for particle system showing attack hit
    public Vector3 thingHittingStancePos;

    public GameObject thingHit;
    public GameObject thingHitObjectRoot;
    bool despawnNextFrame = false;
    public int attackDamage;
    public GameObject creator;
    public string creatorType;
    PolygonCollider2D myCollider;
    bool collided = false;
    private FighterScript guyHitScript = null;

    public void UpdateSprites()
    {
        switch (creatorType)
        {
            case "player":
                warningSprite.enabled = false;
                incomingCircleSprite.enabled = false;
                break;
            case "enemy":
                warningSprite.enabled = true;
                incomingCircleSprite.enabled = true;
                break;
        }
    }

    void Awake()
    {
        //Debug.Log("attackArea position = " + transform.position.x + "," + transform.position.y);
        creatorType = "NONE";
        warningSprite = warning.GetComponent<SpriteRenderer>();
        incomingCircleSprite = incomingCircle.GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        myCollider = this.gameObject.GetComponent<PolygonCollider2D>();
        myCollider.enabled = false;
        despawnNextFrame = false;

        startFrame = Time.frameCount;
        deathFrame = startFrame + lifespan;

        incomingCircle.transform.position = transform.position;

        if (creatorType != "player")
        {
            StartCoroutine(CircleShrink());
        }

    }

    void Update()
    {
        if (creator == null)
        {
            Destroy(this.gameObject);
        }

        if (despawnNextFrame)
        {
            //Debug.Log("attack from " + creatorType);
            Destroy(this.gameObject, 3f / 60f);
        }
        if (Time.frameCount > deathFrame)
        {
            myCollider.enabled = true;
            despawnNextFrame = true;
        }
    }

    IEnumerator CircleShrink()
    {
        float scale = 0f;
        float transparency = 0f;
        for (int time = 0; time < lifespan; time++)
        {
            scale -= 1f / lifespan;
            transparency += 0.5f / lifespan;
            incomingCircle.transform.localScale = new Vector3(scale, scale, scale);
            incomingCircleSprite.color = new Color(255f, 0f, 0f, transparency);
            yield return null;
        }
    }

    void SetThingHit(GameObject arg)
    {
        thingHit = arg;
        thingHitObjectRoot = thingHit.transform.root.gameObject;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collided)
        {
            return;
        }

        SetThingHit(collision.gameObject);

        bool someoneGotHit = false;

        if (creatorType == "enemy" && collision.gameObject.tag == "Player")// enemy hits player
        {
            someoneGotHit = true;
            warningSprite.color = Color.blue;
            collided = true;
            FighterScript playerFS = thingHitObjectRoot.GetComponent<PlayerScript>().PFScript;
            guyHitScript = playerFS;
        }
        if (creatorType == "player" && collision.gameObject.tag == "Enemy") // player hits enemy
        {
            someoneGotHit = true;
            FighterScript danEnemyFS = collision.gameObject.transform.parent.parent.GetComponent<FighterScript>();
            guyHitScript = danEnemyFS;
            GameObject collisionObject = collision.gameObject;
            collided = true;
        }
        if (!someoneGotHit)
        {
            return;
        }

        guyHitScript.TakeDamage(attackDamage);
        PlayAttackEffect();
        CheckIfThingHitIsDead();
        return;
    }

    private void PlayAttackEffect()
    {
        particleEffectController.transform.position = thingHit.transform.position;

        particleEffectController.transform.up = thingHit.transform.position - thingHittingPos;

        particleControllerScript.PlayEffect("attackHit");
    }

    private void CheckIfThingHitIsDead()
    {
        if (guyHitScript.hp <= 0)
        {
            LaunchAwayThingHit(strikeForceVector);
            guyHitScript.Die();
            // in the case we kill an enemy, delete the ghost 
            if (guyHitScript.headLimb.tag == "Enemy")
            {
                // use objectRoot because we want to destroy the entire enemy gameObject
                thingHitObjectRoot.GetComponent<EnemyWithGhostScript>().StopAllCoroutines();
                thingHitObjectRoot.GetComponent<EnemyWithGhostScript>().enabled = false;
                thingHitObjectRoot.GetComponent<DanEnemyScript>().StopAllCoroutines();
                thingHitObjectRoot.GetComponent<DanEnemyScript>().enabled = false;
                Destroy(thingHitObjectRoot.GetComponent<EnemyWithGhostScript>().ghostFighter);
                Destroy(this.gameObject);
                return;
            }
        }
        Destroy(this.gameObject);
    }


    void LaunchAwayThingHit(Vector3 forceVector)
    {
        guyHitScript.SetRagdoll(true);
        Rigidbody2D rb2d = GetRigidbody2DOfObject(thingHit);
        if (rb2d != null)
        {
            rb2d.AddForce(forceVector, ForceMode2D.Impulse);
        }
        else
        {
            Debug.Log("no rigidbody found in gameobject or parent of gameobject");
        }
    }

    Rigidbody2D GetRigidbody2DOfObject(GameObject gObject)
    {
        Rigidbody2D objectRB = gObject.GetComponent<Rigidbody2D>();
        if (objectRB != null)
        {
            return objectRB;
        }
        else
        {
            return gObject.transform.parent.gameObject.GetComponent<Rigidbody2D>();
        }
    }

    public void DirectlyDamage(FighterScript fighterScript, int damage, Vector3 forceVector)
    {
        fighterScript.TakeDamage(damage);
        guyHitScript = fighterScript;
        strikeForceVector = forceVector;
        SetThingHit(fighterScript.headLimb);
        CheckIfThingHitIsDead();
    }
}
