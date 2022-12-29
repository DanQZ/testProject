using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAreaScript : MonoBehaviour {
    public GameStateManagerScript gameStateManagerScript;
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

    public GameObject thingHit;
    public GameObject thingHitObjectRoot;
    bool despawnNextFrame = false;
    public float attackDamage;
    public bool isCrit = false;
    public string damageType; // defaults to "physical"
    public GameObject creator;
    public string creatorType;
    PolygonCollider2D myCollider;
    bool collided = false;
    private FighterScript guyHitScript = null; // created upon collision 
    public FighterScript guyHittingScript = null; // set when created by fighterscript

    public void UpdateSprites() {
        switch (creatorType) {
            case "player":
                //warningSprite.enabled = false;
                // incomingCircleSprite.enabled = false;
                break;
            case "enemy":
                warningSprite.enabled = true;
                incomingCircleSprite.enabled = true;
                break;
        }
    }

    void Awake() {
        //Debug.Log("attackArea position = " + transform.position.x + "," + transform.position.y);
        creatorType = "NONE";
        damageType = "physical";
        warningSprite = warning.GetComponent<SpriteRenderer>();
        incomingCircleSprite = incomingCircle.GetComponent<SpriteRenderer>();

        warningSprite.color = new Color(255f, 0f, 0f, .25f);
    }

    void Start() {
        myCollider = this.gameObject.GetComponent<PolygonCollider2D>();
        myCollider.enabled = false;
        despawnNextFrame = false;

        startFrame = Time.frameCount;
        deathFrame = startFrame + lifespan;

        incomingCircle.transform.position = transform.position;

        if (creatorType != "player") {
            StartCoroutine(CircleGrow());
        }

    }

    void Update() {
        if (creator == null) {
            Destroy(this.gameObject);
        }

        if (despawnNextFrame) {
            //Debug.Log("attack from " + creatorType);
            Destroy(this.gameObject, 3f / 60f);
        }
        if (Time.frameCount > deathFrame) {
            myCollider.enabled = true;
            despawnNextFrame = true;
        }
    }

    IEnumerator CircleGrow() {
        float scale = 0f;
        float transparency = 0f;
        for (int time = 0; time < lifespan; time++) {
            scale -= 1f / lifespan;
            transparency = (float)(time) / lifespan * 255f;
            incomingCircle.transform.localScale = new Vector3(scale, scale, scale);
            incomingCircleSprite.color = new Color(255f, 0f, 0f, transparency);
            yield return null;
        }
    }

    void SetThingHit(GameObject arg) {
        thingHit = arg;
        thingHitObjectRoot = thingHit.transform.root.gameObject;
    }

    void OnTriggerStay2D(Collider2D collision) {
        if (collided) {
            return;
        }

        SetThingHit(collision.gameObject);

        bool someoneGotHit = false;

        if (creatorType == "enemy" && thingHit.tag == "Player")// enemy hits player
        {
            someoneGotHit = true;
            collided = true;
            FighterScript playerFS = thingHitObjectRoot.GetComponent<PlayerScript>().PFScript;
            guyHitScript = playerFS;
            PlayerIsHitExtraEffects();
        }
        if (creatorType == "player" && thingHit.tag == "Enemy") // player hits enemy
        {
            someoneGotHit = true;
            collided = true;
            FighterScript enemyFS = thingHit.transform.parent.parent.GetComponent<FighterScript>();
            guyHitScript = enemyFS;
            EnemyIsHitExtraEffects(false);
        }
        if (!someoneGotHit) {
            return;
        }

        if (!guyHitScript.notInAnimation) {
            isCrit = true;
            attackDamage *= 1.5f;
        }

        guyHitScript.TakeDamage(attackDamage, isCrit, damageType);
        PlayAttackEffect();
        CheckIfThingHitIsDead(guyHitScript);
        return;
    }

    private void PlayAttackEffect() {
        particleEffectController.transform.position = thingHit.transform.position;

        particleEffectController.transform.up = thingHit.transform.position - thingHittingPos;

        particleControllerScript.PlayEffect("attackHit");
    }

    private void CheckIfThingHitIsDead(FighterScript checkedFighterScript) {
        checkedFighterScript = guyHitScript;
        if (checkedFighterScript.hp <= 0) {
            // in the case we kill an enemy, delete the ghost 
            if (checkedFighterScript.headLimb.tag == "Enemy") {
                EnemyDeathProtocol();
            }
            else {
                PlayerDeathProtocol();
            }
            LaunchAwayThingHitBcItDied();
            checkedFighterScript.Die();
        }
        else {
            PushAwayGuy(guyHitScript);
        }
        Destroy(this.gameObject);
    }
    void EnemyIsHitExtraEffects(bool directOrNot) // direct does not cause explosion
    {
        //vampirism perk
        int vampirismLevel = guyHittingScript.vampirismLevel;
        if (vampirismLevel > 0) {
            // 10% + 5% vampirism lvl
            guyHittingScript.TakeHealing(attackDamage * (0.1f + vampirismLevel * 0.05f), "vampirism");
        }

        // poisoner perk
        int poisonerLevel = guyHittingScript.poisonerLevel;
        if (poisonerLevel > 0) {
            guyHitScript.isPoisonedEffect = Mathf.Max(poisonerLevel, guyHitScript.isPoisonedEffect); // weaker poison does not override more powerful
        }

        int explosiveLevel = guyHittingScript.explosiveLevel;
        if (!directOrNot && explosiveLevel > 0) {
            float range = 1f + ((float)explosiveLevel * 0.2f);
            List<GameObject> allEnemiesList = gameStateManagerScript.enemyManagerScript.allEnemiesList;
            for (int i = allEnemiesList.Count - 1; i >= 0; i--) {
                GameObject enemyFighter = allEnemiesList[i].GetComponent<EnemyWithGhostScript>().enemyFighter;

                float distanceToHead = Vector3.Distance(
                    enemyFighter.GetComponent<FighterScript>().headLimb.transform.position,
                    transform.position
                    );

                float distanceToPelvis = Vector3.Distance(
                    enemyFighter.GetComponent<FighterScript>().jointPelvis2.transform.position,
                    transform.position
                    );

                float distance = Mathf.Min(distanceToHead, distanceToPelvis);

                if (enemyFighter != guyHitScript.gameObject && distance < range) {
                    float explosiveDamage = attackDamage * 0.1f * (float)explosiveLevel;

                    FighterScript hitThisGuy = enemyFighter.GetComponent<FighterScript>();

                    Vector3 explosiveStrikeForceVector = Vector3.Normalize(hitThisGuy.gameObject.transform.position - guyHittingScript.gameObject.transform.position) * explosiveDamage * 0.1f;

                    DirectlyDamage(hitThisGuy, explosiveDamage, "explosive", explosiveStrikeForceVector);
                }
            }
        }
    }
    void EnemyDeathProtocol() {
        // use objectRoot because we want to destroy the entire enemy gameObject
        thingHitObjectRoot.GetComponent<EnemyWithGhostScript>().myManagerScript.allEnemiesList.Remove(thingHitObjectRoot);
        thingHitObjectRoot.GetComponent<EnemyWithGhostScript>().StopAllCoroutines();
        thingHitObjectRoot.GetComponent<EnemyWithGhostScript>().enabled = false;
        thingHitObjectRoot.GetComponent<DanEnemyAI>().StopAllCoroutines();
        thingHitObjectRoot.GetComponent<DanEnemyAI>().enabled = false;
        Destroy(thingHitObjectRoot.GetComponent<EnemyWithGhostScript>().ghostFighter);
        Destroy(this.gameObject);
        return;
    }

    void PlayerIsHitExtraEffects() {
        if (guyHitScript.colossusLevel > 0 && !guyHitScript.notInAnimation && !guyHitScript.isTurning && !guyHitScript.isAirborne) // in an animation, but turning around does not count
        {
            float divideBy = 1f + guyHitScript.colossusLevel;
            float origAttackDmg = attackDamage;
            strikeForceVector /= divideBy;
            attackDamage /= divideBy;
            float damagedReduced = origAttackDmg - attackDamage;
            gameStateManagerScript.colossusDamageReduced += damagedReduced * 1.5f; // multiply by 1.5 bc you will be crit if this triggers
        }
    }
    void PlayerDeathProtocol() {

    }

    void LaunchAwayThingHitBcItDied() {
        guyHitScript.SetRagdoll(true);
        Rigidbody2D rb2d = GetRigidbody2DOfObject(thingHit);
        if (rb2d != null) {
            rb2d.AddForce(strikeForceVector / 2f, ForceMode2D.Impulse);
        }
        else {
            Debug.Log("no rigidbody found in gameobject or parent of gameobject");
        }
    }

    void PushAwayGuy(FighterScript pushedFighterScript) {
        Rigidbody2D rb2dTarget = null;
        GameObject enemyWithGhostParent = null;

        if (thingHit.tag == "Enemy") {
            // push both the enemyGhost and enemy
            enemyWithGhostParent = pushedFighterScript.gameObject.transform.parent.gameObject;
            rb2dTarget = enemyWithGhostParent.GetComponent<Rigidbody2D>();
        }
        if (thingHit.tag == "Player") {
            GameObject playerObject = pushedFighterScript.gameObject.transform.parent.gameObject;
            rb2dTarget = playerObject.GetComponent<Rigidbody2D>();
        }

        Vector3 xForce = new Vector3(strikeForceVector.x, 0f, 0f);
        pushedFighterScript.PushParent(xForce);

        if (thingHit.tag == "Enemy") {
            FighterScript ghostFighterScript = enemyWithGhostParent.GetComponent<EnemyWithGhostScript>().ghostFighterScript;
            ghostFighterScript.StartCoroutine(ghostFighterScript.ParentWasPushed());
        }
    }

    Rigidbody2D GetRigidbody2DOfObject(GameObject gObject) {
        Rigidbody2D objectRB = gObject.GetComponent<Rigidbody2D>();
        if (objectRB != null) {
            // limbs have their own objectRBs
            return objectRB;
        }
        else {
            // because the head is slightly different because Im a dumbass 
            return gObject.transform.parent.gameObject.GetComponent<Rigidbody2D>();
        }
    }

    public void DirectlyDamage(FighterScript targetFighterScript, float damage, string damageType, Vector3 forceVector) {
        targetFighterScript.TakeDamage(damage, false, damageType);
        guyHitScript = targetFighterScript;
        strikeForceVector = forceVector;
        SetThingHit(targetFighterScript.headLimb);
        EnemyIsHitExtraEffects(true);
        CheckIfThingHitIsDead(targetFighterScript);
    }
}
