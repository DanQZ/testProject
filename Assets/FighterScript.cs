using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FighterScript : MonoBehaviour {
    private float FPS = 60f;
    public GameObject damageNumberPrefab;
    // perks
    public int colossusLevel = 0;
    public int lightningLevel = 0;
    public int vampirismLevel = 0;
    public int explosiveLevel = 0;
    public int poisonerLevel = 0; // creates isPoisonedEffect
    public int isPoisonedEffect = 0;
    public float poisonDefaultDamage = 10f;
    public float poisonDamagePerExtraLevel = 5f;
    public int pressurePointLevel = 0; // creates isWeakenedEffect
    public int isWeakenedEffect = 0;
    public int attackDelayIfEnemy = 60; // frames 

    // selection of moves
    public string[] armMoveset = new string[9];
    public string[] legMoveset = new string[9];
    public string specialAttack;

    public GameObject parentObject;
    public Rigidbody2D parentRB;
    public GameObject particleEffectController;
    ParticleEffectsController particleControllerScript;
    public GameObject gameStateManager;
    public GameStateManagerScript gameStateManagerScript;

    public GameObject healthBar;
    public GameObject healthBarBackground;
    private Vector3 healthBarHeight;
    private float healthBarScaleX;
    private float healthBarScaleY;

    public GameObject energyBar;
    public GameObject energyBarBackground;
    private Vector3 energyBarHeight;
    private float energyBarScaleX;
    private float energyBarScaleY;

    public bool isPlayer = false;
    public bool isGhost = false;
    public GameObject AttackWarningPrefab;
    private TrailRenderer hand1Trail;
    private TrailRenderer hand2Trail;
    private TrailRenderer elbow1Trail;
    private TrailRenderer elbow2Trail;
    private TrailRenderer knee1Trail;
    private TrailRenderer knee2Trail;
    private TrailRenderer foot1Trail;
    private TrailRenderer foot2Trail;

    public bool isInvulnerable = false;
    public bool isAirborne = false;
    public Rigidbody2D fighterRB;
    public float groundLevel;
    public GameObject fighterOrienter;
    public Transform orientedTran; // .right is forward no matter what direction the fighter is currently looking in

    public string fighterType;
    public string myFightingStyle;

    // health values
    private float defaultMaxHP;
    public float maxhp;
    public float hp;

    // energy values
    private float defaultMaxEnergy;
    public float maxEnergy;
    public float currentEnergy;
    int nextEnergyRegainFrame;
    private float defaultEnergyPerSecond;
    public float energyPerSecond;
    public bool gainEnergyOn;

    // damage values
    private float armPowerDefault;
    public float armPower;
    private float legPowerDefault;
    public float legPower;

    // speed values
    private float speedMultiplierDefault;
    public float speedMultiplier;
    private float defaultMoveSpeed;
    public float moveSpeed;


    public bool facingRight = true;
    public GameObject fighterAttackArea;

    // limb gameObjects are the main fighter body gameObjects
    public GameObject fighterHead;
    public GameObject torsoTop;
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

    // joint gameObjects are used purely for orienting things like LineRenderers and finding distance between limb gameObjects, they should NEVER be manually moved in the script
    public GameObject jointNeck;
    private Transform jointNeckTran;
    public GameObject jointShoulder1;
    private Transform jointShoulder1Tran;
    public GameObject jointShoulder2;
    private Transform jointShoulder2Tran;

    public GameObject jointRib1;
    private Transform jointRib1Tran;
    public GameObject jointRib2;
    private Transform jointRib2Tran;

    public bool drawNormalElbow1 = true;
    public GameObject jointElbow1;
    public Transform customElbow1Tran;
    private Transform jointElbow1Tran;
    public bool drawNormalElbow2 = true;
    public GameObject jointElbow2;
    public Transform customElbow2Tran;
    private Transform jointElbow2Tran;

    public GameObject jointPelvis1;
    private Transform jointPelvis1Tran;
    public GameObject jointPelvis2;
    private Transform jointPelvis2Tran;

    public bool drawNormalKnee1 = true;
    public GameObject jointKnee1;
    public Transform customKnee1Tran;
    private Transform jointKnee1Tran;
    public bool drawNormalKnee2 = true;
    public GameObject jointKnee2;
    public Transform customKnee2Tran;
    private Transform jointKnee2Tran;

    // stance gameObjects force the relevant limb to the stanceObject's position. they are like the opposite of joint gameObjects
    // can activate and deactivate if you want to the limb to move freely for a bit
    public GameObject stanceHead;
    public Transform stanceHeadTran;
    bool stanceHeadActive = true;

    public GameObject stanceHand1;
    public Transform stanceHand1Tran;
    bool stanceHand1Active = true;
    public GameObject stanceHand2;
    public Transform stanceHand2Tran;
    bool stanceHand2Active = true;

    public GameObject stanceFoot1;
    public Transform stanceFoot1Tran;
    bool stanceFoot1Active = true;
    public GameObject stanceFoot2;
    public Transform stanceFoot2Tran;
    bool stanceFoot2Active = true;
    public GameObject stanceTorsoTop;
    public Transform stanceTorsoTopTran;
    bool stanceTorsoTopActive = false;

    public GameObject stanceTorsoBot;
    public Transform stanceTorsoBotTran;
    bool stanceTorsoBotActive = false;

    public GameObject headLimb;
    public SpriteRenderer headSprite;

    // public List<string> movementQueue = new List<string>();

    public float reach; // distance the fighterHead can move from fighter object position

    public bool notInAnimation = true; // enabled, fighter can move head around + limbs move to default positions. disable when in an animation. moves slower when false
    public bool isTurning = false;
    public bool ragdolling = true; // if true, fighter cannot do ANYTHING outside of rigidbody physics 

    float expectedElbowDistanceToNeck1;
    float expectedElbowDistanceToNeck2;

    private LineRenderer arm1Renderer;
    private LineRenderer arm2Renderer;
    private LineRenderer leg1Renderer;
    private LineRenderer leg2Renderer;
    private LineRenderer torsoOutlineRenderer;
    private LineRenderer torsoRenderer;

    private Vector3[] arm1PointArray = new Vector3[3];
    private Vector3[] arm2PointArray = new Vector3[3];
    private Vector3[] leg1PointArray = new Vector3[3];
    private Vector3[] leg2PointArray = new Vector3[3];
    private Vector3[] torsoPointArray = new Vector3[7];

    //default stances relative to the head
    private Vector3 hand1DefaultPos;
    private Vector3 hand2DefaultPos;

    //default stances relative to fighter position
    private Vector3 foot1DefaultPos;
    private Vector3 foot2DefaultPos;

    private HingeJoint2D torsoTopHinge;
    private HingeJoint2D torsoBottomHinge;
    private HingeJoint2D upperArm1Hinge;
    private HingeJoint2D upperArm2Hinge;
    private HingeJoint2D lowerArm1Hinge;
    private HingeJoint2D lowerArm2Hinge;
    private HingeJoint2D thigh1Hinge;
    private HingeJoint2D calf1Hinge;
    private HingeJoint2D thigh2Hinge;
    private HingeJoint2D calf2Hinge;
    private List<HingeJoint2D> allHinges = new List<HingeJoint2D>();
    private TrailRenderer[] allTrails = new TrailRenderer[8];
    private List<GameObject> allBodyParts = new List<GameObject>();
    private List<LineRenderer> allLineRenderers = new List<LineRenderer>();
    private List<Rigidbody2D> allRigidbody2D = new List<Rigidbody2D>();
    private List<PolygonCollider2D> allPolyCollider2D = new List<PolygonCollider2D>();
    private List<GameObject> allStances = new List<GameObject>();
    //private LineRenderer[] allLineRenderers = new LineRenderer[6];
    private List<GameObject> allExistingAttacks = new List<GameObject>();
    public IEnumerator moveCoroutine;
    public IEnumerator walkAnimationCoroutine;
    public IEnumerator InvulnerabilityCoroutine;
    public void ChangeMultiplier(string whichMultiplier, string operationArg, float amount) // so long bc I cant create a reference to a float
    {
        //Debug.Log("changing a multiplier");
        float oldMultiplier = -0f;
        float newMultiplier = -0f;

        bool targetMultiplierFound = false;
        switch (whichMultiplier) {
            case "speed":
                oldMultiplier = speedMultiplier;
                targetMultiplierFound = true;
                break;
            case "leg power":
                oldMultiplier = legPower;
                targetMultiplierFound = true;
                break;
            case "arm power":
                oldMultiplier = armPower;
                targetMultiplierFound = true;
                break;
        }

        if (!targetMultiplierFound) {
            Debug.Log("INVALID MULTIPLIER NAME, OPERATION CANCELLED"); return;
        }

        bool operationSuccess = false;
        switch (operationArg) {
            case "add":
                newMultiplier = oldMultiplier + amount;
                operationSuccess = true;
                break;
            case "subtract":
                newMultiplier = oldMultiplier - amount;
                operationSuccess = true;
                break;
            case "mulitply":
                newMultiplier = oldMultiplier * amount;
                operationSuccess = true;
                break;
            case "divide":
                newMultiplier = oldMultiplier / amount;
                operationSuccess = true;
                break;
            case "set":
                newMultiplier = amount;
                operationSuccess = true;
                break;
        }

        if (!operationSuccess) {
            Debug.Log("INVALID OPERATION NAME, OPERATION CANCELLED");
            return;
        }


        switch (whichMultiplier) {
            case "speed":
                speedMultiplier = newMultiplier;
                Debug.Log("speedmult = " + speedMultiplier);
                moveSpeed = defaultMoveSpeed * speedMultiplier;
                break;
            case "leg power":
                legPower = newMultiplier;
                Debug.Log("leg power is now " + legPower);
                break;
            case "arm power":
                armPower = newMultiplier;
                Debug.Log("arm power is now " + armPower);
                break;
        }
    }
    public void InitDefaultMoveset() { // happens when player is created by the game manager
        myFightingStyle = "unskilled";
        string[][] newMoveset = new string[2][]{
            new string[9],
            new string[9]
        };
        ApplyNewMoveset(newMoveset[0], newMoveset[1]);
    }
    public void SetTags(string tag) // sets the tags for the collisions
    {
        fighterHead.tag = tag;
        headLimb.tag = tag;
        torsoTop.tag = tag;
        torsoBottom.tag = tag;
        upperArm1.tag = tag;
        upperArm2.tag = tag;
        lowerArm1.tag = tag;
        lowerArm2.tag = tag;
        thigh1.tag = tag;
        thigh2.tag = tag;
        calf1.tag = tag;
        calf2.tag = tag;
    }
    void InitHealthAndEnergyBar() {
        healthBarHeight = new Vector3(0f, 0.5f, 0f);
        healthBarScaleX = 2f;
        healthBarScaleY = .25f;

        maxEnergy = 100f;
        currentEnergy = 100f;
        energyPerSecond = 15f;
        energyBarHeight = new Vector3(0f, 0.4f, 0f);
        energyBarScaleX = 2f;
        energyBarScaleY = .1f;

        Vector3 headLimbPos = headLimb.transform.position;

        healthBar.transform.position = headLimbPos + healthBarHeight;
        healthBarBackground.transform.position = healthBar.transform.position;

        energyBar.transform.position = headLimbPos + energyBarHeight;
        energyBarBackground.transform.position = energyBar.transform.position;

        if (isGhost) {
            Destroy(healthBar);
            Destroy(healthBarBackground);
            Destroy(energyBar);
            Destroy(energyBarBackground);
        }
    }
    void InitGameObjects() {
        allBodyParts.Add(fighterHead);

        allBodyParts.Add(torsoTop);
        allBodyParts.Add(torsoBottom);

        allBodyParts.Add(upperArm1);
        allBodyParts.Add(lowerArm1);
        allBodyParts.Add(upperArm2);
        allBodyParts.Add(lowerArm2);

        allBodyParts.Add(thigh1);
        allBodyParts.Add(calf1);
        allBodyParts.Add(thigh2);
        allBodyParts.Add(calf2);

        allStances.Add(stanceHead);
        allStances.Add(stanceTorsoTop);
        allStances.Add(stanceTorsoBot);
        allStances.Add(stanceHand1);
        allStances.Add(stanceHand2);
        allStances.Add(stanceFoot1);
        allStances.Add(stanceFoot2);
    }
    void InitTrailRenderers() {/*
        hand1Trail = stanceHand1.GetComponent<TrailRenderer>();
        hand2Trail = stanceHand2.GetComponent<TrailRenderer>();
        elbow1Trail = jointElbow1.GetComponent<TrailRenderer>();
        elbow2Trail = jointElbow2.GetComponent<TrailRenderer>();
        knee1Trail = jointKnee1.GetComponent<TrailRenderer>();
        knee2Trail = jointKnee2.GetComponent<TrailRenderer>();
        foot1Trail = stanceFoot1.GetComponent<TrailRenderer>();
        foot2Trail = stanceFoot2.GetComponent<TrailRenderer>();

        allTrails[0] = hand1Trail;
        allTrails[1] = hand2Trail;
        allTrails[2] = elbow1Trail;
        allTrails[3] = elbow2Trail;
        allTrails[4] = knee1Trail;
        allTrails[5] = knee2Trail;
        allTrails[6] = foot1Trail;
        allTrails[7] = foot2Trail;

        // color gradient of TrailRenderers
        float alpha = 1.0f;
        Gradient trailGradient = new Gradient();
        trailGradient.SetKeys(
           new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.red, 1.0f) },
           new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
       );

        // width of TrailRenderers
        AnimationCurve widthSizeCurve = new AnimationCurve();
        widthSizeCurve.AddKey(0.0f, 0.0f);
        widthSizeCurve.AddKey(.5f, .5f);

        foreach (var trail in allTrails)
        {
            trail.material = new Material(Shader.Find("Sprites/Default"));
            trail.time = 1f;
            trail.emitting = true;
            trail.colorGradient = trailGradient;
            trail.widthCurve = widthSizeCurve;
            trail.enabled = false;
        }*/
    }
    void HideJointsAndStances() {
        stanceHead.GetComponent<SpriteRenderer>().enabled = false;
        stanceTorsoTop.GetComponent<SpriteRenderer>().enabled = false;
        stanceTorsoBot.GetComponent<SpriteRenderer>().enabled = false;
        stanceHand1.GetComponent<SpriteRenderer>().enabled = false;
        stanceHand2.GetComponent<SpriteRenderer>().enabled = false;
        stanceFoot1.GetComponent<SpriteRenderer>().enabled = false;
        stanceFoot2.GetComponent<SpriteRenderer>().enabled = false;

        jointNeck.GetComponent<SpriteRenderer>().enabled = false;
        jointShoulder1.GetComponent<SpriteRenderer>().enabled = false;
        jointShoulder2.GetComponent<SpriteRenderer>().enabled = false;
        jointRib1.GetComponent<SpriteRenderer>().enabled = false;
        jointRib2.GetComponent<SpriteRenderer>().enabled = false;
        jointPelvis1.GetComponent<SpriteRenderer>().enabled = false;
        jointPelvis2.GetComponent<SpriteRenderer>().enabled = false;

        jointElbow1.GetComponent<SpriteRenderer>().enabled = false;
        jointElbow2.GetComponent<SpriteRenderer>().enabled = false;
        jointKnee1.GetComponent<SpriteRenderer>().enabled = false;
        jointKnee2.GetComponent<SpriteRenderer>().enabled = false;
    }
    void InitHinges() {
        torsoTopHinge = torsoTop.GetComponent<HingeJoint2D>();
        torsoBottomHinge = torsoBottom.GetComponent<HingeJoint2D>();
        upperArm1Hinge = upperArm1.GetComponent<HingeJoint2D>();
        upperArm2Hinge = upperArm2.GetComponent<HingeJoint2D>();
        lowerArm1Hinge = lowerArm1.GetComponent<HingeJoint2D>();
        lowerArm2Hinge = lowerArm2.GetComponent<HingeJoint2D>();
        thigh1Hinge = thigh1.GetComponent<HingeJoint2D>();
        calf1Hinge = calf1.GetComponent<HingeJoint2D>();
        thigh2Hinge = thigh2.GetComponent<HingeJoint2D>();
        calf2Hinge = calf2.GetComponent<HingeJoint2D>();
        allHinges.Add(torsoTopHinge);
        allHinges.Add(torsoBottomHinge);
        allHinges.Add(upperArm1Hinge);
        allHinges.Add(upperArm2Hinge);

        allHinges.Add(thigh1Hinge);
        allHinges.Add(thigh2Hinge);
        allHinges.Add(calf1Hinge);
        allHinges.Add(calf2Hinge);

        // I DONT KNOW WHY BUT FOR SOME REASON FLIPPING THE ANGLES ON THE LOWER ARM HINGES FLIPS THEM THE WRONG WAY WHEN YOU TURN AROUND?????????
        //allHinges[8] = (lowerArm1Hinge);
        //allHinges.Add(lowerArm2Hinge);

        foreach (HingeJoint2D hingeJoint in allHinges) {
            hingeJoint.useMotor = false;
        }
    }
    void InitTransforms() {
        orientedTran = fighterOrienter.transform;
        upperArm1Tran = upperArm1.transform;
        upperArm2Tran = upperArm2.transform;
        lowerArm1Tran = lowerArm1.transform;
        lowerArm2Tran = lowerArm2.transform;

        thigh1Tran = thigh1.transform;
        thigh2Tran = thigh2.transform;
        calf1Tran = calf1.transform;
        calf2Tran = calf2.transform;

        stanceHeadTran = stanceHead.transform;
        stanceTorsoTopTran = stanceTorsoTop.transform;
        stanceTorsoBotTran = stanceTorsoBot.transform;

        stanceHand1Tran = stanceHand1.transform;
        stanceHand2Tran = stanceHand2.transform;
        stanceFoot1Tran = stanceFoot1.transform;
        stanceFoot2Tran = stanceFoot2.transform;

        jointNeckTran = jointNeck.transform;
        jointShoulder1Tran = jointShoulder1.transform;
        jointShoulder2Tran = jointShoulder2.transform;
        jointRib1Tran = jointRib1.transform;
        jointRib2Tran = jointRib2.transform;

        jointElbow1Tran = jointElbow1.transform;
        jointElbow2Tran = jointElbow2.transform;
        GameObject GOcustomElbow1 = new GameObject();
        GameObject GOcustomElbow2 = new GameObject();
        GOcustomElbow1.transform.parent = transform;
        GOcustomElbow2.transform.parent = transform;
        customElbow1Tran = GOcustomElbow1.transform;
        customElbow2Tran = GOcustomElbow2.transform;

        jointPelvis1Tran = jointPelvis1.transform;
        jointPelvis2Tran = jointPelvis2.transform;

        jointKnee1Tran = jointKnee1.transform;
        jointKnee2Tran = jointKnee2.transform;
        GameObject GOcustomKnee1 = new GameObject();
        GameObject GOcustomKnee2 = new GameObject();
        GOcustomKnee1.transform.parent = transform;
        GOcustomKnee2.transform.parent = transform;
        customKnee1Tran = GOcustomKnee1.transform;
        customKnee2Tran = GOcustomKnee2.transform;

    }
    void InitLineRenderers() {
        float thickness = .25f;
        Color bodyColor = Color.black;
        int defaultPositionCount = 3;
        int jointVertices = 2;

        torsoRenderer = torsoTop.GetComponent<LineRenderer>();
        torsoOutlineRenderer = jointNeck.GetComponent<LineRenderer>();
        arm1Renderer = jointShoulder1.GetComponent<LineRenderer>();
        arm2Renderer = jointShoulder2.GetComponent<LineRenderer>();
        leg1Renderer = jointPelvis1.GetComponent<LineRenderer>();
        leg2Renderer = jointPelvis2.GetComponent<LineRenderer>();
        allLineRenderers.Add(torsoRenderer);
        allLineRenderers.Add(torsoOutlineRenderer);
        allLineRenderers.Add(arm1Renderer);
        allLineRenderers.Add(arm2Renderer);
        allLineRenderers.Add(leg1Renderer);
        allLineRenderers.Add(leg2Renderer);

        foreach (LineRenderer renderer in allLineRenderers) {
            renderer.startColor = bodyColor;
            renderer.endColor = bodyColor;
            renderer.startWidth = thickness;
            renderer.endWidth = thickness;
            renderer.positionCount = defaultPositionCount;
            renderer.useWorldSpace = true;
            renderer.numCornerVertices = jointVertices;
            renderer.numCapVertices = jointVertices;
        }
        torsoOutlineRenderer.numCornerVertices = 0;
        torsoOutlineRenderer.positionCount = 7;

        leg1Renderer.startWidth = thickness * 2;
        leg2Renderer.startWidth = thickness * 2;

        Color torsoColor = new Color((bodyColor.r + 1f) / 2f, (bodyColor.g + 1f) / 2f, (bodyColor.b + 1f) / 2f, 1);
        torsoRenderer.startColor = torsoColor;
        torsoRenderer.endColor = torsoColor;
        torsoRenderer.startWidth = thickness * 2;
        torsoRenderer.endWidth = thickness * 2;
        torsoRenderer.positionCount = 3;
        torsoRenderer.numCornerVertices = 0;
        torsoRenderer.numCapVertices = 0;

    }
    void InitRigidbody2DListAndProperties() {
        fighterRB = this.gameObject.GetComponent<Rigidbody2D>();
        int rbCount = 0;
        foreach (GameObject bodyPart in allBodyParts) {
            Rigidbody2D bodyPartRB = bodyPart.GetComponent<Rigidbody2D>();
            if (bodyPartRB != null) {
                allRigidbody2D.Add(bodyPartRB);
                rbCount++;
            }
        }
        //Debug.Log(rbCount + " RigidBodies added to list");
        InitRigidBody2DProperties();
    }
    void InitRigidBody2DProperties() {
        foreach (Rigidbody2D rb2d in allRigidbody2D) {
            rb2d.drag = .5f;
        }
    }
    void InitColliderList() {
        allPolyCollider2D.Add(headLimb.GetComponent<PolygonCollider2D>());
        int bcCount = 1;
        foreach (GameObject bodyPart in allBodyParts) {
            PolygonCollider2D bodyPartBC = bodyPart.GetComponent<PolygonCollider2D>();
            if (bodyPartBC != null) {
                allPolyCollider2D.Add(bodyPartBC);
                bcCount++;
            }
        }
        //Debug.Log(bcCount + " BoxCollider2D added to list");
    }
    void SetColliders(string type) // turns on head and torso, turns off others
    {
        switch (type) {
            case "combat": // purely to check hitboxes for attacks
                foreach (PolygonCollider2D pc2d in allPolyCollider2D) {
                    pc2d.enabled = false;
                    pc2d.isTrigger = true;
                }
                headLimb.GetComponent<PolygonCollider2D>().enabled = true;
                torsoTop.GetComponent<PolygonCollider2D>().enabled = true;
                torsoBottom.GetComponent<PolygonCollider2D>().enabled = true;
                thigh1.GetComponent<PolygonCollider2D>().enabled = true;
                thigh2.GetComponent<PolygonCollider2D>().enabled = true;
                calf1.GetComponent<PolygonCollider2D>().enabled = true;
                calf2.GetComponent<PolygonCollider2D>().enabled = true;

                // every collider is now disabled

                if (isGhost) {
                    headLimb.GetComponent<PolygonCollider2D>().enabled = false;
                    torsoTop.GetComponent<PolygonCollider2D>().enabled = false;
                    torsoBottom.GetComponent<PolygonCollider2D>().enabled = false;
                }
                break;
            case "ragdoll": // interact with physics system
                foreach (PolygonCollider2D pc2d in allPolyCollider2D) {
                    pc2d.enabled = true;
                    pc2d.isTrigger = false;
                }
                break;
        }
    }
    public void SetColor(Color newColorArg) {
        Color newColor = newColorArg;
        Color newTorsoColor = new Color((newColor.r + 1f) / 2f, (newColor.g + 1f) / 2f, (newColor.b + 1f) / 2f, 1);

        if (isGhost) {
            newColor = new Color(newColor.r, newColor.g, newColor.b, 0.1f);
            newTorsoColor = new Color((newColor.r + 1f) / 2f, (newColor.g + 1f) / 2f, (newColor.b + 1f) / 2f, 0.1f);
        }

        headSprite.color = newColor;
        foreach (LineRenderer renderer in allLineRenderers) {
            renderer.startColor = newColor;
            renderer.endColor = newColor;
        }
        torsoRenderer.startColor = newTorsoColor;
        torsoRenderer.endColor = newTorsoColor;
    }
    public void SetRenderSortingLayer(int layer) {
        foreach (LineRenderer renderer in allLineRenderers) {
            renderer.sortingOrder = layer;
        }
        torsoRenderer.sortingOrder = layer - 1;
    }

    // Start is called before the first frame update
    void Awake() {
        // Debug.Log("AWAKE");
        InitGameObjects();
        InitColliderList();
        SetColliders("combat");
        InitRigidbody2DListAndProperties();
        InitHinges();
        InitLineRenderers();
        InitTransforms();

        InitTrailRenderers();

        HideJointsAndStances();

        EqualizeBodyPartMass(false);

        // defaults to enemy tags
        fighterHead.tag = "Enemy";
        torsoTop.tag = "Enemy";
        torsoBottom.tag = "Enemy";
        upperArm1.tag = "Enemy";
        upperArm2.tag = "Enemy";
        lowerArm1.tag = "Enemy";
        lowerArm2.tag = "Enemy";
        thigh1.tag = "Enemy";
        thigh2.tag = "Enemy";
        calf1.tag = "Enemy";
        calf2.tag = "Enemy";

        notInAnimation = true;
        facingRight = true;
        isTurning = false;

        // default stat values 
        defaultMaxHP = 100f;
        defaultMaxEnergy = 100f;
        gainEnergyOn = true;
        defaultEnergyPerSecond = 20f;
        defaultMoveSpeed = 4f / FPS; // x units per 60 frames

        armPower = 1f;
        armPowerDefault = 1f;
        legPowerDefault = 1f;
        legPowerDefault = 1f;

        speedMultiplierDefault = 1f;

        moveSpeed = defaultMoveSpeed * speedMultiplier;
        reach = .75f;

        if (headLimb.transform.position.z != 0f) {
            Debug.Log("head spawned in wrong z value");
        }
    }
    void Start() {
        InitHealthAndEnergyBar(); //THIS NEEDS TO BE IN START, NOT AWAKE
        UpdateDefaultStancePositions(); // need this to set up groundLevel
        groundLevel = Mathf.Min(foot1DefaultPos.y, foot2DefaultPos.y); // keep this in Start()

        particleControllerScript = particleEffectController.GetComponent<ParticleEffectsController>();
        parentObject = transform.parent.gameObject;
        parentRB = parentObject.GetComponent<Rigidbody2D>();
        StartCoroutine(FindAndFixBrokenArmHinges());
    }
    void Update() {
        if (isInvulnerable && InvulnerabilityCoroutine == null) {
            InvulnerabilityCoroutine = InvulnerableColoring();
            StartCoroutine(InvulnerabilityCoroutine);
        }
        KeepZPositionAtZero();
        GainEnergy();
        MoveAndDrawBody(); // for some reason important this is called first
        if (notInAnimation) // if the fighter is not currently in an animation
        {
            MoveTowardsDefaultStance();
        }
        StatusEffects();
    }
    void StatusEffects() // called every update
    {
        // status effects

        if (isPoisonedEffect > 0) {
            // 3 dps + poisonEffect * 3
            float poisonDamage = (poisonDefaultDamage + (float)(isPoisonedEffect - 1) * poisonDamagePerExtraLevel) / FPS;
            ChangeHealth(-1f * poisonDamage, false, "poison");
            UpdateHealthBar();
        }
    }
    void KeepZPositionAtZero() {
        foreach (GameObject bodyPart in allBodyParts) {
            bodyPart.transform.position = new Vector3(
                bodyPart.transform.position.x,
                bodyPart.transform.position.y,
                0f
            );
        }
        foreach (GameObject stance in allStances) {
            stance.transform.position = new Vector3(
                stance.transform.position.x,
                stance.transform.position.y,
                0f
            );
        }
        if (transform.position.z != 0f) {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        }
    }

    IEnumerator FindAndFixBrokenArmHinges() {
        while (true) {
            if (Time.frameCount % 20 == 0 && notInAnimation) {
                UnfuckArm(jointShoulder2, jointElbow2, lowerArm2, stanceHand2);
                UnfuckArm(jointShoulder1, jointElbow1, lowerArm1, stanceHand1);
            }
            yield return null;
        }
    }

    void UnfuckArm(GameObject shoulderLimb, GameObject elbowLimb, GameObject armLimb, GameObject stanceObject) // waits a few frames then fixes 
    {
        float fixAmount = 150f;

        Vector3 lineStart = shoulderLimb.transform.position;
        Vector3 lineEnd = elbowLimb.transform.position;
        Vector3 point = stanceObject.transform.position;

        float determinant = ((lineEnd.x - lineStart.x) * (point.y - lineStart.y) - (lineEnd.y - lineStart.y) * (point.x - lineStart.x));

        if (determinant * transform.localScale.x < 0) {
            //Debug.Log("fucked arm detected");
            armLimb.transform.eulerAngles = new Vector3(0f, 0f, armLimb.transform.eulerAngles.z + fixAmount * transform.localScale.x);
        }
    }
    public void SetCharacterType(string typeArg) // do this as soon as the fighter is created and InitBasedOnCharSettings does the rest
    {
        fighterType = typeArg.ToLower();
    }

    public IEnumerator InitBasedOnCharSettings() // waits a few frames then updates info
    {
        // Debug.Log("starting delay before setting character settings");
        for (int i = 0; i < 2; i++) {
            yield return null;
        }
        string tag = "";

        if (isPlayer) {
            tag = "Player";
        }

        // changing enemy colors
        if (!isPlayer) {
            if (isGhost) {
                tag = "Ghost";
                SetRenderSortingLayer(0);
            }
            else {
                tag = "Enemy";
                SetRenderSortingLayer(1);
                SetStances("all");
            }

            switch (fighterType) {
                case "acolyte":
                    SetColor(Color.white);
                    break;
                case "trickster":
                    SetColor(Color.green);
                    break;
                case "brawler":
                    SetColor(Color.red);
                    break;
            }
        }

        SetTags(tag);

        if (isPlayer) {
            // Debug.Log("UpdateBasedOnCharSettings sitrep: hp: " + hp + "/" + maxhp + ", speedMulti,powerMulti = " + speedMultiplier + ", " + damageMultiplier);
        }
        InitClassCombatStats();
    }
    public void InitClassCombatStats() {
        // first sets default values
        speedMultiplier = 1f;
        legPower = 1f;
        maxhp = 100f;
        hp = 100f;

        // set number stats
        switch (fighterType) {
            case "acolyte":
                // Debug.Log("setting acolyte");
                defaultMaxHP = 200f;
                defaultMaxEnergy = 100f;
                defaultEnergyPerSecond = 20f;

                speedMultiplierDefault = 1f;

                armPowerDefault = 1f;
                legPowerDefault = 1f;
                break;
            case "brawler":
                // Debug.Log("setting brawler");

                defaultMaxHP = 320f;
                defaultMaxEnergy = 100f;
                defaultEnergyPerSecond = 20f;

                speedMultiplierDefault = 0.8f;

                armPowerDefault = 1.4f;
                legPowerDefault = 1.2f;
                break;
            case "trickster":
                // Debug.Log("setting trickster");

                defaultMaxHP = 120f;
                defaultMaxEnergy = 100f;
                defaultEnergyPerSecond = 20f;

                speedMultiplierDefault = 1.3f;

                armPowerDefault = .8f;
                legPowerDefault = .9f;
                break;
        }

        maxhp = defaultMaxHP;
        ReplenishHealth();

        maxEnergy = defaultMaxEnergy;
        energyPerSecond = defaultEnergyPerSecond;
        ReplenishEnergy();

        speedMultiplier = speedMultiplierDefault;
        moveSpeed = defaultMoveSpeed * speedMultiplier;
        armPower = armPowerDefault;
        legPower = legPowerDefault;
        /*
        if (isPlayer)
        {
            maxhp *= 2f;
            hp *= 2f;
        }

        */
        // Debug.Log("After initializing " + characterType + ", hp: " + hp + "/" + maxhp + ", speedMulti,powerMulti = " + speedMultiplier + ", " + damageMultiplier);

        UpdateHealthBar();
        UpdateEnergyBar();
    }
    void MoveTowardsDefaultStance()
    // moves at speed towards default positions of hands and feet
    {
        float fighterX = transform.position.x;
        float fighterY = transform.position.y;
        float fighterHeadX = transform.position.x + stanceHeadTran.position.x;
        float fighterHeadY = transform.position.y + stanceHeadTran.position.y;

        // these need to be transform.right not orientedTran.right 
        if (stanceHeadTran.position.x > fighterX + reach) {
            stanceHeadTran.position -= transform.right * moveSpeed;
        }
        if (stanceHeadTran.position.x < fighterX - reach) {
            stanceHeadTran.position += transform.right * moveSpeed;
        }
        if (stanceHeadTran.position.y < fighterY - reach) {
            stanceHeadTran.position += transform.up * moveSpeed;
        }
        if (stanceHeadTran.position.y > fighterY + reach) {
            stanceHeadTran.position -= transform.up * moveSpeed;
        }

        UpdateDefaultStancePositions();

        // hands
        float distance = Vector3.Distance(stanceHand1Tran.position, hand1DefaultPos);
        if (distance > 0.1f) {
            stanceHand1Tran.LookAt(hand1DefaultPos);
            stanceHand1Tran.position += stanceHand1Tran.forward * Mathf.Max(moveSpeed * distance * 2, moveSpeed);
        }

        distance = Vector3.Distance(stanceHand2Tran.position, hand2DefaultPos);
        if (distance > 0.1f) {
            stanceHand2Tran.LookAt(hand2DefaultPos);
            stanceHand2Tran.position += stanceHand2Tran.forward * Mathf.Max(moveSpeed * distance * 2, moveSpeed);
        }

        // feet
        if (walkAnimationCoroutine == null) {
            distance = Vector3.Distance(stanceFoot1Tran.position, foot1DefaultPos);
            if (distance > 0.1f) {
                stanceFoot1Tran.LookAt(foot1DefaultPos);
                stanceFoot1Tran.position += stanceFoot1Tran.forward * Mathf.Max(moveSpeed * distance * 2f, moveSpeed);
            }

            distance = Vector3.Distance(stanceFoot2Tran.position, foot2DefaultPos);
            if (distance > 0.1f) {
                stanceFoot2Tran.LookAt(foot2DefaultPos);
                stanceFoot2Tran.position += stanceFoot2Tran.forward * Mathf.Max(moveSpeed * distance * 2f, moveSpeed);
            }
        }
    }
    void UpdateDefaultStancePositions()
    // used within MoveTowardsDefaultStance()
    {
        float localScaleX = transform.localScale.x;
        hand1DefaultPos =
            stanceHeadTran.position
            + torsoBottom.transform.right * 0.75f * localScaleX
            - torsoBottom.transform.up * 0.5f;
        hand2DefaultPos =
            stanceHeadTran.position
            + torsoBottom.transform.right * 1.5f * localScaleX
            - torsoBottom.transform.up * 0.25f;
        foot1DefaultPos = transform.position - orientedTran.up * 4f - transform.right * 1f * localScaleX;
        foot2DefaultPos = transform.position - orientedTran.up * 4f + transform.right * 1f * localScaleX;
    }
    void MoveAndDrawBody() //moves limbs to desired location, then positions the LineRenderers to where the joints are
    {
        // stances are used to drag around limbs while letting the physics engine rotate the limbs
        // if stances are active, they pull limbs around regardless of the physics engine
        // if stances are not active, they are instead pulled around by the physics engine
        // stances and limbs will always be in the same positions

        // if out of bounds
        if (transform.position.x < -10f) {
            transform.position += Vector3.right * moveSpeed;
        }
        if (transform.position.x > 10f) {
            transform.position -= Vector3.right * moveSpeed;
        }

        // head and hands
        if (stanceHeadActive) {
            fighterHead.transform.position = stanceHeadTran.position;
        }
        else {
            stanceHeadTran.position = fighterHead.transform.position;
        }

        if (stanceHand1Active) {
            lowerArm1Tran.position = stanceHand1Tran.position;
        }
        else {
            stanceHand1Tran.position = lowerArm1Tran.position;
        }
        if (stanceHand2Active) {
            lowerArm2Tran.position = stanceHand2Tran.position;
        }
        else {
            stanceHand2Tran.position = lowerArm2Tran.position;
        }


        // forces the shoulder to connect to the elbow
        upperArm1Tran.position = jointElbow1Tran.position;
        upperArm2Tran.position = jointElbow2Tran.position;

        // torso stances
        if (stanceTorsoBotActive) {
            torsoBottom.transform.position = stanceTorsoBotTran.position;
        }
        else {
            stanceTorsoBotTran.position = torsoBottom.transform.position;
        }

        if (stanceTorsoTopActive) {
            torsoTop.transform.position = stanceTorsoTopTran.position;
        }
        else {
            stanceTorsoTopTran.position = torsoTop.transform.position;
        }

        // feet stances
        if (stanceFoot1Active) {
            calf1Tran.position = stanceFoot1Tran.position;
        }
        else {
            stanceFoot1Tran.position = calf1Tran.position;
        }
        if (stanceFoot2Active) {
            calf2Tran.position = stanceFoot2Tran.position;
        }
        else {
            stanceFoot2Tran.position = calf2Tran.position;
        }

        // TORSO COLORING
        Vector3[] torsoPoints = {
            jointNeckTran.position,
            (jointRib1Tran.position + jointRib2Tran.position)/2f,
            stanceTorsoBotTran.position,
        };
        torsoRenderer.SetPositions(torsoPoints);

        // ARM 1
        if (drawNormalElbow1) {
            Vector3[] arm1points = {
            jointShoulder1Tran.position,
            upperArm1Tran.position,
            lowerArm1Tran.position
            };
            arm1Renderer.SetPositions(arm1points);
        }
        else {
            Vector3[] arm1points = {
            jointShoulder1Tran.position,
            customElbow1Tran.position,
            lowerArm1Tran.position
            };
            arm1Renderer.SetPositions(arm1points);
        }

        // ARM 2
        if (drawNormalElbow2) {
            Vector3[] arm2points = {
            jointShoulder2Tran.position,
            upperArm2Tran.position,
            lowerArm2Tran.position
            };
            arm2Renderer.SetPositions(arm2points);
        }
        else {
            Vector3[] arm2points = {
            jointShoulder2Tran.position,
            customElbow2Tran.position,
            lowerArm2Tran.position
            };
            arm2Renderer.SetPositions(arm2points);
        }

        // LEG 1
        if (drawNormalKnee1) {
            Vector3[] leg1points = {
            jointPelvis1Tran.position,
            jointKnee1Tran.position,
            stanceFoot1Tran.position
            };
            leg1Renderer.SetPositions(leg1points);
        }
        else {
            Vector3[] leg1points = {
            jointPelvis1Tran.position,
            customKnee1Tran.position,
            stanceFoot1Tran.position
            };
            leg1Renderer.SetPositions(leg1points);
        }

        // LEG 2
        if (drawNormalKnee2) {
            Vector3[] leg2points = {
            jointPelvis2Tran.position,
            jointKnee2Tran.position,
            stanceFoot2Tran.position
            };
            leg2Renderer.SetPositions(leg2points);
        }
        else {
            Vector3[] leg2points = {
            jointPelvis2Tran.position,
            customKnee2Tran.position,
            stanceFoot2Tran.position
            };
            leg2Renderer.SetPositions(leg2points);
        }

        // TORSO OUTLINE
        Vector3[] torsoOutlinePoints = {
            jointNeckTran.position,
            jointShoulder1Tran.position,
            jointRib1Tran.position,
            jointPelvis1Tran.position,
            jointPelvis2Tran.position,
            jointRib2Tran.position,
            jointShoulder2Tran.position,
            };
        torsoOutlineRenderer.SetPositions(torsoOutlinePoints);
    }

    public void SetStances(string type) {
        bool turnOn = true;
        switch (type) {
            case "all":
                turnOn = true;
                break;
            case "none":
                turnOn = false;
                break;
        }
        stanceHeadActive = turnOn;
        stanceTorsoTopActive = turnOn;
        stanceTorsoBotActive = turnOn;
        stanceHand1Active = turnOn;
        stanceHand2Active = turnOn;
        stanceFoot1Active = turnOn;
        stanceFoot2Active = turnOn;

        if (type == "combat") {
            stanceTorsoTopActive = false;
            stanceTorsoBotActive = false;
        }
    }
    private void EqualizeBodyPartMass(bool equalize) {
        foreach (Rigidbody2D rb2d in allRigidbody2D) {
            rb2d.mass = 1f;
        }

        if (equalize) {
            return;
        }
        else {
            fighterHead.GetComponent<Rigidbody2D>().mass = 100f;
            torsoTop.GetComponent<Rigidbody2D>().mass = 10f;
        }
    }
    private void EnableGravity(bool gravityOn) {
        float gravityScale = 0f;
        if (gravityOn) {
            gravityScale = 1f;
        }
        foreach (Rigidbody2D rb2d in allRigidbody2D) {
            rb2d.gravityScale = gravityScale;
        }
    }
    public void SetRagdoll(bool ragdoll) {
        if (ragdoll) {
            //Debug.Log("ragdoll set true");
            notInAnimation = false;
            EnableGravity(true);
            EqualizeBodyPartMass(true);
            SetColliders("ragdoll");
            SetStances("none");
        }
        else {
            //Debug.Log("ragdoll set false");
            notInAnimation = true;
            EnableGravity(false);
            EqualizeBodyPartMass(false);
            SetColliders("combat");
            SetStances("combat");
        }
    }
    public void TakeHealing(float healing, string healingType) {
        ChangeHealth(healing, false, healingType);
    }
    public void TakeDamage(float damage, bool isCrit, string damageType) {
        if (isInvulnerable) {
            ChangeHealth(0f, false, "negated");
            return;
        }
        float damageTaken = damage * -1f;
        ChangeHealth(damageTaken, isCrit, damageType);
    }
    private void ChangeHealth(float change, bool isCrit, string changeType) {
        // prevent poison from killing
        if (changeType == "poison" && hp <= 1) {
            return;
        }

        float amountChanged = change;
        hp += change;

        // does not let health go above max
        if (hp > maxhp) {
            amountChanged = maxhp - hp;
            hp = maxhp;
        }
        // does not let health drop below 0
        if (hp <= 0f) {
            amountChanged = hp;
            hp = 0f;
        }

        if (Mathf.Abs(amountChanged) > 1f && changeType != "blocked") {
            GameObject changeNumber = Instantiate(damageNumberPrefab, stanceHeadTran.position + Vector3.up, transform.rotation);
            DamageNumberSprite numberScript = changeNumber.GetComponent<DamageNumberSprite>();
            numberScript.type = "health";
            numberScript.changeNumber = change;
            numberScript.isCrit = isCrit;
        }

        // damage was dealt
        if (amountChanged < 0f) {
            if (!isPlayer) {
                // player dealt damage to enemies
                switch (changeType) {
                    case "physical":
                        gameStateManagerScript.damageDealtCurrent += amountChanged;
                        break;
                    case "explosive":
                        gameStateManagerScript.explosiveDamageCurrent += amountChanged;
                        break;
                    case "poison":
                        gameStateManagerScript.poisonDamageDealtCurrent += amountChanged;
                        break;
                }
            }
            // enemy dealt damage to player
            if (isPlayer) {
                gameStateManagerScript.damageTakenCurrent += amountChanged;
            }
        }
        else {
            // healing
            if (isPlayer) {
                switch (changeType) {
                    case "checkpoint":
                        gameStateManagerScript.checkpointHealingCurrent += amountChanged;
                        break;
                    case "vampirism":
                        gameStateManagerScript.vampirismHealingCurrent += amountChanged;
                        break;
                }
            }
        }

        UpdateHealthBar();
    }
    public void SetHealth(float amount) {
        hp = amount;
        if (hp > maxhp) {
            hp = maxhp;
        }
        UpdateHealthBar();
    }
    public void ChangeEnergy(float change) {
        currentEnergy += change;
        if (currentEnergy > maxEnergy) {
            currentEnergy = maxEnergy;
        }
        UpdateEnergyBar();

        if (Mathf.Abs(change) < 1f || !isPlayer) {
            return;
        }
        GameObject damageNumber = Instantiate(damageNumberPrefab, stanceHeadTran.position + Vector3.up, transform.rotation);
        damageNumber.GetComponent<DamageNumberSprite>().type = "energy";
        damageNumber.GetComponent<DamageNumberSprite>().changeNumber = change;
    }
    public void ReplenishEnergy() {
        currentEnergy = maxEnergy;
    }
    public void ReplenishHealth() {
        hp = maxhp;
        UpdateHealthBar();
    }
    void GainEnergy() {
        if (!gainEnergyOn) {
            return;
        }
        else {
            ChangeEnergy(energyPerSecond / FPS);
        }
    }
    public void UpdateHealthBar() {
        if (healthBar == null) {
            return;
        }
        if (hp <= 0) {
            healthBar.transform.localScale = new Vector3(0f, 0f, 0f);
            return;
        }

        float percentage = hp / maxhp;
        healthBar.transform.localScale = new Vector3(
            2f * percentage * transform.localScale.x,
            healthBarScaleY,
            1f
            );
        healthBarBackground.transform.localScale = new Vector3(
            2f * transform.localScale.x,
            healthBarScaleY,
            1f
            );

        healthBarHeight = new Vector3(0f, transform.localScale.y * 0.5f, 0f);

        healthBar.transform.position = new Vector3(
            (headLimb.transform.position.x - (1f - percentage) * healthBarScaleX / 2f),
            headLimb.transform.position.y,
            1
            )
            + healthBarHeight;
    }
    public void UpdateEnergyBar() {
        if (energyBar == null) {
            return;
        }
        if (currentEnergy <= 0f) {
            energyBar.transform.localScale = new Vector3(0f, 0f, 0f);
            return;
        }

        float percentage = currentEnergy / maxEnergy;
        energyBar.transform.localScale = new Vector3(
            energyBarScaleX * percentage * transform.localScale.x,
            energyBarScaleY,
            1f
            );
        energyBarBackground.transform.localScale = new Vector3(
            2f * transform.localScale.x,
            energyBarScaleY,
            1f
            );

        energyBarHeight = new Vector3(0f, transform.localScale.y * 0.4f, 0f);

        energyBar.transform.position = new Vector3(
            (headLimb.transform.position.x - (1f - percentage) * energyBarScaleX / 2f),
            headLimb.transform.position.y,
            1f
            )
            + energyBarHeight;
    }
    public void Die() {
        UpdateEnergyBar();
        UpdateHealthBar();
        drawNormalElbow1 = true;
        drawNormalElbow2 = true;
        drawNormalKnee1 = true;
        drawNormalKnee2 = true;
        Destroy(healthBar);
        Destroy(healthBarBackground);
        Destroy(energyBar);
        Destroy(energyBarBackground);
        StopAllCoroutines();
        SetRagdoll(true);
        if (isPlayer) {
            gameStateManagerScript.GameOver();
            return;
        }
        if (!isGhost && !isPlayer) {
            gameStateManagerScript.AddScore(100);
            gameStateManagerScript.enemiesKilledCurrent++;
        }
        Destroy(this.transform.root.gameObject, 5);
    }
    public void PushParent(Vector3 pushVector) {
        if (myFightingStyle == "muaythai") {
            pushVector = 0.75f * pushVector;
        }
        parentRB.AddForce(pushVector, ForceMode2D.Impulse);
        StartCoroutine(ParentWasPushed());
    }
    public IEnumerator ParentWasPushed() {
        while (Vector3.Magnitude(parentRB.velocity) > 0f) {
            transform.position += new Vector3(
                parentRB.velocity.x / FPS,
                parentRB.velocity.y / FPS,
                0f);
            yield return null;
        }
    }
    public void MoveBody(Vector3 direction) // put an animation on this shit while it's happening
    {
        float walkSpeed = moveSpeed * 0.75f;
        bool walkingRight = (direction.x > 0);
        if (isAirborne || !notInAnimation) // will not move in air/attack animation
        {
            return;
        }

        if (isPlayer) // player limitations
        {
            if (Mathf.Abs(transform.position.x) > 10f) {
                return;
            }
        }
        walkSpeed = WalkSpeedOnConditions(walkSpeed, walkingRight);
        transform.position += Vector3.Normalize(direction) * walkSpeed;
        if (walkAnimationCoroutine == null) {
            //walkAnimationCoroutine = WalkAnimation1(direction);
            //StartCoroutine(walkAnimationCoroutine);
        }
    }
    private float WalkSpeedOnConditions(float walkSpeedArg, bool walkingRight) {
        float output = walkSpeedArg;
        string debugString = "";

        // not walking and facing same direction
        if ((!walkingRight && facingRight) || (walkingRight && !facingRight)) {
            walkSpeedArg *= 0.75f;
            debugString += "walking backwards, ";
        }
        else {
            debugString += "walking forwards, ";
        }

        // crouching
        // slow if lower than top sector
        float yTop = transform.position.y;
        float yHeadPos = stanceHeadTran.position.y;
        float crouchMultiplier = 1f;
        if (yHeadPos < yTop) {
            float yBottom = transform.position.y - reach;
            float distanceTopToBot = yTop - yBottom;
            crouchMultiplier = 0.75f + 0.25f * ((yHeadPos - yBottom) / distanceTopToBot);
            output *= crouchMultiplier;
            debugString += "crouching " + crouchMultiplier + ", ";
        }
        else {
            debugString += "not crouching, ";
        }

        // leaning the opposite way of walking
        float directionMultiplier = 1f;
        if ((!walkingRight && facingRight)
        || (walkingRight && !facingRight)) {
            directionMultiplier = -1f;
        }
        float xLimitTop = reach * directionMultiplier;
        float xHeadLocalPos = stanceHeadTran.localPosition.x;
        float leanMultiplier = 1f;
        if (xHeadLocalPos < xLimitTop) {
            float xLimitBottom = 0 - reach * directionMultiplier;
            float distanceToBack = xLimitTop - xLimitBottom;
            leanMultiplier = 0.75f + 0.25f * ((xHeadLocalPos - xLimitBottom) / distanceToBack);
            output *= leanMultiplier;
        }
        debugString += "leaning * " + leanMultiplier + ", ";

        // walking AROUND a hostile if they are too close and in the direction you are walking
        GameObject hostileFighter = null;
        if (isPlayer) { // checks until finding an enemy that slows the player
            foreach (GameObject enemy in gameStateManagerScript.enemyManagerScript.GetAllEnemiesList()) {
                float origOutput = output;
                hostileFighter = enemy.GetComponent<EnemyWithGhostScript>().enemyFighter;
                output = WalkSlowAtCloseHostile(output, walkingRight, transform.position, hostileFighter.transform.position);
                if (origOutput != output) {
                    break;
                }
            }
        }
        else {
            hostileFighter = this.gameObject;
            output = WalkSlowAtCloseHostile(output, walkingRight, transform.position, hostileFighter.transform.position);
        }

        //Debug.Log(debugString);
        //Debug.Break();
        return output;
    }

    private float WalkSlowAtCloseHostile(float walkSpeedArg, bool walkingRight, Vector3 thisFighterPos, Vector3 hostileFighterPos) {
        float output = walkSpeedArg;
        float distance = hostileFighterPos.x - thisFighterPos.x;
        if (Mathf.Abs(distance) < 4f) { // an enemy is close, now will check direction
            if ((distance > 0 && walkingRight) // walking right towards enemy to right
            || (distance < 0 && !walkingRight) // walkiing left towards enemy to left
            ) {
                output *= 0.5f;
                return output;
            }
        }
        return output;
    }

    private IEnumerator WalkAnimation1(Vector3 direction) {
        Debug.Log("starting walk animation");
        int framesPerPart = 7;

        /*Transform firstFootTran = null;
        Transform secondFootTran = null;
        if (direction.x > 0)
        {
            if (facingRight)
            {
                firstFootTran = stanceFoot2Tran;
                foot2Tran = stanceFoot1Tran;
            }
            else
            {

            }
        }
        else
        {

        }*/
        Vector3 localFoot1PosStart = stanceFoot1Tran.localPosition;
        Vector3 localFoot2PosStart = stanceFoot2Tran.localPosition;
        Vector3 moveInDirection = Vector3.Normalize(direction) * moveSpeed / 2f;
        for (int i = 0; i < framesPerPart * 2; i++) {
            stanceFoot2Tran.localPosition += moveInDirection;
            stanceFoot2Tran.localPosition += Vector3.up * moveSpeed / 2f;

            yield return null;
        }
        for (int i = 0; i < framesPerPart; i++) {
            stanceFoot2Tran.localPosition -= moveInDirection;
            stanceFoot2Tran.localPosition -= Vector3.up * moveSpeed / 2f;

            stanceFoot1Tran.localPosition += moveInDirection;
            stanceFoot1Tran.localPosition += Vector3.up * moveSpeed / 2f;
            yield return null;
        }
        for (int i = 0; i < framesPerPart; i++) {
            stanceFoot2Tran.localPosition -= moveInDirection;
            stanceFoot2Tran.localPosition -= Vector3.up * moveSpeed / 2f;

            stanceFoot1Tran.localPosition += moveInDirection;
            stanceFoot1Tran.localPosition += Vector3.up * moveSpeed / 2f;
            yield return null;
        }
        for (int i = 0; i < framesPerPart * 2; i++) {
            stanceFoot1Tran.localPosition -= moveInDirection;
            stanceFoot1Tran.localPosition -= Vector3.up * moveSpeed / 2f;
            yield return null;
        }
        walkAnimationCoroutine = null;
    }
    public Vector3 GetSectorPosition(int sector) {
        Vector3 output = new Vector3(0, 0, 0);
        float reachForX = reach * transform.localScale.x;
        switch (sector) {
            case 0: // bottom back
                output = new Vector3(
                    transform.position.x - reachForX,
                    transform.position.y - reach, 0);
                break;
            case 1: // bottom middle
                output = new Vector3(
                    transform.position.x,
                    transform.position.y - reach, 0);
                break;
            case 2: // bottom forward
                output = new Vector3(
                    transform.position.x + reachForX,
                transform.position.y - reach, 0);
                break;
            case 3: // center back
                output = new Vector3(
                    transform.position.x - reachForX,
                    transform.position.y, 0);
                break;
            case 4: // true center
                output = new Vector3(
                    transform.position.x,
                    transform.position.y, 0);
                break;
            case 5: // center forward
                output = new Vector3(
                    transform.position.x + reachForX,
                    transform.position.y, 0);
                break;
            case 6: // top back
                output = new Vector3(
                    transform.position.x - reachForX,
                    transform.position.y + reach, 0);
                break;
            case 7: // top center
                output = new Vector3(
                    transform.position.x,
                    transform.position.y + reach, 0);
                break;
            case 8: // top forward
                output = new Vector3(
                    transform.position.x + reachForX,
                    transform.position.y + reach, 0);
                break;
        }
        return output;
    }
    public Vector3 GetHeadDirectionToSector(int sector) {
        Vector3 sectorPos = GetSectorPosition(sector);
        Vector3 direction = sectorPos - stanceHeadTran.position;
        return Vector3.Normalize(direction);
    }
    public void MoveHead(int direction) {
        switch (direction) {
            case 1: // up
                MoveHeadInDirection(Vector3.up);
                break;
            case 2: // down
                MoveHeadInDirection(Vector3.up * -1f);
                break;
            case 3: // left
                MoveHeadInDirection(Vector3.right * -1f);
                break;
            case 4: // right
                MoveHeadInDirection(Vector3.right);
                break;
        }
    }
    private void MoveHeadInDirection(Vector3 direction) {
        float finalMoveSpeed = moveSpeed;
        if (!notInAnimation) {
            if (myFightingStyle == "taekwondo") {
                finalMoveSpeed /= 1.5f;
            }
            else {
                finalMoveSpeed /= 3f;
            }
        }
        // prevents player from slowing down the turn
        if (isTurning) {
            direction = new Vector3(0f, direction.y, 0f);
        }

        Vector3 expectedPos = stanceHeadTran.position + (direction * finalMoveSpeed);
        if (IsPositionWithinSectors(expectedPos)) {
            stanceHeadTran.position = expectedPos;
        }
    }
    public void MoveHeadAtPosition(Vector3 targetPosition) {
        targetPosition = new Vector3(targetPosition.x, targetPosition.y, 0f);
        Vector3 direction = targetPosition - stanceHeadTran.position;
        direction = Vector3.Normalize(direction);
        MoveHeadInDirection(direction);
    }
    public void MoveHeadTowardsSector(int sector) {
        // 0 1 2 = bottom back, bottom, bottom forward
        // 3 4 5 = center back, true center, center forward
        // 6 7 8 = top back, top, top forard
        MoveHeadInDirection(GetHeadDirectionToSector(sector));
    }
    public bool IsHeadWithinSectors() {
        return IsPositionWithinSectors(stanceHeadTran.position);
    }
    public bool IsPositionWithinSectors(Vector3 posVector) {
        if (posVector.x > transform.position.x + reach || posVector.y > transform.position.y + reach) {
            return false;
        }
        if (posVector.x < transform.position.x - reach || posVector.y < transform.position.y - reach) {
            return false;
        }
        return true;
    }
    // finds what sector the head is in, in order to do a
    public int GetHeadSector() {
        string[] sectors = {
        "bottom back", "bottom", "bottom forward",
        "center back", "true center", "center forward",
        "top back", "top", "top forward"
        };

        int xSector = 1;
        float fighterHeadX = stanceHeadTran.position.x;
        float fighterX = transform.position.x;
        float fighterHeadToCenterX = (fighterHeadX - fighterX) * transform.localScale.x;
        if (fighterHeadToCenterX < 0 - reach / 3) // on lean back side
        {
            xSector = 0;
        }
        if (fighterHeadToCenterX > reach / 3) // on lean forward side
        {
            xSector = 2;
        }

        int ySector = 1;
        float fighterHeadY = stanceHeadTran.position.y;
        float fighterY = transform.position.y;
        float fighterHeadToCenterY = (fighterHeadY - fighterY) * transform.localScale.y;
        if (fighterHeadToCenterY < 0 - reach / 3) // on bottom side
        {
            ySector = 0;
        }
        if (fighterHeadToCenterY > reach / 3) // on top side
        {
            ySector = 2;
        }

        int sector = ySector * 3 + xSector;
        //Debug.Log(sectors[sector] + " sector");
        return sector;
    }
    public void SwapHingeAngles() {
        foreach (HingeJoint2D hinge in allHinges) {
            JointAngleLimits2D newLimits = hinge.limits;
            newLimits.min = 0 - hinge.limits.min;
            newLimits.max = 0 - hinge.limits.max;
            hinge.limits = newLimits;
        }
    }
    public void TurnBody() {

        float targetScale = 0 - transform.localScale.x; // can't use directionMultiplier for this!!!
        transform.localScale = new Vector3(targetScale, 1f, 1f);

        switch (transform.localScale.x) {
            case 1: // go from right to left
                facingRight = true;
                break;
            case -1: // go from left to right
                facingRight = false;
                break;
        }

        Vector3 orienter = transform.position;
        orienter -= orientedTran.forward;
        orientedTran.LookAt(orienter);
        UpdateHealthBar();
        UpdateEnergyBar();
    }
    IEnumerator GoToCenterXAndTurn() {
        if (isTurning) {
            yield break;
        }
        isTurning = true;
        notInAnimation = false;

        float directionMultiplier = orientedTran.position.x - stanceHeadTran.position.x;
        Vector3 movementVector = transform.right * directionMultiplier * moveSpeed * 2f;
        while (Mathf.Abs(transform.position.x - stanceHeadTran.position.x) > 0.1f) {
            stanceHand1Tran.position += movementVector;
            stanceHand2Tran.position += movementVector;
            stanceHeadTran.position += movementVector;
            yield return null;
        }

        // one more iteration so that it doesnt fuck up (for some reason this fixes a weird turning glitch that happens if you turn too fast)
        stanceHand1Tran.position += movementVector;
        stanceHand2Tran.position += movementVector;
        stanceHeadTran.position += movementVector;

        TurnBody();
        SwapHingeAngles();
        notInAnimation = true;
        isTurning = false;
        //Debug.Log("facing right: " + facingRight);
    }

    IEnumerator Spin180Animation() {
        for (int i = 0; i < 180; i++) {
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y - 1f, 0);
            yield return null;
        }
    }
    public void TurnTo(string direction) {
        if (!notInAnimation || isTurning) {
            return;
        }
        // only turns if it is not already facing the direction
        if (
            (direction == "right" && facingRight)
            || (direction == "left" && !facingRight)
            ) {
            return;
        }
        StartCoroutine(GoToCenterXAndTurn());
        MoveAndDrawBody();
    }

    /*---------------------------------------------------------------------------
    ---------------------------------------------------------------------------
    ---------------------------------------------------------------------------
    // ATTACKS AND ANIMATIONS BELOW
    ---------------------------------------------------------------------------
    ---------------------------------------------------------------------------
    ---------------------------------------------------------------------------
    */

    public void ApplyMyStyleDefaultMoveset() {
        string[][] newMoveset = new string[2][]{
            new string[9],
            new string[9]
        };
        Debug.Log(gameStateManagerScript.GetDefaultMovesetOf(myFightingStyle)[0][0]);
        newMoveset[0] = gameStateManagerScript.GetDefaultMovesetOf(myFightingStyle)[0];
        newMoveset[1] = gameStateManagerScript.GetDefaultMovesetOf(myFightingStyle)[1];
        ApplyNewMoveset(newMoveset[0], newMoveset[1]);
    }
    public void ApplyNewMoveset(string[] newArmMoveset, string[] newLegMoveset) {
        for (int i = 0; i < 9; i++) {
            armMoveset[i] = newArmMoveset[i];
            legMoveset[i] = newLegMoveset[i];
        }
    }

    public void Attack(string attackType) {
        // attackType = "arms" or "legs"
        if (!notInAnimation || !IsHeadWithinSectors()) {
            return;
        }
        string[] sectors = {
        "bottom back", "bottom", "bottom forward",
        "center back", "true center", "center forward",
        "top back", "top", "top forward"
        };

        if (attackType == "groundslam") { // temp disabled
            //            StartCoroutine(GroundSlam());
            return;
        }

        int sector = GetHeadSector();

        switch (attackType) {
            case "arms":
                UseSelectedMove("arms", armMoveset[sector]);
                break;
            case "legs":
                UseSelectedMove("legs", legMoveset[sector]);
                break;
        }
    }
    public void UseSelectedMove(string armsOrLegs, string moveName) {
        if (moveName == "none") {
            Debug.Log("none selected for this sector");
            return;
        }
        switch (armsOrLegs) {
            case "arms":
                switch (moveName) {
                    case "hook":
                        moveCoroutine = Hook(false);
                        StartCoroutine(moveCoroutine);
                        return;
                    case "jab combo":
                        StartCoroutine(JabCombo("combo"));
                        return;
                    case "fast jab":
                        StartCoroutine(JabCombo("fast"));
                        return;
                    case "uppercut":
                        StartCoroutine(Uppercut());
                        return;
                    case "chainpunch":
                        StartCoroutine(ChainPunch());
                        return;
                    case "elbow":
                        StartCoroutine(Elbow());
                        return;
                }
                Debug.Log(moveName + " does not exist for arm attacks");
                return;
            case "legs":
                switch (moveName) {
                    case "roundhousekick":
                        StartCoroutine(RoundhouseKick("straight"));
                        return;
                    case "roundhousekickhigh":
                        StartCoroutine(RoundhouseKick("high"));
                        return;
                    case "knee":
                        StartCoroutine(Knee("grounded"));
                        return;
                    case "pushkick":
                        StartCoroutine(PushKick("grounded"));
                        return;
                    case "flyingkick":
                        StartCoroutine(FlyingKick());
                        return;
                }
                Debug.Log(moveName + " does not exist for leg attack");
                return;
        }
    }
    public float[] GetAttackInfo(string attackID) {
        float[] output = new float[4]; // output = [damage, knockback multiplier, energy cost, default time taken]
        float damage = -1f;
        float pushMultiplier = -1f;
        float energyCost = -1f;
        float timeTaken = -1f;
        string type = "none";
        switch (attackID.ToLower()) {
            case "hook":
                type = "arms";
                damage = 50f * armPower;
                pushMultiplier = 1f;
                energyCost = 15f * armPower;
                timeTaken = .25f;
                break;
            case "fast jab":
                type = "arms";
                damage = 30f * armPower;
                pushMultiplier = 2f;
                energyCost = 13f * armPower;
                timeTaken = .12f;
                break;
            case "jab combo":
                type = "arms";
                damage = 30f * armPower;
                pushMultiplier = 2f;
                energyCost = 25f * armPower;
                timeTaken = .12f;
                break;
            case "roundhousekick":
                type = "legs";
                damage = 60f * legPower;
                pushMultiplier = 0.5f;
                energyCost = 40f * legPower;
                timeTaken = .3f;
                break;
            case "roundhousekickhigh":
                type = "legs";
                damage = 70f * legPower;
                pushMultiplier = 0.5f;
                energyCost = 50f * legPower;
                timeTaken = .36f;
                break;
            case "pushkick":
                type = "legs";
                damage = 60f * legPower;
                pushMultiplier = 2f;
                energyCost = 45f * legPower;
                timeTaken = .35f;
                break;
            case "flyingkick":
                type = "legs";
                damage = 120 * legPower;
                pushMultiplier = 2f;
                energyCost = 70f * legPower;
                timeTaken = .35f;
                break;
            case "uppercut":
                type = "arms";
                damage = 60f * armPower;
                pushMultiplier = 1f;
                energyCost = 18f * armPower;
                timeTaken = .25f;
                break;
            case "groundslam":
                type = "special";
                damage = 60f;
                pushMultiplier = 2.5f;
                energyCost = 100f;
                timeTaken = 0.8f;
                break;
            case "knee":
                type = "legs";
                damage = 65f * legPower;
                pushMultiplier = 1f;
                energyCost = 20f * legPower;
                timeTaken = 0.25f;
                break;
            case "chainpunch":
                type = "arms";
                damage = 25f;
                pushMultiplier = 0.5f;
                energyCost = 15f * armPower;
                timeTaken = 0.22f;
                break;
            case "elbow":
                type = "arms";
                damage = 70f;
                pushMultiplier = 1.5f;
                energyCost = 10f * armPower;
                timeTaken = 0.15f;
                break;
        }

        if (isPlayer && notInAnimation) // adds to counter of punches/kicks thrown
        {
            if (energyCost > currentEnergy) {
                type = "no energy";
            }
            else {
                if (walkAnimationCoroutine != null) {
                    StopCoroutine(walkAnimationCoroutine);
                    walkAnimationCoroutine = null;
                }
                switch (type) {
                    case "arms":
                        gameStateManagerScript.armAttacksUsedCurrent++;
                        break;
                    case "legs":
                        gameStateManagerScript.legAttacksUsedCurrent++;
                        break;
                    case "special":
                        gameStateManagerScript.specialAttacksUsedCurrent++;
                        break;
                }
                SuccessfulAttackStart(attackID);
            }
        }

        timeTaken /= speedMultiplier;

        if (isWeakenedEffect > 0) {

        }

        if (explosiveLevel > 0) {
            pushMultiplier *= (1f + (float)explosiveLevel * 0.20f);
        }
        if (lightningLevel > 0) {
            timeTaken /= 1f + ((float)lightningLevel * 0.1f);
        }

        output[0] = damage;
        output[1] = pushMultiplier;
        output[2] = energyCost;
        output[3] = timeTaken;
        notInAnimation = false;

        return output;
    }
    public Vector3 GetAttackTargetPos(string attackName) {
        attackName = attackName.ToLower();
        Vector3 output = new Vector3(0, 0, 0);
        switch (attackName) {
            case "hook":
                output =
                    stanceHeadTran.position
                    + orientedTran.right * 3.75f
                    - transform.up * .25f;
                break;
            case "jab combo":
                output =
                    stanceHand2Tran.position
                    + orientedTran.right * 1.2f;
                break;
            case "fast jab":
                output =
                    stanceHand2Tran.position
                    + orientedTran.right * 2.1f;
                break;
            case "uppercut":
                output =
                    stanceHand1Tran.position
                    + orientedTran.up * 2f
                    + orientedTran.right * 1.5f;
                break;
            case "roundhousekick":
                output =
                    jointPelvis2Tran.position
                    + orientedTran.right * 4.5f;
                break;
            case "roundhousekickhigh":
                output =
                    stanceHeadTran.position
                    + orientedTran.right * 3.5f;
                break;
            case "pushkick": // dynamically changing thing
                break;
            case "knee": // dymanically changing
                break;
            case "chainpunch":
                output =
                    stanceHeadTran.position
                    + orientedTran.right * 3f
                    - transform.up * .25f;
                break;
            case "elbow":
                output =
                    stanceHeadTran.position
                    + orientedTran.right * 1.5f;
                break;
        }
        return output;
    }

    private void StrikeThisLocation(
        float damage,
        float pushMultiplier, // multiplies damage by this for force vector
        Vector3 toVector, // attack area spawn will be averaged with From
        Vector3 fromVector, // strike direction is determined by this and To
        GameObject strikingStanceObject, // the stance object of the limb
        GameObject limbObject, // the actual limb itself
        float length, float width // size of attack area 
        )
    // creates an AttackArea, be it enemy or friendly
    // if it is an enemy attack, has 1 second delay before damage
    // if it is a player attack, immediately does damage
    {
        if (!isGhost && !isPlayer) {
            return;
        }

        // just in case
        toVector = new Vector3(toVector.x, toVector.y, 0f);
        fromVector = new Vector3(fromVector.x, fromVector.y, 0f);

        GameObject newAttack = Instantiate(
            AttackWarningPrefab,
            (toVector + fromVector) / 2f,
            transform.rotation);


        newAttack.transform.parent = transform.parent; // so that it moves with parent but not with itself 


        AttackAreaScript newAttackScript = newAttack.GetComponent<AttackAreaScript>();
        newAttackScript.creator = this.gameObject;
        newAttackScript.guyHittingScript = this;
        newAttackScript.gameStateManagerScript = gameStateManagerScript;

        newAttackScript.thingHittingPos = limbObject.transform.position;

        // force vector
        newAttackScript.strikeForceVector = pushMultiplier * ((float)damage) * Vector3.Normalize(toVector - fromVector);

        //particle effect stuff
        newAttackScript.particleEffectController = particleEffectController;
        newAttackScript.particleControllerScript = particleControllerScript;

        // getting the rotation of the area
        newAttack.transform.right = toVector - fromVector;

        newAttack.transform.localScale = new Vector3(width, length, 1f);

        //set damage
        newAttackScript.attackDamage = damage;

        if (isGhost) // is the ghost of an enemy, creates visble attack area and warning, invokes creation of visible warning after framesUntilStrike frames
        {
            newAttackScript.creatorType = "enemy";
            newAttackScript.lifespan = attackDelayIfEnemy;
        }

        if (isPlayer) { // is the player, creates invisible attack area that checks for collision with enemies
            newAttackScript.lifespan = 0;
            newAttackScript.creatorType = "player";
            newAttackScript.UpdateSprites();
        }
        //Debug.Break();
    }
    private void DirectlyDamage(float damage, FighterScript targetFighterScript, string damageType, Vector3 strikeForceVector) {
        // use 1 attackAreaScript to attack all enemies
        GameObject directDamager = Instantiate(
            AttackWarningPrefab,
            transform.position + Vector3.up * 100f,
            transform.rotation
        );

        AttackAreaScript damagerScript = directDamager.GetComponent<AttackAreaScript>();

        damagerScript.guyHittingScript = this;
        damagerScript.attackDamage = damage;

        damagerScript.DirectlyDamage(
            targetFighterScript,
            damage,
            damageType,
            strikeForceVector
            );

        Destroy(directDamager);
    }
    IEnumerator KeepHandsInPlace() {
        Vector3 fromHeadHand1 = stanceHeadTran.position - stanceHand1Tran.position;
        Vector3 fromHeadHand2 = stanceHeadTran.position - stanceHand2Tran.position;

        while (true) {
            Vector3 currentHand1 = stanceHeadTran.position - fromHeadHand1;
            Vector3 currentHand2 = stanceHeadTran.position - fromHeadHand2;
            lowerArm1Tran.position = currentHand1;
            lowerArm2Tran.position = currentHand2;
            stanceHand1Tran.position = currentHand1;
            stanceHand2Tran.position = currentHand2;
            yield return null;
        }
    }

    IEnumerator GetStunned(float seconds) {
        if (!notInAnimation) {
            yield break;
        }

        notInAnimation = false;
        for (int i = 0; i < (int)(seconds * FPS); i++) {
            yield return null;
        }
        yield return null;
    }
    private void SuccessfulAttackStart(string attackID) {
        if (myFightingStyle == "wingchun") {
            Debug.Log("wingchun attack");
            StartCoroutine(MakeInvulnerableForSeconds(0.15f));
        }
    }
    IEnumerator MakeInvulnerableForSeconds(float seconds) {
        if (isInvulnerable) {
            yield break;
        }
        isInvulnerable = true;
        for (int i = 0; i < (int)(FPS * seconds); i++) {
            isInvulnerable = true;
            yield return null;
        }
        isInvulnerable = false;
    }
    public IEnumerator InvulnerableColoring() {
        Color origColor = torsoOutlineRenderer.startColor;
        Color invulColor = new Color((origColor.r + 1f) / 2f, (origColor.g + 1f) / 2f, (origColor.b + 1f) / 2f, 1);
        SetColor(invulColor);
        while (isInvulnerable) {
            yield return null;
        }
        SetColor(origColor);
        InvulnerabilityCoroutine = null;
    }
    IEnumerator Hook(bool isPartOfCombo)// true input  means no energy cost 
    {
        float[] info = GetAttackInfo("hook");
        float damage = info[0];
        float pushMultiplier = info[1];
        float energyCost = info[2];
        if (isPartOfCombo) {
            energyCost = 0f;
        }
        float timeTaken = info[3]; //seconds

        if (currentEnergy < energyCost) {
            notInAnimation = true;
            yield break;
        }
        else {
            ChangeEnergy(0f - energyCost);
        }

        int framesTaken = (int)(timeTaken * 60);
        int windUpFrames = (int)((float)framesTaken * .3f);
        int throwPunchFrames = (int)((float)framesTaken * .7f);
        Vector3 attackTarget = GetAttackTargetPos("Hook");

        yield return attackTarget;

        // wind up animation
        Vector3 handWindUpTarget = stanceHand1Tran.position - orientedTran.right * 0.5f;
        Vector3 handStartPos = stanceHand1Tran.position;
        for (int i = 0; i < windUpFrames; i++) {
            stanceHeadTran.position += orientedTran.right * moveSpeed;
            stanceHand1Tran.position = Vector3.Lerp(handStartPos, handWindUpTarget, ((float)i) / ((float)windUpFrames));
            yield return null;
        }


        // throw punch animation
        Vector3 throwStartPos = stanceHand1Tran.position;
        for (int i = 0; i < throwPunchFrames; i++) {
            stanceHeadTran.position += orientedTran.right * moveSpeed;

            // punching hand
            stanceHand1Tran.position = Vector3.Lerp(throwStartPos, attackTarget, (((float)i)) / ((float)throwPunchFrames));

            // guard hand
            Vector3 guardTargetPos = stanceHeadTran.position + orientedTran.right * 1f;
            stanceHand2Tran.LookAt(guardTargetPos);
            float distanceToGuardTarget = Vector3.Distance(stanceHand2Tran.position, guardTargetPos);
            stanceHand2Tran.position += stanceHand2Tran.forward * Mathf.Max(moveSpeed * distanceToGuardTarget * 2f, moveSpeed);
            upperArm1Tran.position = jointElbow1Tran.position;
            yield return null;
        }
        StrikeThisLocation(damage, pushMultiplier, attackTarget, jointElbow1Tran.position, stanceHand1, lowerArm1, 0.5f, 1.5f);

        // return to default stance fast before regaining control
        while (Vector3.Distance(stanceHand2Tran.position, hand2DefaultPos) > 0.1f) {
            MoveTowardsDefaultStance();
            MoveTowardsDefaultStance();
            yield return null;
        }
        notInAnimation = true;
    }
    IEnumerator JabCombo(string type)// type = "combo" or "fast". combo is a 2 hit combo, fast is a single far jab
        {
        string jabID = "";
        switch (type) {
            case "fast":
                jabID = "fast jab";
                break;
            case "combo":
                jabID = "jab combo";
                break;
        }
        float[] info = GetAttackInfo(jabID);
        float damage = info[0];
        float pushMultiplier = info[1];
        float energyCost = info[2];
        float timeTaken = info[3]; //seconds

        if (currentEnergy < energyCost) {
            notInAnimation = true;
            yield break;
        }
        else {
            ChangeEnergy(0f - energyCost);
        }

        Vector3 attackTarget = GetAttackTargetPos(jabID);
        int framesTaken = (int)(timeTaken * 60);
        Vector3 handStartPos = stanceHand2Tran.position;
        float hand2DistancePerFrame = Mathf.Abs(stanceHand2Tran.position.x - attackTarget.x) / framesTaken;

        float headDistancePerFrame = 0.3f * hand2DistancePerFrame;
        if (type == "fast") {
            headDistancePerFrame = 0.3f * hand2DistancePerFrame;
        }


        Vector3 headMoveVector = orientedTran.right * headDistancePerFrame;
        if (type == "combo")
            headMoveVector = -1f * orientedTran.right * headDistancePerFrame;

        // jab animation
        for (int i = 0; i < framesTaken; i++) {
            stanceHeadTran.position += headMoveVector;

            //moves jabbing hand 
            stanceHand2Tran.LookAt(attackTarget);
            stanceHand2Tran.position = Vector3.Lerp(handStartPos, attackTarget, (float)i / (float)framesTaken);

            if (type == "fast") {
                // moves nonjab hand to guard
                stanceHand1Tran.LookAt(stanceHeadTran.position + orientedTran.right * 1f);
                stanceHand1Tran.position += stanceHand1Tran.forward * hand2DistancePerFrame;
                upperArm1Tran.position = jointElbow1Tran.position;
            }
            yield return null;
        }
        StrikeThisLocation(damage, pushMultiplier, attackTarget, jointElbow2Tran.position, stanceHand2, lowerArm2, 0.5f, 1f);

        if (type == "fast") {
            // pull jabbing hand back
            while (Vector3.Distance(stanceHand2Tran.position, hand2DefaultPos) > 0.1f) {
                MoveTowardsDefaultStance();
                MoveTowardsDefaultStance();
                yield return null;
            }
        }

        // start hook if combo
        if (type == "combo") {
            yield return StartCoroutine(Hook(true));
            yield return null;
        }
        if (type == "fast") // end animation if fast
        {
            notInAnimation = true;
        }
        //Debug.Log("controls re-enabled");
    }
    IEnumerator Uppercut() {
        float[] info = GetAttackInfo("uppercut");
        float damage = info[0];
        float pushMultiplier = info[1];
        float energyCost = info[2];
        float timeTaken = info[3]; //seconds

        if (currentEnergy < energyCost) {
            notInAnimation = true;
            yield break;
        }
        else {
            notInAnimation = false;
            ChangeEnergy(0f - energyCost);
        }
        int framesTaken = (int)(timeTaken * FPS);
        int windUpFrames = (int)((float)framesTaken * .3f);
        int throwingFrames = (int)((float)framesTaken * .7f);

        Vector3 attackTarget = GetAttackTargetPos("Uppercut");

        yield return attackTarget;
        float distance = Vector3.Distance(attackTarget, stanceHand1Tran.position);

        float windUpDistancePerFrame = .5f / windUpFrames;
        float punchDistancePerFrame = distance / throwingFrames;

        // throw punch animation
        for (int i = 0; i < windUpFrames; i++) {
            stanceHeadTran.position += Vector3.up * windUpDistancePerFrame;
            stanceHand1Tran.position -= Vector3.up * windUpDistancePerFrame;// * 2f;
            yield return null;
        }

        for (int i = 0; i < throwingFrames; i++) {
            stanceHeadTran.position += orientedTran.up * punchDistancePerFrame * .4f;
            stanceHeadTran.position += orientedTran.right * punchDistancePerFrame * .25f;

            // punching hand
            stanceHand1Tran.LookAt(attackTarget);
            stanceHand1Tran.position += stanceHand1Tran.forward * punchDistancePerFrame;

            // guard hand
            stanceHand2Tran.LookAt(stanceHeadTran.position + orientedTran.right * .33f - orientedTran.up * 0.5f);
            stanceHand2Tran.position += stanceHand2Tran.forward * punchDistancePerFrame * 0.5f;
            upperArm1Tran.position = jointElbow1Tran.position;
            yield return null;
        }
        StrikeThisLocation(damage, pushMultiplier, attackTarget, jointElbow1Tran.position, stanceHand1, lowerArm1, 0.5f, 1.25f);

        // return to default stance fast before regaining control
        while (Vector3.Distance(stanceHand2Tran.position, hand2DefaultPos) > 0.1f) {
            MoveTowardsDefaultStance();
            MoveTowardsDefaultStance();
            yield return null;
        }
        notInAnimation = true;
    }
    IEnumerator RoundhouseKick(string type)// type = high or straight
    {
        float[] info = GetAttackInfo("roundhousekick");
        if (type == "high") {
            info = GetAttackInfo("roundhousekickhigh");
        }
        float damage = info[0];
        float pushMultiplier = info[1];
        float energyCost = info[2];
        float timeTaken = info[3]; //seconds

        if (currentEnergy < energyCost) {
            notInAnimation = true;
            yield break;
        }
        else {
            ChangeEnergy(0f - energyCost);

        }
        int framesTaken = (int)(timeTaken * FPS);

        Transform kickingFootTran = stanceFoot1Tran;

        //stanceTorsoBotActive = true;
        Vector3 origHeadPosition = stanceHeadTran.position;
        Vector3 origBotTorsoPosition = torsoBottom.transform.position;

        Vector3 attackTarget = GetAttackTargetPos("RoundhouseKick");
        if (type == "high") {
            drawNormalKnee1 = false;
            attackTarget = GetAttackTargetPos("RoundhouseKickHigh");

        }

        float footMovingDistance = Vector3.Distance(kickingFootTran.position, attackTarget);
        float distancePerFrame = footMovingDistance / framesTaken;

        // setting up balancing animation
        Vector3 torsoMoveVelocity = orientedTran.right * distancePerFrame * 0.3f;
        Vector3 headMoveVelocity = torsoMoveVelocity * 0.5f;
        Vector3 wholeBodyMoveVelocity = new Vector3(0f, 0f, 0f);
        if (type == "high") {
            headMoveVelocity = orientedTran.right * moveSpeed * -2f - orientedTran.up * moveSpeed * 0.5f;
            torsoMoveVelocity = new Vector3(0f, 0f, 0f);
            wholeBodyMoveVelocity = orientedTran.right * distancePerFrame * 0.3f;
        }

        // kick animation
        for (int i = 0; i < framesTaken; i++) {
            // balancing
            stanceHeadTran.position += headMoveVelocity;
            //stanceTorsoBotTran.position += torsoMoveVelocity;
            transform.position += wholeBodyMoveVelocity;
            stanceFoot2Tran.position -= wholeBodyMoveVelocity; // keeps foot in place

            kickingFootTran.LookAt(attackTarget);
            kickingFootTran.position += kickingFootTran.forward * distancePerFrame;

            // custom joint here, midway from foot to pelvis
            customKnee1Tran.position = (jointPelvis1Tran.position + kickingFootTran.position) / 2f;

            // guarding
            Vector3 hand1Guard = stanceHeadTran.position + orientedTran.right * 1f + orientedTran.up * 0f;
            float distanceHand1 = Vector3.Distance(hand1Guard, stanceHand1Tran.position);
            stanceHand1Tran.LookAt(hand1Guard);
            stanceHand1Tran.position += stanceHand1Tran.forward * Mathf.Max(moveSpeed * distanceHand1 * 2f, moveSpeed / 2f);

            // other hand goes the other direction
            if (type == "straight") {
                Vector3 hand2Guard = stanceHeadTran.position + orientedTran.right * 1.5f - orientedTran.up * 2f;
                float distanceHand2 = Vector3.Distance(hand2Guard, stanceHand2Tran.position);
                stanceHand2Tran.LookAt(hand2Guard);
                stanceHand2Tran.position += stanceHand2Tran.forward * Mathf.Max(moveSpeed * distanceHand2 * 2f, moveSpeed / 2f);
            }
            else {
                Vector3 hand2Guard = stanceHeadTran.position + orientedTran.right * -1f - orientedTran.up * 2f;
                float distanceHand2 = Vector3.Distance(hand2Guard, stanceHand2Tran.position);
                stanceHand2Tran.LookAt(hand2Guard);
                stanceHand2Tran.position += stanceHand2Tran.forward * Mathf.Max(moveSpeed * distanceHand2 * 2f, moveSpeed / 2f);
            }

            yield return null;
        }
        StrikeThisLocation(damage, pushMultiplier, attackTarget, customKnee1Tran.position, stanceFoot1, calf1, 0.75f, 2f);
        stanceTorsoBotActive = false;

        float returnFrames = (int)(22f / speedMultiplier);
        float returnSpeedMult = (float)framesTaken / returnFrames;
        wholeBodyMoveVelocity = -1f * wholeBodyMoveVelocity * returnSpeedMult;
        torsoMoveVelocity = -1f * torsoMoveVelocity * returnSpeedMult;
        headMoveVelocity = -1f * headMoveVelocity * returnSpeedMult;

        // setting up not moving foot

        for (int i = 0; i < returnFrames; i++) // tested number of frames on acolyte
        {
            // keeps guarding hand in place 
            Vector3 origHand1Pos = stanceHand1Tran.localPosition;

            // keeps front foot in place
            Vector3 origFootPos = stanceFoot2Tran.position;
            float distance = Vector3.Distance(stanceFoot2Tran.position, foot2DefaultPos);
            Vector3 predictedFootMovement = new Vector3(0f, 0f, 0f);

            stanceHeadTran.position += headMoveVelocity; // needs to be before moveTowardsDefaultStance because it determines defaultStance

            // this part is just the movetowradsdefaultstance movement + bodymovevelocity
            UpdateDefaultStancePositions();
            if (distance > 0.1f) {
                stanceFoot2Tran.LookAt(foot2DefaultPos);
                predictedFootMovement = stanceFoot2Tran.forward * Mathf.Max(moveSpeed * distance * 2f, moveSpeed) + wholeBodyMoveVelocity;
            }
            MoveTowardsDefaultStance();
            stanceFoot2Tran.position -= predictedFootMovement;
            stanceHand1Tran.localPosition -= wholeBodyMoveVelocity;

            // draw leg correctly
            customKnee1Tran.position = (jointPelvis1Tran.position + kickingFootTran.position) / 2f;

            // go back to place
            stanceHand2Tran.position += orientedTran.right * moveSpeed * 1f - orientedTran.up * moveSpeed * 2f;
            //stanceTorsoBotTran.position += torsoMoveVelocity;
            transform.position += wholeBodyMoveVelocity;
            MoveAndDrawBody();

            yield return null;
        }
        SetStances("combat");
        drawNormalKnee1 = true;
        notInAnimation = true;
        //Debug.Log("controls re-enabled");
    }
    IEnumerator PushKick(string type)// type = "grounded" or "flying" because I am lazy to make another ienumerator for a flying push kick 
    {
        float[] info = GetAttackInfo("pushkick");
        if (type == "flying") {
            info = GetAttackInfo("flyingkick");
            info[2] = 0; // sets energy cost to 0
        }

        float damage = info[0]; // to be set in below switch statement
        float pushMultiplier = info[1];
        float energyCost = info[2];
        float totalTimeTaken = info[3];
        if (type == "flying") {
            totalTimeTaken *= speedMultiplier;
        }

        bool grounded = true;

        if (currentEnergy < energyCost) {
            notInAnimation = true;
            yield break;
        }
        else {
            ChangeEnergy(0f - energyCost);
        }

        // range and animation setup
        float range = 0f;
        switch (type) {
            case "grounded":
                range = 5f;
                grounded = true;
                stanceTorsoBotActive = true;
                break;
            case "flying":
                range = 3.5f;
                grounded = false;
                SetStances("none");
                stanceFoot2Active = true;
                stanceHeadActive = true;
                stanceHand1Active = true;
                stanceHand2Active = true;
                break;
        }
        int raiseFootFrames = (int)(.4f * totalTimeTaken * FPS);
        int kickFrames = (int)(.6f * totalTimeTaken * FPS);


        Transform kickingFootTran = stanceFoot2Tran;

        stanceTorsoBotActive = true;

        Vector3 origHeadPosition = stanceHeadTran.localPosition;
        Vector3 origBotTorsoPosition = torsoBottom.transform.localPosition;
        Vector3 attackTarget = jointPelvis2Tran.localPosition + orientedTran.right * range;

        float raiseLegDistance = Mathf.Abs(kickingFootTran.localPosition.y - attackTarget.y);

        float kickDistance = Vector3.Distance(kickingFootTran.localPosition, attackTarget);
        float kickDistancePerFrame = kickDistance / kickFrames;

        // raise front leg
        float kickHeightMod = 0f;
        if (type == "grounded") {
            kickHeightMod = 0.5f;
        }
        else {
            kickHeightMod = 0f;
        }
        Vector3 origKickFootLocalPos = kickingFootTran.localPosition;
        Vector3 raisedKickLocalPos = stanceTorsoBotTran.localPosition + transform.right * 1f + transform.up * kickHeightMod;

        for (int i = 0; i < raiseFootFrames; i++) {
            //kickingFootTran.position += orientedTran.up * kickDistancePerFrame;
            kickingFootTran.localPosition = Vector3.Lerp(origKickFootLocalPos, raisedKickLocalPos, ((float)i / (float)raiseFootFrames));
            yield return null;
        }

        // target vector created for real here
        if (type == "grounded") {
            kickHeightMod = 0.33f;
        }
        else {
            kickHeightMod = -.25f;
        }
        attackTarget = stanceTorsoBotTran.localPosition + transform.right * range + transform.up * kickHeightMod;

        Vector3 footStartPos = kickingFootTran.localPosition;

        Vector3 torsoMoveDistance = transform.right * kickDistancePerFrame * 0.3f;

        // extend and kick
        for (int i = 0; i < kickFrames; i++) {

            // lean back
            if (grounded) {
                stanceTorsoTopActive = true;
                stanceHeadTran.localPosition += torsoMoveDistance * .75f;
                stanceTorsoTopTran.localPosition += torsoMoveDistance * .75f;
                stanceTorsoBotTran.localPosition += torsoMoveDistance;
            }
            else {
                stanceHeadTran.localPosition += new Vector3(
                    -2f * moveSpeed,
                    0f,
                    0f
                );
                stanceTorsoBotTran.localPosition += new Vector3(
                    -0.5f * moveSpeed,
                    0f,
                    0f
                );
            }

            // kicking foot movement
            kickingFootTran.localPosition = Vector3.Lerp(footStartPos, attackTarget, ((float)i) / ((float)kickFrames));

            // swinging hand down
            Vector3 hand1Guard = stanceHeadTran.position - orientedTran.up * 1.5f + orientedTran.right * .25f;

            // guarding hand up
            Vector3 hand2Guard = stanceHeadTran.position + orientedTran.right * 1f;

            float hand1distance = Vector3.Distance(hand1Guard, stanceHand1Tran.position);
            stanceHand1Tran.LookAt(hand1Guard);
            stanceHand1Tran.position += stanceHand1Tran.forward * Mathf.Max(moveSpeed * hand1distance, moveSpeed);

            float distance = Vector3.Distance(hand2Guard, stanceHand2Tran.position);
            stanceHand2Tran.LookAt(hand2Guard);
            stanceHand2Tran.position += stanceHand2Tran.forward * Mathf.Max(moveSpeed * distance, moveSpeed);

            yield return null;
        }

        // resets turning on the torso stances
        SetStances("combat");
        StrikeThisLocation(damage, pushMultiplier, kickingFootTran.position, (kickingFootTran.position + jointKnee2Tran.position) / 2f, stanceFoot2, calf2, 0.75f, 1f);

        while (Vector3.Distance(kickingFootTran.position, foot2DefaultPos) > .25f) {
            MoveTowardsDefaultStance();
            yield return null;
        }
        notInAnimation = true;
        //Debug.Log("controls re-enabled");
    }
    IEnumerator FlyingKickPart2(float jumpVectorY) {
        IEnumerator handStanceCoroutine = KeepHandsInPlace();
        StartCoroutine(handStanceCoroutine);
        isAirborne = true;
        while (stanceHeadTran.position.y <= transform.position.y + reach) {
            stanceHeadTran.position += orientedTran.up * jumpVectorY / FPS;
            //stanceFoot2Tran.position += orientedTran.up * jumpSpeed / FPS;
            yield return null;
        }

        //Debug.Log("jumped into air");
        fighterRB.velocity = new Vector2(5f * transform.localScale.x, jumpVectorY);
        fighterRB.gravityScale = 3f;
        stanceFoot1Tran.position += Vector3.up * 0.05f;
        stanceFoot2Tran.position += Vector3.up * 0.05f;
        StopCoroutine(handStanceCoroutine);

        // flying front kick
        StartCoroutine(PushKick("flying"));

        float y = transform.position.y;
        while (y >= transform.position.y) {
            yield return null;
        }
        // raise back leg
        Vector3 stanceFoot1Target = jointPelvis1Tran.position + orientedTran.right - orientedTran.up * 0.5f;
        float timeTaken = 15f;
        for (int i = 0; i < 15; i++) {
            stanceFoot1Tran.position = Vector3.Lerp(stanceFoot1Tran.position, stanceFoot1Target, ((float)(i)) / timeTaken);
            yield return null;
        }

        while (stanceFoot1Tran.position.y > groundLevel
            && stanceFoot2Tran.position.y > groundLevel) {
            yield return null;
        }

        fighterRB.velocity = new Vector2(0, 0);
        fighterRB.gravityScale = 0f;
        transform.position = new Vector3(
            transform.position.x,
            0,
            0
        );

        //Debug.Log("landed");
        for (int i = 0; i < 10; i++) {
            stanceHeadTran.position -= Vector3.up * 0.5f / 10f;
            yield return null;
        }
        isAirborne = false;
    }
    IEnumerator FlyingKick() {
        float energyCost = GetAttackInfo("flyingkick")[2];
        // no power listed here, rather it is listed on the front kick
        if (currentEnergy < energyCost) {
            notInAnimation = true;
            yield break;
        }
        else {
            ChangeEnergy(0f - energyCost);
        }

        int sector = GetHeadSector();

        // move head to top forward section
        Vector3 headTargetPosition = orientedTran.position + orientedTran.right + orientedTran.up;
        Vector3 initHeadPosition = fighterHead.transform.position;
        float headMoveTime = 20f / speedMultiplier;
        IEnumerator keepHandsInPlace = KeepHandsInPlace();
        StartCoroutine(keepHandsInPlace);
        for (int i = 0; i < (int)headMoveTime; i++) {
            stanceHeadTran.position = Vector3.Lerp(initHeadPosition, headTargetPosition, ((float)(i)) / headMoveTime);
            yield return null;
        }
        StopCoroutine(keepHandsInPlace);

        // jump and kick at the same time
        yield return StartCoroutine(FlyingKickPart2(10f));
        yield return null;
    }
    IEnumerator GroundSlam() {

        float[] info = GetAttackInfo("groundslam");
        float damage = info[0];
        float pushMultiplier = info[1];
        float energyCost = info[2];
        float totalAnimationTimeTaken = info[3];

        if (currentEnergy < energyCost) {
            notInAnimation = true;
            yield break;
        }
        else {
            ChangeEnergy(0f - energyCost);
            UpdateEnergyBar();
            gainEnergyOn = false;
        }

        float timeTakenWindUp = 0.33f * totalAnimationTimeTaken;
        float timeTakenJump = 0.66f * totalAnimationTimeTaken;
        int framesTakenWindUp = (int)(timeTakenWindUp * FPS);
        int framesTakenJump = (int)(timeTakenJump * FPS);

        IEnumerator keepHandsInPlace = KeepHandsInPlace();
        StartCoroutine(keepHandsInPlace);

        // wind up animation
        Vector3 initHeadPos = stanceHeadTran.position;
        float xTarget = GetSectorPosition(2).x;
        Vector3 headTargetPos = new Vector3(
            xTarget,
            -1f,
            stanceHeadTran.position.z
            );

        for (int i = 0; i < framesTakenWindUp; i++) {
            stanceHeadTran.position = Vector3.Lerp(initHeadPos, headTargetPos, ((float)(i)) / framesTakenWindUp);
            yield return null;
        }

        // jump animation

        initHeadPos = stanceHeadTran.position;
        headTargetPos = new Vector3(
            stanceHeadTran.position.x,
            2f,
            stanceHeadTran.position.z
            );

        for (int i = 0; i < framesTakenJump; i++) {
            stanceHeadTran.position = Vector3.Lerp(initHeadPos, headTargetPos, ((float)(i)) / framesTakenJump);
            yield return null;
        }

        // jump 
        float jumpSpeed = 20f;
        fighterRB.velocity = new Vector2(0f, jumpSpeed);
        fighterRB.gravityScale = 3f;
        stanceFoot1Tran.position += Vector3.up * 0.1f;
        stanceFoot2Tran.position += Vector3.up * 0.1f;

        while (stanceFoot1Tran.position.y > groundLevel
            && stanceFoot2Tran.position.y > groundLevel) {
            MoveHeadTowardsSector(1);
            yield return null;
        }
        StopCoroutine(keepHandsInPlace);

        fighterRB.velocity = new Vector2(0, 0);
        fighterRB.gravityScale = 0f;
        transform.position = new Vector3(
            transform.position.x,
            0,
            0
        );

        //particle effects object
        particleEffectController.transform.position =
            stanceFoot1Tran.position
            - (stanceFoot1Tran.position - stanceFoot2Tran.position) / 2f;

        particleControllerScript.PlayEffect("groundslam");


        // damage all enemies
        List<GameObject> allEnemiesRef = gameStateManagerScript.enemyManagerScript.GetAllEnemiesList(); // removes null enemies before returning the list
        GameObject[] allEnemiesArray = new GameObject[allEnemiesRef.Count];
        for (int i = 0; i < allEnemiesRef.Count; i++) {
            allEnemiesArray[i] = allEnemiesRef[i];
        }

        Debug.Log("slam hit " + allEnemiesRef.Count + " enemies");

        // use 1 attackAreaScript to attack all enemies
        GameObject directDamager = Instantiate(
            AttackWarningPrefab,
            transform.position + Vector3.up * 100f,
            transform.rotation
        );
        AttackAreaScript damagerScript = directDamager.GetComponent<AttackAreaScript>();

        // run through the list to damage them
        for (int i = 0; i < allEnemiesArray.Length; i++) {
            FighterScript targetFS = allEnemiesArray[i].GetComponent<EnemyWithGhostScript>().enemyFighterScript;

            Vector3 strikeForceDirection = (
                Vector3.Normalize(targetFS.gameObject.transform.position - transform.position) + Vector3.up
            );

            Vector3 strikeForceVector = strikeForceDirection * damage * pushMultiplier;

            DirectlyDamage(damage, targetFS, "physical", strikeForceVector);

        }

        Destroy(directDamager);

        gainEnergyOn = true;
        notInAnimation = true;
    }
    IEnumerator Elbow() {
        float[] info = GetAttackInfo("elbow");
        float damage = info[0];
        float pushMultiplier = info[1];
        float energyCost = info[2];
        float timeTaken = info[3]; //seconds

        if (currentEnergy < energyCost) {
            notInAnimation = true;
            yield break;
        }
        else {
            ChangeEnergy(0f - energyCost);
        }

        int framesTaken = (int)(timeTaken * 60);
        Vector3 attackTarget = GetAttackTargetPos("elbow");

        // elbow animation
        Vector3 H2StartPos = stanceHand1Tran.position;
        Vector3 H2TargetPosFromHead = orientedTran.right * 0.5f + Vector3.up * 0.5f;
        for (int i = 0; i < framesTaken; i++) {
            stanceHeadTran.position += orientedTran.up * moveSpeed / 2f;

            // elbow hand
            stanceHand2Tran.position = Vector3.Lerp(
                H2StartPos,
                stanceHeadTran.position + H2TargetPosFromHead,
                (((float)i)) / ((float)framesTaken));
            lowerArm2Tran.eulerAngles = new Vector3(0f, 0f, 0f);

            // guard hand
            Vector3 guardTargetPos = stanceHeadTran.position + orientedTran.right * 1f;
            stanceHand2Tran.LookAt(guardTargetPos);
            float distanceToGuardTarget = Vector3.Distance(stanceHand2Tran.position, guardTargetPos);
            stanceHand2Tran.position += stanceHand2Tran.forward * Mathf.Max(moveSpeed * distanceToGuardTarget * 2f, moveSpeed);
            upperArm2Tran.position = jointElbow1Tran.position;
            yield return null;
        }
        stanceHand2Tran.position = stanceHeadTran.position + H2TargetPosFromHead;
        StrikeThisLocation(damage, pushMultiplier, jointElbow2Tran.position, stanceHand2Tran.position + (jointElbow2Tran.position - stanceHand2Tran.position), jointElbow2, lowerArm2, 0.5f, 1.5f);

        // return to default stance fast before regaining control
        while (Vector3.Distance(stanceHand2Tran.position, hand2DefaultPos) > 0.1f) {
            MoveTowardsDefaultStance();
            MoveTowardsDefaultStance();
            yield return null;
        }
        notInAnimation = true;
    }
    IEnumerator Knee(string type)// grounded or flying
        {
        float[] info = GetAttackInfo("knee");
        float damage = info[0];
        float pushMultiplier = info[1];
        float energyCost = info[2];
        float timeTaken = info[3]; //seconds
        if (currentEnergy < energyCost) {
            notInAnimation = true;
            yield break;
        }
        else {
            ChangeEnergy(0f - energyCost);
        }

        int framesTaken = (int)(timeTaken * FPS);

        // move head
        Vector3 localHeadFrom = stanceHeadTran.localPosition;
        Vector3 localHeadTo = new Vector3(
            stanceHeadTran.localPosition.x - 1.5f,
            stanceHeadTran.localPosition.y + ((transform.position.y + reach) - stanceHeadTran.position.y), // distance to top sector
            0f
        );

        // move body with knee
        float bodyMove = 2f;
        if (type == "flying") {
        }
        Vector3 wholeBodyStart = transform.position;
        Vector3 wholeBodyEnd = transform.position + orientedTran.right * bodyMove;
        Vector3 foot2Start = stanceFoot2Tran.localPosition;
        Vector3 foot2End = stanceFoot2Tran.localPosition - transform.right * bodyMove;

        Vector3 footFrom = stanceFoot1Tran.localPosition;
        // knee animation
        for (int i = 0; i < framesTaken; i++) {
            Vector3 footTo = stanceHeadTran.localPosition
                + transform.right * 1f
                + transform.up * -2f; ;
            float lerpAmount = ((float)i) / ((float)framesTaken);
            stanceFoot1Tran.localPosition = Vector3.Lerp(footFrom, footTo, lerpAmount);
            transform.position = Vector3.Lerp(wholeBodyStart, wholeBodyEnd, lerpAmount);
            stanceFoot2Tran.localPosition = Vector3.Lerp(foot2Start, foot2End, lerpAmount);
            stanceHeadTran.localPosition = Vector3.Lerp(localHeadFrom, localHeadTo, lerpAmount);

            // hands
            Vector3 hand1TargetPos = jointRib1Tran.position;
            stanceHand1Tran.LookAt(hand1TargetPos);
            stanceHand1Tran.position += stanceHand1Tran.forward * moveSpeed * 2f;
            Vector3 hand2TargetPos = jointRib2Tran.position;
            stanceHand2Tran.LookAt(hand2TargetPos);
            stanceHand2Tran.position += stanceHand2Tran.forward * moveSpeed * 2f;

            yield return null;
        }
        StrikeThisLocation(damage, pushMultiplier, jointKnee1Tran.position, stanceFoot1Tran.position, stanceFoot1, calf1, 0.75f, 1.25f);

        while (Vector3.Distance(stanceFoot1Tran.position, foot1DefaultPos) > .25f) {
            MoveTowardsDefaultStance();
            yield return null;
        }
        notInAnimation = true;
    }
    IEnumerator ChainPunch() {
        float[] info = GetAttackInfo("chainpunch");
        float damage = info[0];
        float pushMultiplier = info[1];
        float energyCost = info[2];
        float timeTaken = info[3]; //seconds

        if (currentEnergy < energyCost) {
            notInAnimation = true;
            yield break;
        }
        else {
            ChangeEnergy(0f - energyCost);
        }

        int framesTaken = (int)(timeTaken * 60);
        int windUpFrames = (int)((float)framesTaken * .3f);
        int comboFramesTotal = (int)((float)framesTaken * .7f);
        Vector3 attackTarget = GetAttackTargetPos("chainpunch");


        // throw punch1, draw hand2 back
        Vector3 throwStartPos = stanceHand1Tran.position;

        Vector3 hand2InitPos = stanceHand2Tran.position;
        Vector3 drawBackPos = stanceHeadTran.position + Vector3.right * 0.5f - Vector3.up * 1f;

        float lerpPercent = 0f;
        float framesPerPunch = comboFramesTotal / 2f;
        for (int i = 0; i < comboFramesTotal / 2f; i++) {
            lerpPercent = (((float)i)) / framesPerPunch;
            // punching hand
            stanceHand1Tran.position = Vector3.Lerp(throwStartPos, attackTarget, lerpPercent);

            // draw hand2 back to prepare the punch
            stanceHand2Tran.position = Vector3.Lerp(hand2InitPos, drawBackPos, lerpPercent);

            upperArm1Tran.position = jointElbow1Tran.position;
            yield return null;
        }

        StrikeThisLocation(damage, pushMultiplier, attackTarget, jointElbow1Tran.position, stanceHand1, lowerArm1, 0.5f, .75f);

        // throw punch2, draw hand1 back
        Vector3 hand1InitPos = stanceHand1Tran.position;
        lerpPercent = 0f;
        for (int i = 0; i < comboFramesTotal / 2f; i++) {
            lerpPercent = (((float)i)) / framesPerPunch;
            // punching hand
            stanceHand2Tran.position = Vector3.Lerp(drawBackPos, attackTarget, lerpPercent);

            // draw hand1 back to prepare the punch
            stanceHand1Tran.position = Vector3.Lerp(hand1InitPos, hand1DefaultPos, lerpPercent);

            upperArm2Tran.position = jointElbow2Tran.position;
            yield return null;
        }
        StrikeThisLocation(damage, pushMultiplier, attackTarget, jointElbow2Tran.position, stanceHand2, lowerArm2, 0.5f, .75f);

        // return to default stance fast before regaining control
        while (Vector3.Distance(stanceHand2Tran.position, hand2DefaultPos) > 0.1f) {
            MoveTowardsDefaultStance();
            MoveTowardsDefaultStance();
            MoveTowardsDefaultStance();
            yield return null;
        }
        notInAnimation = true;
    }
}
