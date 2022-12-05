using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// to do
/*

-sorting layer stuff

*/

public class FighterScript : MonoBehaviour
{

    public GameObject damageNumberPrefab;
    // perks
    public int vampirismLevel = 0;
    public int lightningLevel = 0;
    public int explosiveLevel = 0;
    public int poisonerLevel = 0; // creates isPoisonedEffect
    public int isPoisonedEffect = 0;
    public int pressurePointLevel = 0; // creates isWeakenedEffect
    public int isWeakenedEffect = 0;

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

    public bool airborne = false;
    public Rigidbody2D fighterRB;
    public float groundLevel;
    public GameObject fighterOrienter;
    public Transform orientedTran; // .right is forward no matter what direction the fighter is currently looking in

    public string fighterType;

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

    public bool notInAttackAnimation = true; // enabled, fighter can move head around + limbs move to default positions. disable when in an animation. moves slower when false
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
    public void ChangeMultiplier(string whichMultiplier, string operationArg, float amount) // so long bc I cant create a reference to a float
    {
        //Debug.Log("changing a multiplier");
        float oldMultiplier = -0f;
        float newMultiplier = -0f;

        bool targetMultiplierFound = false;
        switch (whichMultiplier)
        {
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

        if (!targetMultiplierFound)
        {
            Debug.Log("INVALID MULTIPLIER NAME, OPERATION CANCELLED"); return;
        }

        bool operationSuccess = false;
        switch (operationArg)
        {
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

        if (!operationSuccess)
        {
            Debug.Log("INVALID OPERATION NAME, OPERATION CANCELLED");
            return;
        }


        switch (whichMultiplier)
        {
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
    public void InitDefaultMoveset()
    {
        legMoveset[0] = "flyingkick";
        legMoveset[1] = "flyingkick";
        legMoveset[2] = "knee";
        legMoveset[3] = "pushkick";
        legMoveset[4] = "roundhousekick";
        legMoveset[5] = "roundhousekick";
        legMoveset[6] = "pushkick";
        legMoveset[7] = "roundhousekick";
        legMoveset[8] = "roundhousekickhigh";

        armMoveset[0] = "uppercut";
        armMoveset[1] = "uppercut";
        armMoveset[2] = "uppercut";
        armMoveset[3] = "hook";
        armMoveset[4] = "hook";
        armMoveset[5] = "jabaggressive";
        armMoveset[6] = "hook";
        armMoveset[7] = "hook";
        armMoveset[8] = "jabcombo";
    }
    public void SetTags(string tag) // sets the tags for the collisions
    {
        fighterHead.tag = tag;
        headLimb.tag = tag;
        torsoTop.tag = tag;
        torsoBottom.tag = tag;
    }
    void InitHealthAndEnergyBar()
    {
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

        if (isGhost)
        {
            Destroy(healthBar);
            Destroy(healthBarBackground);
            Destroy(energyBar);
            Destroy(energyBarBackground);
        }
    }
    void InitGameObjects()
    {
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
    void InitTrailRenderers()
    {/*
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
    void HideJointsAndStances()
    {
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
    void InitHinges()
    {
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

        foreach (HingeJoint2D hingeJoint in allHinges)
        {
            hingeJoint.useMotor = false;
        }
    }
    void InitTransforms()
    {
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
    void InitLineRenderers()
    {
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

        foreach (LineRenderer renderer in allLineRenderers)
        {
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
    void InitRigidbody2DListAndProperties()
    {
        fighterRB = this.gameObject.GetComponent<Rigidbody2D>();
        int rbCount = 0;
        foreach (GameObject bodyPart in allBodyParts)
        {
            Rigidbody2D bodyPartRB = bodyPart.GetComponent<Rigidbody2D>();
            if (bodyPartRB != null)
            {
                allRigidbody2D.Add(bodyPartRB);
                rbCount++;
            }
        }
        //Debug.Log(rbCount + " RigidBodies added to list");
        InitRigidBody2DProperties();
    }
    void InitRigidBody2DProperties()
    {
        foreach (Rigidbody2D rb2d in allRigidbody2D)
        {
            rb2d.drag = .5f;
        }
    }
    void InitColliderList()
    {
        allPolyCollider2D.Add(headLimb.GetComponent<PolygonCollider2D>());
        int bcCount = 1;
        foreach (GameObject bodyPart in allBodyParts)
        {
            PolygonCollider2D bodyPartBC = bodyPart.GetComponent<PolygonCollider2D>();
            if (bodyPartBC != null)
            {
                allPolyCollider2D.Add(bodyPartBC);
                bcCount++;
            }
        }
        //Debug.Log(bcCount + " BoxCollider2D added to list");
    }
    void SetColliders(string type) // turns on head and torso, turns off others
    {
        switch (type)
        {
            case "combat": // purely to check hitboxes for attacks
                foreach (PolygonCollider2D pc2d in allPolyCollider2D)
                {
                    pc2d.enabled = false;
                    pc2d.isTrigger = true;
                }
                headLimb.GetComponent<PolygonCollider2D>().enabled = true;
                torsoTop.GetComponent<PolygonCollider2D>().enabled = true;
                torsoBottom.GetComponent<PolygonCollider2D>().enabled = true;

                // every collider is now disabled

                if (isGhost)
                {
                    headLimb.GetComponent<PolygonCollider2D>().enabled = false;
                    torsoTop.GetComponent<PolygonCollider2D>().enabled = false;
                    torsoBottom.GetComponent<PolygonCollider2D>().enabled = false;
                }
                break;
            case "ragdoll": // interact with physics system
                foreach (PolygonCollider2D pc2d in allPolyCollider2D)
                {
                    pc2d.enabled = true;
                    pc2d.isTrigger = false;
                }
                break;
        }
    }
    public void SetColor(Color newColorArg)
    {
        Color newColor = newColorArg;
        Color newTorsoColor = new Color((newColor.r + 1f) / 2f, (newColor.g + 1f) / 2f, (newColor.b + 1f) / 2f, 1);

        if (isGhost)
        {
            newColor = new Color(newColor.r, newColor.g, newColor.b, 0.1f);
            newTorsoColor = new Color((newColor.r + 1f) / 2f, (newColor.g + 1f) / 2f, (newColor.b + 1f) / 2f, 0.1f);
        }

        foreach (LineRenderer renderer in allLineRenderers)
        {
            renderer.startColor = newColor;
            renderer.endColor = newColor;
        }

        torsoRenderer.startColor = newTorsoColor;
        torsoRenderer.endColor = newTorsoColor;

        headSprite.color = newColor;
    }
    public void SetRenderSortingLayer(int layer)
    {
        foreach (LineRenderer renderer in allLineRenderers)
        {
            renderer.sortingOrder = layer;
        }
        torsoRenderer.sortingOrder = layer - 1;
    }

    // Start is called before the first frame update
    void Awake()
    {
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

        InitDefaultMoveset();

        // defaults to enemy tags
        fighterHead.tag = "Enemy";
        torsoTop.tag = "Enemy";
        torsoBottom.tag = "Enemy";

        notInAttackAnimation = true;
        facingRight = true;
        isTurning = false;

        // default stat values 
        defaultMaxHP = 100f;
        defaultMaxEnergy = 100f;
        gainEnergyOn = true;
        defaultEnergyPerSecond = 20f;
        defaultMoveSpeed = 4f / 60f; // x units per 60 frames

        armPower = 1f;
        armPowerDefault = 1f;
        legPowerDefault = 1f;
        legPowerDefault = 1f;

        speedMultiplierDefault = 1f;

        moveSpeed = defaultMoveSpeed * speedMultiplier;
        reach = .75f;

        if (headLimb.transform.position.z != 0f)
        {
            Debug.Log("head spawned in wrong z value");
        }
    }
    void Start()
    {
        InitHealthAndEnergyBar(); //THIS NEEDS TO BE IN START, NOT AWAKE
        UpdateDefaultStancePositions(); // need this to set up groundLevel
        groundLevel = Mathf.Min(foot1DefaultPos.y, foot2DefaultPos.y); // keep this in Start()

        particleControllerScript = particleEffectController.GetComponent<ParticleEffectsController>();
        parentObject = transform.parent.gameObject;
        parentRB = parentObject.GetComponent<Rigidbody2D>();
        StartCoroutine(FindAndFixBrokenArmHinges(10f));
    }
    void Update()
    {
        KeepZPositionAtZero();
        GainEnergy();
        MoveAndDrawBody(); // for some reason important this is called first
        if (notInAttackAnimation) // if the fighter is not currently in an animation
        {
            MoveTowardsDefaultStance();
        }
        StatusEffects();
    }
    void StatusEffects() // called every update
    {
        // status effects

        if (isPoisonedEffect > 0)
        {
            // 3 dps + poisonEffect * 3
            float poisonDamage = (3f + (float)isPoisonedEffect * 3f)/60f;
            hp -= poisonDamage;
            if (hp < 0)
            {
                hp = 1f;
            }
            UpdateHealthBar();
        }
    }
    void KeepZPositionAtZero()
    {
        foreach (GameObject bodyPart in allBodyParts)
        {
            bodyPart.transform.position = new Vector3(
                bodyPart.transform.position.x,
                bodyPart.transform.position.y,
                0f
            );
        }
        foreach (GameObject stance in allStances)
        {
            stance.transform.position = new Vector3(
                stance.transform.position.x,
                stance.transform.position.y,
                0f
            );
        }
        if (transform.position.z != 0f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        }
    }

    IEnumerator FindAndFixBrokenArmHinges(float degreesOfLenience) // this code is disgusting but I am sofucking tired of the arms getting fucked
    {
        float lenience = degreesOfLenience;

        while (true)
        {
            if (Time.frameCount % 10 == 0 && notInAttackAnimation)
            {
                HingeJoint2D hingeJointRef = lowerArm2Hinge;
                Transform jointObjectTrans = hingeJointRef.gameObject.transform;

                //Debug.Log("arm " + 2 + ": " + hingeJointRef.jointAngle + " in limits " + hingeJointRef.limits.min + "/" + hingeJointRef.limits.max);

                if (hingeJointRef.jointAngle < hingeJointRef.limits.min || hingeJointRef.jointAngle > hingeJointRef.limits.max)
                {
                    if (hingeJointRef.jointAngle < hingeJointRef.limits.min)
                    {
                        //Debug.Log("arm 2 lower than min");
                    }
                    else
                    {
                        //Debug.Log("arm 2 higher than max");
                    }
                    // for some reason needs -1f * orientedTran.localScale.x idk why
                    if (!lowerArm1Hinge.useMotor)
                    {
                        StartCoroutine(UnfuckArm2());
                    }
                    //jointObjectTrans.right = -1f * orientedTran.localScale.x * (jointObjectTrans.position - jointKnee1Tran.position);
                }
            }

            yield return null;
        }
    }

    IEnumerator UnfuckArm2() // just try not to fuck it up to begin with bc this looks weird
    {
        lowerArm1Hinge.useMotor = true;
        while (lowerArm2Hinge.jointAngle < lowerArm2Hinge.limits.min || lowerArm2Hinge.jointAngle > lowerArm2Hinge.limits.max)
        {
            lowerArm2Tran.eulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));
            yield return null;
        }

        lowerArm1Hinge.useMotor = false;
    }
    public void SetCharacterType(string typeArg) // do this as soon as the fighter is created and InitBasedOnCharSettings does the rest
    {
        fighterType = typeArg.ToLower();
    }

    public IEnumerator InitBasedOnCharSettings() // waits a few frames then updates info
    {
        // Debug.Log("starting delay before setting character settings");
        for (int i = 0; i < 2; i++)
        {
            yield return null;
        }
        string tag = "";

        if (isPlayer)
        {
            tag = "Player";
        }

        // changing enemy colors
        if (!isPlayer)
        {
            if (isGhost)
            {
                tag = "Ghost";
                SetRenderSortingLayer(0);
            }
            else
            {
                tag = "Enemy";
                SetRenderSortingLayer(1);
                SetStances("all");
            }

            switch (fighterType)
            {
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

        if (isPlayer)
        {
            // Debug.Log("UpdateBasedOnCharSettings sitrep: hp: " + hp + "/" + maxhp + ", speedMulti,powerMulti = " + speedMultiplier + ", " + damageMultiplier);
        }
        InitClassCombatStats();
    }
    public void InitClassCombatStats()
    {
        // first sets default values
        speedMultiplier = 1f;
        legPower = 1f;
        maxhp = 100f;
        hp = 100f;

        // set number stats
        switch (fighterType)
        {
            case "acolyte":
                // Debug.Log("setting acolyte");
                defaultMaxHP = 100f;
                defaultMaxEnergy = 100f;
                defaultEnergyPerSecond = 20f;

                speedMultiplierDefault = 1f;

                armPowerDefault = 1f;
                legPowerDefault = 1f;
                break;
            case "brawler":
                // Debug.Log("setting brawler");

                defaultMaxHP = 160f;
                defaultMaxEnergy = 100f;
                defaultEnergyPerSecond = 20f;

                speedMultiplierDefault = 0.8f;

                armPowerDefault = 1.4f;
                legPowerDefault = 1.2f;
                break;
            case "trickster":
                // Debug.Log("setting trickster");

                defaultMaxHP = 60f;
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

        if (isPlayer)
        {
            maxhp *= 2f;
            hp *= 2f;
        }
        // Debug.Log("After initializing " + characterType + ", hp: " + hp + "/" + maxhp + ", speedMulti,powerMulti = " + speedMultiplier + ", " + damageMultiplier);
        UpdateHealthBar();
        UpdateEnergyBar();
    }
    void MoveTowardsDefaultStance()
    //only happens with controlsEnabled. moves at speed towards default positions of hands and feet
    {
        float fighterX = transform.position.x;
        float fighterY = transform.position.y;
        float fighterHeadX = transform.position.x + stanceHeadTran.position.x;
        float fighterHeadY = transform.position.y + stanceHeadTran.position.y;

        // these need to be transform.right not orientedTran.right 
        if (stanceHeadTran.position.x > fighterX + reach)
        {
            stanceHeadTran.position -= transform.right * moveSpeed;
        }
        if (stanceHeadTran.position.x < fighterX - reach)
        {
            stanceHeadTran.position += transform.right * moveSpeed;
        }
        if (stanceHeadTran.position.y < fighterY - reach)
        {
            stanceHeadTran.position += transform.up * moveSpeed;
        }
        if (stanceHeadTran.position.y > fighterY + reach)
        {
            stanceHeadTran.position -= transform.up * moveSpeed;
        }

        UpdateDefaultStancePositions();

        // hands
        float distance = Vector3.Distance(stanceHand1Tran.position, hand1DefaultPos);
        if (distance > 0.1f)
        {
            stanceHand1Tran.LookAt(hand1DefaultPos);
            stanceHand1Tran.position += stanceHand1Tran.forward * Mathf.Max(moveSpeed * distance * 2, moveSpeed);
        }

        distance = Vector3.Distance(stanceHand2Tran.position, hand2DefaultPos);
        if (distance > 0.1f)
        {
            stanceHand2Tran.LookAt(hand2DefaultPos);
            stanceHand2Tran.position += stanceHand2Tran.forward * Mathf.Max(moveSpeed * distance * 2, moveSpeed);
        }

        // feet
        distance = Vector3.Distance(stanceFoot1Tran.position, foot1DefaultPos);
        if (distance > 0.1f)
        {
            stanceFoot1Tran.LookAt(foot1DefaultPos);
            stanceFoot1Tran.position += stanceFoot1Tran.forward * Mathf.Max(moveSpeed * distance * 2, moveSpeed);
        }

        distance = Vector3.Distance(stanceFoot2Tran.position, foot2DefaultPos);
        if (distance > 0.1f)
        {
            stanceFoot2Tran.LookAt(foot2DefaultPos);
            stanceFoot2Tran.position += stanceFoot2Tran.forward * Mathf.Max(moveSpeed * distance * 2, moveSpeed);
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
        if (stanceHeadActive)
        {
            fighterHead.transform.position = stanceHeadTran.position;
        }
        else
        {
            stanceHeadTran.position = fighterHead.transform.position;
        }

        if (stanceHand1Active)
        {
            lowerArm1Tran.position = stanceHand1Tran.position;
        }
        else
        {
            stanceHand1Tran.position = lowerArm1Tran.position;
        }
        if (stanceHand2Active)
        {
            lowerArm2Tran.position = stanceHand2Tran.position;
        }
        else
        {
            stanceHand2Tran.position = lowerArm2Tran.position;
        }


        // forces the shoulder to connect to the elbow
        upperArm1Tran.position = jointElbow1Tran.position;
        upperArm2Tran.position = jointElbow2Tran.position;

        if (stanceTorsoBotActive)
        {
            torsoBottom.transform.position = stanceTorsoBotTran.position;
        }
        else
        {
            stanceTorsoBotTran.position = torsoBottom.transform.position;
        }

        if (stanceTorsoTopActive)
        {
            torsoTop.transform.position = stanceTorsoTopTran.position;
        }
        else
        {
            stanceTorsoTopTran.position = torsoTop.transform.position;
        }

        if (stanceFoot1Active)
        {
            calf1Tran.position = stanceFoot1Tran.position;
        }
        else
        {
            stanceFoot1Tran.position = calf1Tran.position;
        }
        if (stanceFoot2Active)
        {
            calf2Tran.position = stanceFoot2Tran.position;
        }
        else
        {
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
        if (drawNormalElbow1)
        {
            Vector3[] arm1points = {
            jointShoulder1Tran.position,
            upperArm1Tran.position,
            lowerArm1Tran.position
            };
            arm1Renderer.SetPositions(arm1points);
        }
        else
        {
            Vector3[] arm1points = {
            jointShoulder1Tran.position,
            customElbow1Tran.position,
            lowerArm1Tran.position
            };
            arm1Renderer.SetPositions(arm1points);
        }

        // ARM 2
        if (drawNormalElbow2)
        {
            Vector3[] arm2points = {
            jointShoulder2Tran.position,
            upperArm2Tran.position,
            lowerArm2Tran.position
            };
            arm2Renderer.SetPositions(arm2points);
        }
        else
        {
            Vector3[] arm2points = {
            jointShoulder2Tran.position,
            customElbow2Tran.position,
            lowerArm2Tran.position
            };
            arm2Renderer.SetPositions(arm2points);
        }

        // LEG 1
        if (drawNormalKnee1)
        {
            Vector3[] leg1points = {
            jointPelvis1Tran.position,
            jointKnee1Tran.position,
            stanceFoot1Tran.position
            };
            leg1Renderer.SetPositions(leg1points);
        }
        else
        {
            Vector3[] leg1points = {
            jointPelvis1Tran.position,
            customKnee1Tran.position,
            stanceFoot1Tran.position
            };
            leg1Renderer.SetPositions(leg1points);
        }

        // LEG 2
        if (drawNormalKnee2)
        {
            Vector3[] leg2points = {
            jointPelvis2Tran.position,
            jointKnee2Tran.position,
            stanceFoot2Tran.position
            };
            leg2Renderer.SetPositions(leg2points);
        }
        else
        {
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

    public void SetStances(string type)
    {
        bool turnOn = true;
        switch (type)
        {
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

        if (type == "combat")
        {
            stanceTorsoTopActive = false;
            stanceTorsoBotActive = false;
        }
    }
    private void EqualizeBodyPartMass(bool equalize)
    {
        foreach (Rigidbody2D rb2d in allRigidbody2D)
        {
            rb2d.mass = 1f;
        }

        if (equalize)
        {
            return;
        }
        else
        {
            fighterHead.GetComponent<Rigidbody2D>().mass = 100f;
            torsoTop.GetComponent<Rigidbody2D>().mass = 10f;
        }
    }
    private void EnableGravity(bool gravityOn)
    {
        float gravityScale = 0f;
        if (gravityOn)
        {
            gravityScale = 1f;
        }
        foreach (Rigidbody2D rb2d in allRigidbody2D)
        {
            rb2d.gravityScale = gravityScale;
        }
    }
    public void SetRagdoll(bool ragdoll)
    {
        if (ragdoll)
        {
            //Debug.Log("ragdoll set true");
            notInAttackAnimation = false;
            EnableGravity(true);
            EqualizeBodyPartMass(true);
            SetColliders("ragdoll");
            SetStances("none");
        }
        else
        {
            //Debug.Log("ragdoll set false");
            notInAttackAnimation = true;
            EnableGravity(false);
            EqualizeBodyPartMass(false);
            SetColliders("combat");
            SetStances("combat");
        }
    }
    public void TakeHealing(float healing)
    {
        ChangeHealth(healing, false);
    }
    public void TakeDamage(float damage)
    {
        float damageTaken = damage * -1f;
        if (!notInAttackAnimation)
        { // if hit in the middle of an animation
            damageTaken *= 1.5f;
        }
        ChangeHealth(damageTaken, !notInAttackAnimation);
    }
    private void ChangeHealth(float change, bool isCrit)
    {
        hp += change;

        GameObject damageNumber = Instantiate(damageNumberPrefab, stanceHeadTran.position, transform.rotation);
        damageNumber.GetComponent<DamageNumberSprite>().type = "health";
        damageNumber.GetComponent<DamageNumberSprite>().changeNumber = change;
        damageNumber.GetComponent<DamageNumberSprite>().isCrit = isCrit;

        if (hp > maxhp)
        {
            hp = maxhp;
        }
        UpdateHealthBar();
    }
    public void SetHealth(float amount)
    {
        hp = amount;
        if (hp > maxhp)
        {
            hp = maxhp;
        }
        UpdateHealthBar();
    }
    public void ChangeEnergy(float change)
    {
        currentEnergy += change;
        if (currentEnergy > maxEnergy)
        {
            currentEnergy = maxEnergy;
        }
        UpdateEnergyBar();

        if (Mathf.Abs(change) < 1f || !isPlayer)
        {
            return;
        }
        GameObject damageNumber = Instantiate(damageNumberPrefab, stanceHeadTran.position, transform.rotation);
        damageNumber.GetComponent<DamageNumberSprite>().type = "energy";
        damageNumber.GetComponent<DamageNumberSprite>().changeNumber = change;
    }
    public void ReplenishEnergy()
    {
        currentEnergy = maxEnergy;
    }
    public void ReplenishHealth()
    {
        hp = maxhp;
        UpdateHealthBar();
    }
    void GainEnergy()
    {
        if (!gainEnergyOn)
        {
            return;
        }
        else
        {
            ChangeEnergy(energyPerSecond / 60f);
        }
    }
    public void UpdateHealthBar()
    {
        if (healthBar == null)
        {
            return;
        }
        if (hp <= 0)
        {
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
    public void UpdateEnergyBar()
    {
        if (energyBar == null)
        {
            return;
        }
        if (currentEnergy <= 0f)
        {
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
    public void Die()
    {
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
        if (isPlayer)
        {
            gameStateManager.GetComponent<GameStateManagerScript>().GameOver();
            return;
        }
        if (!isGhost && !isPlayer)
        {
            gameStateManager.GetComponent<GameStateManagerScript>().AddScore(100);
        }
        Destroy(this.transform.root.gameObject, 5);
    }
    public IEnumerator Stumble(float time, Vector3 direction, float impulse, GameObject partHit) // enable physics temporarily then turn back on stances
    {
        int frames = (int)(time * 60f);
        SetRagdoll(true);

        // adds force to the ragdoll
        Rigidbody2D rb2d = partHit.GetComponent<Rigidbody2D>();
        if (rb2d != null)
        {
            rb2d.AddForce(direction * impulse, ForceMode2D.Impulse);
        }

        // waits x frames until regaining control
        for (int i = 0; i < frames; i++)
        {
            yield return null;
        }
        SetRagdoll(false);
    }

    public void Move(Vector3 direction) // put an animation on this shit while it's happening
    {
        if (airborne)
        {
            return;
        }
        //controlsEnabled = false;
        //StartCoroutine(Step((int)direction.x));
        if (notInAttackAnimation)
        {
            transform.position += Vector3.Normalize(direction) * moveSpeed / 2f;
        }
        else
        {
            return;
        }
    }
    public Vector3 GetSectorPosition(int sector)
    {
        Vector3 output = new Vector3(0, 0, 0);
        float reachForX = reach * transform.localScale.x;
        switch (sector)
        {
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
    public Vector3 GetHeadDirectionToSector(int sector)
    {
        Vector3 sectorPos = GetSectorPosition(sector);
        Vector3 direction = sectorPos - stanceHeadTran.position;
        return Vector3.Normalize(direction);
    }
    public void MoveHead(int direction)
    {
        float toMoveSpeed = moveSpeed;
        if (!notInAttackAnimation)
        {
            toMoveSpeed /= 3f;
        }
        float playerX = transform.position.x;
        float playerY = transform.position.y;
        float playerHeadX = stanceHeadTran.position.x;
        float playerHeadY = stanceHeadTran.position.y;

        switch (direction)
        {
            case 1: // up
                if (playerHeadY + toMoveSpeed < playerY + reach)
                {
                    stanceHeadTran.position += Vector3.up * toMoveSpeed;
                }
                break;
            case 2: // down
                if (playerHeadY - toMoveSpeed > playerY - reach)
                {
                    stanceHeadTran.position += Vector3.down * toMoveSpeed;
                }
                break;
            case 3: // left
                if (playerHeadX - toMoveSpeed > playerX - reach && !isTurning)
                {
                    stanceHeadTran.position += Vector3.left * toMoveSpeed;
                }
                break;
            case 4: // right
                if (playerHeadX + toMoveSpeed < playerX + reach && !isTurning)
                {
                    stanceHeadTran.position += Vector3.right * toMoveSpeed;
                }
                break;
        }
    }
    private void MoveHeadInDirection(Vector3 direction)
    {
        stanceHeadTran.position += Vector3.Normalize(direction) * moveSpeed;
    }
    public void MoveHeadAtPosition(Vector3 targetPosition)
    {
        float toMoveSpeed = moveSpeed;
        if (!notInAttackAnimation)
        {
            toMoveSpeed /= 3f;
        }

        Vector3 targetPos2D = new Vector3(targetPosition.x, targetPosition.y, 0f);
        if (targetPos2D.x > transform.position.x + reach)
        {
            targetPos2D = new Vector3(transform.position.x + reach, targetPos2D.y, 0f);
        }
        if (targetPos2D.x < transform.position.x - reach)
        {
            targetPos2D = new Vector3(transform.position.x - reach, targetPos2D.y, 0f);
        }


        if (targetPos2D.y > transform.position.y + reach)
        {
            targetPos2D = new Vector3(targetPos2D.x, transform.position.y + reach, 0f);
        }
        if (targetPos2D.y < transform.position.y - reach)
        {
            targetPos2D = new Vector3(targetPos2D.x, transform.position.y - reach, 0f);
        }

        if (isTurning)
        {
            targetPos2D = new Vector3(0f, targetPos2D.y, 0f);
        }

        stanceHeadTran.LookAt(targetPos2D);
        Vector3 moveVector = stanceHeadTran.forward * toMoveSpeed;

        if (moveSpeed >= Vector3.Distance(stanceHeadTran.position, targetPos2D) && IsPositionWithinSectors(targetPos2D))
        {
            return;
        }

        stanceHeadTran.position += moveVector;
    }
    public void MoveHeadTowardsSector(int sector)
    {
        // 0 1 2 = bottom back, bottom, bottom forward
        // 3 4 5 = center back, true center, center forward
        // 6 7 8 = top back, top, top forard
        MoveHeadInDirection(GetHeadDirectionToSector(sector));
    }
    public bool IsHeadWithinSectors() // checks if head is within boundaries
    {
        return IsPositionWithinSectors(stanceHeadTran.position);
    }
    public bool IsPositionWithinSectors(Vector3 posVector) // checks if head is within boundaries
    {
        if (posVector.x > transform.position.x + reach || posVector.y > transform.position.y + reach)
        {
            return false;
        }
        if (posVector.x < transform.position.x - reach || posVector.y < transform.position.y - reach)
        {
            return false;
        }
        return true;
    }
    // finds what sector the head is in, in order to do a
    public int GetHeadSector()
    {
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
    public void SwapHingeAngles()
    {
        foreach (HingeJoint2D hinge in allHinges)
        {
            JointAngleLimits2D newLimits = hinge.limits;
            newLimits.min = 0 - hinge.limits.min;
            newLimits.max = 0 - hinge.limits.max;
            hinge.limits = newLimits;
        }
    }
    public void TurnBody()
    {

        float targetScale = 0 - transform.localScale.x; // can't use directionMultiplier for this!!!
        transform.localScale = new Vector3(targetScale, 1f, 1f);

        switch (transform.localScale.x)
        {
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
    IEnumerator GoToCenterXAndTurn()
    {
        if (isTurning)
        {
            yield break;
        }
        isTurning = true;
        notInAttackAnimation = false;

        float directionMultiplier = orientedTran.position.x - stanceHeadTran.position.x;
        Vector3 movementVector = transform.right * directionMultiplier * moveSpeed * 2f;
        while (Mathf.Abs(transform.position.x - stanceHeadTran.position.x) > 0.1f)
        {
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

        isTurning = false;
        notInAttackAnimation = true;
        //Debug.Log("facing right: " + facingRight);
    }

    IEnumerator Spin180Animation()
    {
        for (int i = 0; i < 180; i++)
        {
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y - 1f, 0);
            yield return null;
        }
    }
    public void TurnTo(string direction)
    {
        if (!notInAttackAnimation || isTurning)
        {
            return;
        }
        // only turns if it is not already facing the direction
        if (
            (direction == "right" && facingRight)
            || (direction == "left" && !facingRight)
            )
        {
            return;
        }
        StartCoroutine(GoToCenterXAndTurn());
        MoveAndDrawBody();
    }
    public void ApplyNewMoveset(string[] newArmMoveset, string[] newLegMoveset)
    {
        for (int i = 0; i < 9; i++)
        {
            armMoveset[i] = newArmMoveset[i];
            legMoveset[i] = newLegMoveset[i];
        }
    }
    public void Attack(string attackType) // attackType = "arms" or "legs"
    {
        string[] sectors = {
        "bottom back", "bottom", "bottom forward",
        "center back", "true center", "center forward",
        "top back", "top", "top forward"
        };
        notInAttackAnimation = false;

        if (attackType == "groundslam")
        {
            StartCoroutine(GroundSlam());
            return;
        }

        int sector = GetHeadSector();

        switch (attackType)
        {
            case "arms":
                UseSelectedMove("arms", armMoveset[sector]);
                break;
            case "legs":
                UseSelectedMove("legs", legMoveset[sector]);
                break;
        }
    }
    public void UseSelectedMove(string armsOrLegs, string attackName)
    {
        switch (armsOrLegs)
        {
            case "arms":
                switch (attackName)
                {
                    case "hook":
                        StartCoroutine(Hook(false));
                        return;
                    case "jabcombo":
                        StartCoroutine(JabCombo("combo"));
                        return;
                    case "jabaggressive":
                        StartCoroutine(JabCombo("aggressive"));
                        return;
                    case "uppercut":
                        StartCoroutine(Uppercut());
                        return;
                }
                Debug.Log(attackName + " does not exist for arm attacks");
                return;
            case "legs":
                switch (attackName)
                {
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
                Debug.Log(attackName + " does not exist for leg attack");
                return;
        }
    }
    public float[] GetAttackInfo(string name)
    {
        float[] output = new float[4]; // output = [damage, knockback multiplier, energy cost, normal time taken]
        float damage = -1f;
        float pushMultiplier = -1f;
        float energyCost = -1f;
        float timeTaken = -1f;
        switch (name.ToLower())
        {
            case "hook":
                damage = 50f * armPower;
                pushMultiplier = 1f;
                energyCost = 15f * armPower;
                timeTaken = .25f;
                break;
            case "jabaggressive":
                damage = 30f * armPower;
                pushMultiplier = 3f;
                energyCost = 13f * armPower;
                timeTaken = .12f;
                break;
            case "jabcombo":
                damage = 30f * armPower;
                pushMultiplier = 3f;
                energyCost = 25f * armPower;
                timeTaken = .12f;
                break;
            case "roundhousekick":
                damage = 60f * legPower;
                pushMultiplier = 0.5f;
                energyCost = 40f * legPower;
                timeTaken = .3f;
                break;
            case "roundhousekickhigh":
                damage = 70f * legPower;
                pushMultiplier = 0.5f;
                energyCost = 50f * legPower;
                timeTaken = .36f;
                break;
            case "pushkick":
                damage = 60f * legPower;
                pushMultiplier = 2f;
                energyCost = 45f * legPower;
                timeTaken = .35f;
                break;
            case "flyingkick":
                damage = 80f * legPower;
                pushMultiplier = 2f;
                energyCost = 70f * legPower;
                timeTaken = .35f;
                break;
            case "uppercut":
                damage = 60f * armPower;
                pushMultiplier = 1f;
                energyCost = 18f * armPower;
                timeTaken = .25f;
                break;
            case "groundslam":
                damage = 60f;
                pushMultiplier = 2.5f;
                energyCost = 100f;
                timeTaken = 0.8f;
                break;
            case "knee":
                damage = 65f * legPower;
                pushMultiplier = 1f;
                energyCost = 20f * legPower;
                timeTaken = 0.25f;
                break;
        }
        timeTaken /= speedMultiplier;

        if (isWeakenedEffect > 0)
        {
            damage -= 20f * isWeakenedEffect;
            if (damage < 5f)
            {
                damage = 5f;
            }
        }

        if(explosiveLevel > 0){
            pushMultiplier *= (float)explosiveLevel * 1.25f;
        }

        output[0] = damage;
        output[1] = pushMultiplier;
        output[2] = energyCost;
        output[3] = timeTaken;
        return output;
    }
    public Vector3 GetAttackTargetPos(string attackName)
    {
        attackName = attackName.ToLower();
        Vector3 output = new Vector3(0, 0, 0);
        switch (attackName)
        {
            case "hook":
                output =
                    stanceHeadTran.position
                    + orientedTran.right * 3.75f
                    - transform.up * .25f;
                break;
            case "jabcombo":
                output =
                    stanceHand2Tran.position
                    + orientedTran.right * 1.2f;
                break;
            case "jabaggressive":
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
            case "knee":
                output = jointPelvis2Tran.localPosition
                + transform.right * 0f
                + transform.up * -0.25f;
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
        if (!isGhost && !isPlayer)
        {
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
        }

        if (isPlayer)
        { // is the player, creates invisible attack area that checks for collision with enemies
            newAttackScript.lifespan = 0;
            newAttackScript.creatorType = "player";
            newAttackScript.UpdateSprites();
        }
        //Debug.Break();
    }
    private void DirectlyDamage(float damage, FighterScript targetFighterScript, Vector3 strikeForceVector)
    {
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
            strikeForceVector
            );

        Destroy(directDamager);
    }
    public IEnumerator ParentWasPushed()
    {
        while (Vector3.Magnitude(parentRB.velocity) > 0f)
        {
            transform.position += new Vector3(
                parentRB.velocity.x / 60f,
                parentRB.velocity.y / 60f,
                0f);
            yield return null;
        }
    }
    IEnumerator KeepHandsInPlace()
    {
        Vector3 fromHeadHand1 = stanceHeadTran.position - stanceHand1Tran.position;
        Vector3 fromHeadHand2 = stanceHeadTran.position - stanceHand2Tran.position;

        while (true)
        {
            Vector3 currentHand1 = stanceHeadTran.position - fromHeadHand1;
            Vector3 currentHand2 = stanceHeadTran.position - fromHeadHand2;
            lowerArm1Tran.position = currentHand1;
            lowerArm2Tran.position = currentHand2;
            stanceHand1Tran.position = currentHand1;
            stanceHand2Tran.position = currentHand2;
            yield return null;
        }
    }

    IEnumerator Hook(bool partOfCombo) // true means no energy cost
    {
        float[] info = GetAttackInfo("hook");
        float damage = info[0];
        float pushMultiplier = info[1];
        float energyCost = info[2];
        if (partOfCombo)
        {
            energyCost = 0f;
        }
        float timeTaken = info[3]; //seconds

        if (currentEnergy < energyCost)
        {
            notInAttackAnimation = true;
            yield break;
        }
        else
        {
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
        for (int i = 0; i < windUpFrames; i++)
        {
            stanceHeadTran.position += orientedTran.right * moveSpeed;
            stanceHand1Tran.position = Vector3.Lerp(handStartPos, handWindUpTarget, ((float)i) / ((float)windUpFrames));
            yield return null;
        }


        // throw punch animation
        Vector3 throwStartPos = stanceHand1Tran.position;
        for (int i = 0; i < throwPunchFrames; i++)
        {
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
        while (Vector3.Distance(stanceHand2Tran.position, hand2DefaultPos) > 0.1f)
        {
            MoveTowardsDefaultStance();
            MoveTowardsDefaultStance();
            yield return null;
        }
        notInAttackAnimation = true;
    }
    IEnumerator JabCombo(string type) // type = "combo" or "aggressive". combo is a 2 hit combo, aggressive is a single far jab
    {
        float[] info = GetAttackInfo("jab" + type);
        float damage = info[0];
        float pushMultiplier = info[1];
        float energyCost = info[2];
        float timeTaken = info[3]; //seconds

        if (currentEnergy < energyCost)
        {
            notInAttackAnimation = true;
            yield break;
        }
        else
        {
            ChangeEnergy(0f - energyCost);
        }

        Vector3 attackTarget = GetAttackTargetPos("Jab" + type);
        int framesTaken = (int)(timeTaken * 60);
        Vector3 handStartPos = stanceHand2Tran.position;
        float hand2DistancePerFrame = Mathf.Abs(stanceHand2Tran.position.x - attackTarget.x) / framesTaken;

        float headDistancePerFrame = 0.3f * hand2DistancePerFrame;
        if (type == "aggressive")
        {
            headDistancePerFrame = 0.3f * hand2DistancePerFrame;
        }


        Vector3 headMoveVector = orientedTran.right * headDistancePerFrame;
        if (type == "combo")
            headMoveVector = -1f * orientedTran.right * headDistancePerFrame;

        // jab animation
        for (int i = 0; i < framesTaken; i++)
        {
            stanceHeadTran.position += headMoveVector;

            //moves jabbing hand 
            stanceHand2Tran.LookAt(attackTarget);
            stanceHand2Tran.position = Vector3.Lerp(handStartPos, attackTarget, (float)i / (float)framesTaken);

            if (type == "aggressive")
            {
                // moves nonjab hand to guard
                stanceHand1Tran.LookAt(stanceHeadTran.position + orientedTran.right * 1f);
                stanceHand1Tran.position += stanceHand1Tran.forward * hand2DistancePerFrame;
                upperArm1Tran.position = jointElbow1Tran.position;
            }
            yield return null;
        }
        StrikeThisLocation(damage, pushMultiplier, attackTarget, jointElbow2Tran.position, stanceHand2, lowerArm2, 0.5f, 1f);

        if (type == "aggressive")
        {
            // pull jabbing hand back
            while (Vector3.Distance(stanceHand2Tran.position, hand2DefaultPos) > 0.1f)
            {
                MoveTowardsDefaultStance();
                MoveTowardsDefaultStance();
                yield return null;
            }
        }

        // start hook if combo
        if (type == "combo")
        {
            yield return StartCoroutine(Hook(true));
            yield return null;
        }
        if (type == "aggressive") // end animation if aggressive
        {
            notInAttackAnimation = true;
        }
        //Debug.Log("controls re-enabled");
    }
    IEnumerator Uppercut()
    {
        float[] info = GetAttackInfo("uppercut");
        float damage = info[0];
        float pushMultiplier = info[1];
        float energyCost = info[2];
        float timeTaken = info[3]; //seconds

        if (currentEnergy < energyCost)
        {
            notInAttackAnimation = true;
            yield break;
        }
        else
        {
            notInAttackAnimation = false;
            ChangeEnergy(0f - energyCost);
        }
        int framesTaken = (int)(timeTaken * 60f);
        int windUpFrames = (int)((float)framesTaken * .3f);
        int throwingFrames = (int)((float)framesTaken * .7f);

        Vector3 attackTarget = GetAttackTargetPos("Uppercut");

        yield return attackTarget;
        float distance = Vector3.Distance(attackTarget, stanceHand1Tran.position);

        float windUpDistancePerFrame = .5f / windUpFrames;
        float punchDistancePerFrame = distance / throwingFrames;

        // throw punch animation
        for (int i = 0; i < windUpFrames; i++)
        {
            stanceHeadTran.position += Vector3.up * windUpDistancePerFrame;
            stanceHand1Tran.position -= Vector3.up * windUpDistancePerFrame;// * 2f;
            yield return null;
        }

        for (int i = 0; i < throwingFrames; i++)
        {
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
        while (Vector3.Distance(stanceHand2Tran.position, hand2DefaultPos) > 0.1f)
        {
            MoveTowardsDefaultStance();
            MoveTowardsDefaultStance();
            yield return null;
        }
        notInAttackAnimation = true;
    }
    IEnumerator RoundhouseKick(string type) // type = high or straight
    {
        float[] info = GetAttackInfo("roundhousekick");
        if (type == "high")
        {
            info = GetAttackInfo("roundhousekickhigh");
        }
        float damage = info[0];
        float pushMultiplier = info[1];
        float energyCost = info[2];
        float timeTaken = info[3]; //seconds

        if (currentEnergy < energyCost)
        {
            notInAttackAnimation = true;
            yield break;
        }
        else
        {
            ChangeEnergy(0f - energyCost);

        }
        int framesTaken = (int)(timeTaken * 60f);

        Transform kickingFootTran = stanceFoot1Tran;

        //stanceTorsoBotActive = true;
        Vector3 origHeadPosition = stanceHeadTran.position;
        Vector3 origBotTorsoPosition = torsoBottom.transform.position;

        Vector3 attackTarget = GetAttackTargetPos("RoundhouseKick");
        if (type == "high")
        {
            drawNormalKnee1 = false;
            attackTarget = GetAttackTargetPos("RoundhouseKickHigh");

        }

        float footMovingDistance = Vector3.Distance(kickingFootTran.position, attackTarget);
        float distancePerFrame = footMovingDistance / framesTaken;

        // setting up balancing animation
        Vector3 torsoMoveVelocity = orientedTran.right * distancePerFrame * 0.3f;
        Vector3 headMoveVelocity = torsoMoveVelocity * 0.5f;
        Vector3 bodyMoveVelocity = new Vector3(0f, 0f, 0f);
        if (type == "high")
        {
            headMoveVelocity = orientedTran.right * moveSpeed * -2f - orientedTran.up * moveSpeed * 0.5f;
            torsoMoveVelocity = new Vector3(0f, 0f, 0f);
            bodyMoveVelocity = orientedTran.right * distancePerFrame * 0.3f;
        }

        // kick animation
        for (int i = 0; i < framesTaken; i++)
        {
            // balancing
            stanceHeadTran.position += headMoveVelocity;
            //stanceTorsoBotTran.position += torsoMoveVelocity;
            transform.position += bodyMoveVelocity;
            stanceFoot2Tran.position -= bodyMoveVelocity; // keeps foot in place

            kickingFootTran.LookAt(attackTarget);
            kickingFootTran.position += kickingFootTran.forward * distancePerFrame;

            // custom joint here, midway from foot to pelvis
            customKnee1Tran.position = (jointPelvis1Tran.position + kickingFootTran.position) / 2f;

            // guarding
            Vector3 hand1Guard = stanceHeadTran.position + orientedTran.right * 1f + orientedTran.up * .25f;
            float distanceHand1 = Vector3.Distance(hand1Guard, stanceHand1Tran.position);
            stanceHand1Tran.LookAt(hand1Guard);
            stanceHand1Tran.position += stanceHand1Tran.forward * Mathf.Max(moveSpeed * distanceHand1 * 2f, moveSpeed / 2f);

            // other hand goes the other direction
            if (type == "straight")
            {
                Vector3 hand2Guard = stanceHeadTran.position + orientedTran.right * 1.5f - orientedTran.up * 2f;
                float distanceHand2 = Vector3.Distance(hand2Guard, stanceHand2Tran.position);
                stanceHand2Tran.LookAt(hand2Guard);
                stanceHand2Tran.position += stanceHand2Tran.forward * Mathf.Max(moveSpeed * distanceHand2 * 2f, moveSpeed / 2f);
            }
            else
            {
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

        // set up not moving foot
        Vector3 origFoot2Pos = stanceFoot2Tran.position;

        for (int i = 0; i < returnFrames; i++) // tested number of frames on acolyte
        {
            MoveTowardsDefaultStance();

            // draw leg correctly
            customKnee1Tran.position = (jointPelvis1Tran.position + kickingFootTran.position) / 2f;

            // go back to place
            stanceHeadTran.position -= headMoveVelocity * returnSpeedMult;
            stanceHand2Tran.position += orientedTran.right * moveSpeed * 1f - orientedTran.up * moveSpeed * 2f;
            stanceTorsoBotTran.position -= torsoMoveVelocity * returnSpeedMult;
            transform.position -= bodyMoveVelocity * returnSpeedMult;

            // keep foot in place
            stanceFoot2Tran.position = origFoot2Pos;
            yield return null;
        }

        drawNormalKnee1 = true;
        notInAttackAnimation = true;
        //Debug.Log("controls re-enabled");
    }
    IEnumerator PushKick(string type) // type = "grounded" or "flying" because I am lazy to make another ienumerator for a flying push kick
    {
        float[] info = GetAttackInfo("pushkick");
        if (type == "flying")
        {
            info = GetAttackInfo("flyingkick");
            info[2] = 0; // sets energy cost to 0
        }

        float damage = info[0]; // to be set in below switch statement
        float pushMultiplier = info[1];
        float energyCost = info[2];
        float totalTimeTaken = info[3];
        if (type == "flying")
        {
            totalTimeTaken *= speedMultiplier;
        }

        bool grounded = true;

        if (currentEnergy < energyCost)
        {
            notInAttackAnimation = true;
            yield break;
        }
        else
        {
            ChangeEnergy(0f - energyCost);
        }

        // range and animation setup
        float range = 0f;
        switch (type)
        {
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
        int raiseFootFrames = (int)(.4f * totalTimeTaken * 60f);
        int kickFrames = (int)(.6f * totalTimeTaken * 60f);


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
        if (type == "grounded")
        {
            kickHeightMod = 0.5f;
        }
        else
        {
            kickHeightMod = 0f;
        }
        Vector3 origKickFootLocalPos = kickingFootTran.localPosition;
        Vector3 raisedKickLocalPos = stanceTorsoBotTran.localPosition + transform.right * 1f + transform.up * kickHeightMod;

        for (int i = 0; i < raiseFootFrames; i++)
        {
            //kickingFootTran.position += orientedTran.up * kickDistancePerFrame;
            kickingFootTran.localPosition = Vector3.Lerp(origKickFootLocalPos, raisedKickLocalPos, ((float)i / (float)raiseFootFrames));
            yield return null;
        }

        // target vector created for real here
        if (type == "grounded")
        {
            kickHeightMod = 0.33f;
        }
        else
        {
            kickHeightMod = -.25f;
        }
        attackTarget = stanceTorsoBotTran.localPosition + transform.right * range + transform.up * kickHeightMod;

        Vector3 footStartPos = kickingFootTran.localPosition;

        Vector3 torsoMoveDistance = transform.right * kickDistancePerFrame * 0.3f;

        // extend and kick
        for (int i = 0; i < kickFrames; i++)
        {

            // lean back
            if (grounded)
            {
                stanceTorsoTopActive = true;
                stanceHeadTran.localPosition += torsoMoveDistance * .75f;
                stanceTorsoTopTran.localPosition += torsoMoveDistance * .75f;
                stanceTorsoBotTran.localPosition += torsoMoveDistance;
            }
            else
            {
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

        while (Vector3.Distance(kickingFootTran.position, foot2DefaultPos) > .25f)
        {
            MoveTowardsDefaultStance();
            yield return null;
        }
        notInAttackAnimation = true;
        //Debug.Log("controls re-enabled");
    }
    IEnumerator FlyingKickPart2(float jumpSpeed)
    {
        IEnumerator handStanceCoroutine = KeepHandsInPlace();
        StartCoroutine(handStanceCoroutine);
        notInAttackAnimation = false;
        airborne = true;
        while (stanceHeadTran.position.y <= transform.position.y + reach)
        {
            stanceHeadTran.position += orientedTran.up * jumpSpeed / 60f;
            //stanceFoot2Tran.position += orientedTran.up * jumpSpeed / 60f;
            yield return null;
        }

        //Debug.Log("jumped into air");
        fighterRB.velocity = new Vector2(5f * transform.localScale.x, jumpSpeed);
        fighterRB.gravityScale = 3f;
        stanceFoot1Tran.position += Vector3.up * 0.05f;
        stanceFoot2Tran.position += Vector3.up * 0.05f;
        StopCoroutine(handStanceCoroutine);

        // flying front kick
        StartCoroutine(PushKick("flying"));
        notInAttackAnimation = false;

        Vector3 jumpPos = transform.position;
        float y = transform.position.y;
        while (y >= transform.position.y)
        {
            yield return null;
        }
        // raise back leg
        Vector3 stanceFoot1Target = jointPelvis1Tran.position + orientedTran.right - orientedTran.up * 0.5f;
        float timeTaken = 15f;
        for (int i = 0; i < 15; i++)
        {
            stanceFoot1Tran.position = Vector3.Lerp(stanceFoot1Tran.position, stanceFoot1Target, ((float)(i)) / timeTaken);
            yield return null;
        }

        while (stanceFoot1Tran.position.y > groundLevel
            && stanceFoot2Tran.position.y > groundLevel)
        {
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
        for (int i = 0; i < 10; i++)
        {
            stanceHeadTran.position -= Vector3.up * 0.5f / 10f;
            yield return null;
        }
        airborne = false;
    }
    IEnumerator FlyingKick()
    {
        float energyCost = GetAttackInfo("flyingkick")[2];
        // no power listed here, rather it is listed on the front kick
        if (currentEnergy < energyCost)
        {
            notInAttackAnimation = true;
            yield break;
        }
        else
        {
            ChangeEnergy(0f - energyCost);
        }

        notInAttackAnimation = false;
        int sector = GetHeadSector();

        // move head to top forward section
        Vector3 headTargetPosition = orientedTran.position + orientedTran.right + orientedTran.up;
        Vector3 initHeadPosition = fighterHead.transform.position;
        float headMoveTime = 20f / speedMultiplier;
        IEnumerator keepHandsInPlace = KeepHandsInPlace();
        StartCoroutine(keepHandsInPlace);
        for (int i = 0; i < (int)headMoveTime; i++)
        {
            stanceHeadTran.position = Vector3.Lerp(initHeadPosition, headTargetPosition, ((float)(i)) / headMoveTime);
            yield return null;
        }
        StopCoroutine(keepHandsInPlace);

        // jump and kick at the same time
        yield return StartCoroutine(FlyingKickPart2(10f));
        yield return null;
    }
    IEnumerator GroundSlam()
    {
        notInAttackAnimation = false;

        float[] info = GetAttackInfo("groundslam");
        float damage = info[0];
        float pushMultiplier = info[1];
        float energyCost = info[2];
        float totalAnimationTimeTaken = info[3];

        if (currentEnergy < energyCost)
        {
            notInAttackAnimation = true;
            yield break;
        }
        else
        {
            ChangeEnergy(0f - energyCost);
            UpdateEnergyBar();
            gainEnergyOn = false;
        }

        float timeTakenWindUp = 0.33f * totalAnimationTimeTaken;
        float timeTakenJump = 0.66f * totalAnimationTimeTaken;
        int framesTakenWindUp = (int)(timeTakenWindUp * 60f);
        int framesTakenJump = (int)(timeTakenJump * 60f);

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

        for (int i = 0; i < framesTakenWindUp; i++)
        {
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

        for (int i = 0; i < framesTakenJump; i++)
        {
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
            && stanceFoot2Tran.position.y > groundLevel)
        {
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
        List<GameObject> tempAllEnemiesList = gameStateManagerScript.enemyManagerScript.GetAllEnemiesList(); // removes null enemies before returning the list

        // use 1 attackAreaScript to attack all enemies
        GameObject directDamager = Instantiate(
            AttackWarningPrefab,
            transform.position + Vector3.up * 100f,
            transform.rotation
        );
        AttackAreaScript damagerScript = directDamager.GetComponent<AttackAreaScript>();

        // run through the list to damage them
        foreach (GameObject enemy in tempAllEnemiesList)
        {
            if (!enemy.GetComponent<EnemyWithGhostScript>().enemyFighterScript.airborne)
            {
                FighterScript targetFighterScript = enemy.GetComponent<EnemyWithGhostScript>().enemyFighterScript;

                Vector3 strikeForceDirection = (
                    Vector3.Normalize(targetFighterScript.transform.position - transform.position) + Vector3.up
                );

                Vector3 strikeForceVector = strikeForceDirection * damage * pushMultiplier;

                DirectlyDamage(damage, targetFighterScript, strikeForceVector);
            }
        }
        Destroy(directDamager);

        gainEnergyOn = true;
        notInAttackAnimation = true;
    }
    IEnumerator Knee(string type) // grounded or flying
    {
        float[] info = GetAttackInfo("knee");
        float damage = info[0];
        float pushMultiplier = info[1];
        float energyCost = info[2];
        float timeTaken = info[3]; //seconds
        if (currentEnergy < energyCost)
        {
            notInAttackAnimation = true;
            yield break;
        }
        else
        {
            ChangeEnergy(0f - energyCost);
        }

        int framesTaken = (int)(timeTaken * 60f);

        // move foot
        Vector3 footFrom = stanceFoot1Tran.localPosition;
        Vector3 footTo = GetAttackTargetPos("knee"); // this is not where the attack will spawn but rather where the foot will go to form the knee attack

        // move head
        Vector3 localHeadFrom = stanceHeadTran.localPosition;
        Vector3 localHeadTo = new Vector3(
            stanceHeadTran.localPosition.x - 1.5f,
            stanceHeadTran.localPosition.y + ((transform.position.y + reach) - stanceHeadTran.position.y), // distance to top sector
            0f
        );

        // move body with knee
        float bodyMove = 2f;
        if (type == "flying")
        {
        }
        Vector3 wholeBodyStart = transform.position;
        Vector3 wholeBodyEnd = transform.position + orientedTran.right * bodyMove;
        Vector3 foot2Start = stanceFoot2Tran.localPosition;
        Vector3 foot2End = stanceFoot2Tran.localPosition - transform.right * bodyMove;

        // knee animation
        for (int i = 0; i < framesTaken; i++)
        {
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

        while (Vector3.Distance(stanceFoot1Tran.position, foot1DefaultPos) > .25f)
        {
            MoveTowardsDefaultStance();
            yield return null;
        }
        notInAttackAnimation = true;
    }
}
