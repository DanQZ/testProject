using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

// to do
/*

-sorting layer stuff

*/

public class FighterScript : MonoBehaviour
{
    public GameObject gameStateManager;
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

    public string characterType;
    private float speedMultiplier;
    private float defaultMoveSpeed;
    private float moveSpeed;
    private float powerMultiplier;
    private int maxhp;
    public int hp;

    public int maxEnergy;
    public int currentEnergy;
    int nextEnergyRegainFrame;
    int energyPerSecond;
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

    public GameObject jointElbow1;
    private Transform jointElbow1Tran;
    public GameObject jointElbow2;
    private Transform jointElbow2Tran;

    public GameObject jointPelvis1;
    private Transform jointPelvis1Tran;
    public GameObject jointPelvis2;
    private Transform jointPelvis2Tran;

    public GameObject jointKnee1;
    private Transform jointKnee1Tran;
    public GameObject jointKnee2;
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

    public bool controlsEnabled = true; // enabled, fighter can move head around + limbs move to default positions. disable when in an animation
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
    private Vector3 hand1DefaultVector;
    private Vector3 hand2DefaultVector;

    //default stances relative to fighter position
    private Vector3 foot1DefaultVector;
    private Vector3 foot2DefaultVector;

    private HingeJoint2D torsoTopHinge;
    private HingeJoint2D torsoBottomHinge;
    private HingeJoint2D upperArm1Hinge;
    private HingeJoint2D lowerArm1Hinge;
    private HingeJoint2D upperArm2Hinge;
    private HingeJoint2D lowerArm2Hinge;
    private HingeJoint2D thigh1Hinge;
    private HingeJoint2D calf1Hinge;
    private HingeJoint2D thigh2Hinge;
    private HingeJoint2D calf2Hinge;
    private HingeJoint2D[] allHinges = new HingeJoint2D[8];
    private TrailRenderer[] allTrails = new TrailRenderer[8];
    private List<GameObject> allBodyParts = new List<GameObject>();
    private List<LineRenderer> allLineRenderers = new List<LineRenderer>();
    private List<Rigidbody2D> allRigidbody2D = new List<Rigidbody2D>();
    private List<PolygonCollider2D> allPolyCollider2D = new List<PolygonCollider2D>();
    private List<GameObject> allStances = new List<GameObject>();
    //private LineRenderer[] allLineRenderers = new LineRenderer[6];

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

        maxEnergy = 100;
        currentEnergy = 100;
        energyPerSecond = 15;
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
        allHinges[0] = (torsoTopHinge);
        allHinges[1] = (torsoBottomHinge);
        allHinges[2] = (upperArm1Hinge);
        allHinges[3] = (upperArm2Hinge);
        // I DONT KNOW WHY BUT FOR SOME REASON FLIPPING THE ANGLES ON THE LOWER ARM HINGES FLIPS THEM THE WRONG WAY WHEN YOU TURN AROUND?????????
        //allHinges[4] = (lowerArm1Hinge);
        //allHinges[5] = (lowerArm2Hinge);
        allHinges[4] = (thigh1Hinge);
        allHinges[5] = (thigh2Hinge);
        allHinges[6] = (calf1Hinge);
        allHinges[7] = (calf2Hinge);
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

        jointPelvis1Tran = jointPelvis1.transform;
        jointPelvis2Tran = jointPelvis2.transform;

        jointKnee1Tran = jointKnee1.transform;
        jointKnee2Tran = jointKnee2.transform;
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
    public void SetColor(Color newColor)
    {
        foreach (LineRenderer renderer in allLineRenderers)
        {
            renderer.startColor = newColor;
            renderer.endColor = newColor;
        }
        Color torsoColor = new Color((newColor.r + 1f) / 2f, (newColor.g + 1f) / 2f, (newColor.b + 1f) / 2f, 1);
        torsoRenderer.startColor = torsoColor;
        torsoRenderer.endColor = torsoColor;

        headSprite.color = newColor;
    }
    public void SetOpacity(float alpha) // 1f = opaque, 0f = transparent
    {
        Color bodyColor = torsoOutlineRenderer.startColor;
        Color newOpacity = new Color(bodyColor.r, bodyColor.g, bodyColor.b, alpha);
        headSprite.color = newOpacity;
        foreach (LineRenderer renderer in allLineRenderers)
        {
            renderer.startColor = newOpacity;
            renderer.endColor = newOpacity;
        }

        Color torsoColor = torsoRenderer.startColor;
        Color newTorsOpacity = new Color(torsoColor.r, torsoColor.g, torsoColor.b, alpha);
        torsoRenderer.startColor = torsoColor;
        torsoRenderer.endColor = torsoColor;

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
        Debug.Log("AWAKE");
        InitGameObjects();
        InitColliderList();
        SetColliders("combat");
        InitRigidbody2DListAndProperties();
        InitHinges();
        InitLineRenderers();
        InitTransforms();

        InitTrailRenderers();

        HideJointsAndStances();

        hp = 10;
        maxhp = 10;
        speedMultiplier = 1f;
        powerMultiplier = 1f;

        // defaults to enemy tags
        fighterHead.tag = "Enemy";
        torsoTop.tag = "Enemy";
        torsoBottom.tag = "Enemy";

        controlsEnabled = true;
        facingRight = true;

        defaultMoveSpeed = 4f / 60f; // x units per 60 frames
        moveSpeed = defaultMoveSpeed * speedMultiplier;
        reach = .75f;
    }
    void Start()
    {
        InitHealthAndEnergyBar(); //THIS NEEDS TO BE IN START, NOT AWAKE
        UpdateDefaultStancePositions(); // need this to set up groundLevel
        groundLevel = Mathf.Min(foot1DefaultVector.y, foot2DefaultVector.y); // keep this in Start()
    }
    void GainEnergy()
    {
        if (Time.frameCount % 60 == 0)
        {
            if (currentEnergy < maxEnergy)
            {
                currentEnergy += energyPerSecond;
                if (currentEnergy > maxEnergy)
                {
                    currentEnergy = maxEnergy;
                }
                UpdateEnergyBar();
            }
        }
    }
    void Update()
    {
        GainEnergy();
        MoveAndDrawBody(); // for some reason important this is called first
        if (controlsEnabled) // if the fighter is not currently in an animation
        {
            MoveTowardsDefaultStance();
        }
    }
    public void SetCharacterType(string typeArg)
    {
        characterType = typeArg.ToLower();
        Debug.Log("fighter is now " + characterType);
    }

    public IEnumerator InitBasedOnCharSettings() // waits a few frames then updates info
    {
        Debug.Log("starting delay before setting character settings");
        for (int i = 0; i < 2; i++)
        {
            yield return null;
        }
        string tag = "";
        if (isPlayer)
        {
            tag = "Player";
        }
        if (isGhost)
        {
            tag = "Ghost";
            SetColor(Color.red);
            SetOpacity(0.2f);
            SetRenderSortingLayer(0);
        }
        if (!isPlayer && !isGhost)
        {
            tag = "Enemy";
            SetColor(Color.red);
            SetRenderSortingLayer(1);
            SetStances("all");
        }
        SetTags(tag);

        if (isPlayer)
        {
            Debug.Log("UpdateBasedOnCharSettings sitrep: hp: " + hp + "/" + maxhp + ", speedMulti,powerMulti = " + speedMultiplier + ", " + powerMultiplier);
        }
        InitClassCombatStats();
    }
    public void InitClassCombatStats()
    {
        // first sets default values
        speedMultiplier = 1f;
        powerMultiplier = 1f;
        maxhp = 10;
        hp = 10;

        // set number stats
        switch (characterType)
        {
            case "acolyte":
                Debug.Log("setting acolyte");
                speedMultiplier = 1f;
                powerMultiplier = 1f;
                maxhp = 10;
                hp = 10;
                break;
            case "brawler":
                Debug.Log("setting brawler");
                speedMultiplier = 0.8f;
                powerMultiplier = 1.4f;
                maxhp = 16;
                hp = 16;
                break;
            case "trickster":
                Debug.Log("setting trickster");
                speedMultiplier = 1.3f;
                powerMultiplier = .8f;
                maxhp = 6;
                hp = 6;
                break;
        }

        if (isPlayer)
        {
            maxhp *= 2;
            hp *= 2;

        }
        Debug.Log("After initializing " + characterType + ", hp: " + hp + "/" + maxhp + ", speedMulti,powerMulti = " + speedMultiplier + ", " + powerMultiplier);
        moveSpeed = defaultMoveSpeed * speedMultiplier;
    }
    void UpdateDefaultStancePositions()
    // used within MoveTowardsDefaultStance()
    {
        float localScaleX = transform.localScale.x;
        hand1DefaultVector =
            stanceHeadTran.position
            + torsoBottom.transform.right * 0.75f * localScaleX
            - torsoBottom.transform.up * 0.5f;
        hand2DefaultVector =
            stanceHeadTran.position
            + torsoBottom.transform.right * 1.5f * localScaleX
            - torsoBottom.transform.up * 0.25f;
        foot1DefaultVector = transform.position - orientedTran.up * 4f - orientedTran.right * 1f;
        foot2DefaultVector = transform.position - orientedTran.up * 4f + orientedTran.right * 1f;
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
        float distance = Vector3.Distance(stanceHand1Tran.position, hand1DefaultVector);
        if (distance > 0.1f)
        {
            stanceHand1Tran.LookAt(hand1DefaultVector);
            stanceHand1Tran.position += stanceHand1Tran.forward * Mathf.Max(moveSpeed * distance * 2, moveSpeed);
        }

        distance = Vector3.Distance(stanceHand2Tran.position, hand2DefaultVector);
        if (distance > 0.1f)
        {
            stanceHand2Tran.LookAt(hand2DefaultVector);
            stanceHand2Tran.position += stanceHand2Tran.forward * Mathf.Max(moveSpeed * distance * 2, moveSpeed);
        }

        // feet
        distance = Vector3.Distance(stanceFoot1Tran.position, foot1DefaultVector);
        if (distance > 0.1f)
        {
            stanceFoot1Tran.LookAt(foot1DefaultVector);
            stanceFoot1Tran.position += stanceFoot1Tran.forward * Mathf.Max(moveSpeed * distance * 2, moveSpeed);
        }

        distance = Vector3.Distance(stanceFoot2Tran.position, foot2DefaultVector);
        if (distance > 0.1f)
        {
            stanceFoot2Tran.LookAt(foot2DefaultVector);
            stanceFoot2Tran.position += stanceFoot2Tran.forward * Mathf.Max(moveSpeed * distance * 2, moveSpeed);
        }
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
            stanceHand2Tran.position = lowerArm1Tran.position;
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

        Vector3[] torsoPoints = {
            jointNeckTran.position,
            (jointRib1Tran.position + jointRib2Tran.position)/2f,
            stanceTorsoBotTran.position,
        };
        torsoRenderer.SetPositions(torsoPoints);

        Vector3[] arm1points = {
            jointShoulder1Tran.position,
            upperArm1Tran.position,
            lowerArm1Tran.position
            };
        arm1Renderer.SetPositions(arm1points);

        Vector3[] arm2points = {
            jointShoulder2Tran.position,
            upperArm2Tran.position,
            lowerArm2Tran.position
            };
        arm2Renderer.SetPositions(arm2points);

        Vector3[] leg1points = {
            jointPelvis1Tran.position,
            jointKnee1Tran.position,
            stanceFoot1Tran.position
            };
        leg1Renderer.SetPositions(leg1points);

        Vector3[] leg2points = {
            jointPelvis2Tran.position,
            jointKnee2Tran.position,
            stanceFoot2Tran.position
            };
        leg2Renderer.SetPositions(leg2points);

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
            controlsEnabled = false;
            EnableGravity(true);
            EqualizeBodyPartMass(true);
            SetColliders("ragdoll");
            SetStances("none");
        }
        else
        {
            //Debug.Log("ragdoll set false");
            controlsEnabled = true;
            EnableGravity(false);
            EqualizeBodyPartMass(false);
            SetColliders("combat");
            SetStances("combat");
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

        float percentage = ((float)hp) / ((float)maxhp);
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
        if (currentEnergy <= 0)
        {
            energyBar.transform.localScale = new Vector3(0f, 0f, 0f);
            return;
        }

        float percentage = ((float)currentEnergy) / ((float)maxEnergy);
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
        transform.position += Vector3.Normalize(direction) * moveSpeed / 2f;
    }
    IEnumerator Step(int directionArg) // WIP 
    {
        float stepDistance = 0.5f;
        float stepTime = 0.15f;
        float delay = stepTime / 2f;
        int delayFrames = (int)Mathf.Ceil(delay * 60f);
        string directionInput = "";
        string firstFoot = "";
        string secondFoot = "";

        if (directionArg > 0)
        {
            directionInput = "forward";
            firstFoot = "front";
            secondFoot = "back";
        }
        else
        {
            directionInput = "backward";
            firstFoot = "back";
            secondFoot = "front";
        }
        StartCoroutine(IndivLimbStep("head", directionInput, stepDistance, stepTime * 1.5f)); // re-enables controls when finished
        StartCoroutine(IndivLimbStep(firstFoot, directionInput, stepDistance, stepTime));
        for (int i = 0; i < delayFrames; i++)
        {
            yield return null;
        }
        StartCoroutine(IndivLimbStep(secondFoot, directionInput, stepDistance, stepTime));
    }
    IEnumerator IndivLimbStep(string limb, string forwardOrBackward, float moveDistance, float time) // WIP
    {
        Transform limbStanceTran = null;
        if (limb == "front")
        {
            limbStanceTran = stanceFoot2Tran;
        }
        if (limb == "back")
        {
            limbStanceTran = stanceFoot1Tran;
        }
        if (limb == "head")
        {
            limbStanceTran = stanceHeadTran;
        }
        float movingSpeed = moveDistance / time;
        if (forwardOrBackward == "forward")
        {
            limbStanceTran.position += orientedTran.right * movingSpeed;
        }
        if (forwardOrBackward == "backward")
        {
            limbStanceTran.position += orientedTran.right * -1f * movingSpeed;
        }
        yield return null;
        // on head movement completion, change the transform and return to monke
        if (limb == "head")
        {
            controlsEnabled = true;
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
        float playerX = transform.position.x;
        float playerY = transform.position.y;
        float playerHeadX = stanceHeadTran.position.x;
        float playerHeadY = stanceHeadTran.position.y;

        switch (direction)
        {
            case 1:
                if (playerHeadY + moveSpeed < playerY + reach)
                {
                    stanceHeadTran.position += Vector3.up * moveSpeed;
                }
                break;
            case 2:
                if (playerHeadY - moveSpeed > playerY - reach)
                {
                    stanceHeadTran.position += Vector3.down * moveSpeed;
                }
                break;
            case 3:
                if (playerHeadX - moveSpeed > playerX - reach)
                {
                    stanceHeadTran.position += Vector3.left * moveSpeed;
                }
                break;
            case 4:
                if (playerHeadX + moveSpeed < playerX + reach)
                {
                    stanceHeadTran.position += Vector3.right * moveSpeed;
                }
                break;
        }
    }
    public void MoveHead(Vector3 direction)
    {
        stanceHeadTran.position += Vector3.Normalize(direction) * moveSpeed;
    }
    public void MoveHeadTowardsSector(int sector)
    {
        MoveHead(GetHeadDirectionToSector(sector));
    }
    public bool IsHeadWithinSectors() // checks if head is within boundaries
    {
        if (stanceHeadTran.position.x > transform.position.x + reach || stanceHeadTran.position.y > transform.position.y + reach)
        {
            return false;
        }
        if (stanceHeadTran.position.x < transform.position.x - reach || stanceHeadTran.position.y < transform.position.y - reach)
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
        foreach (var hinge in allHinges)
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
        controlsEnabled = false;

        float directionMultiplier = orientedTran.position.x - stanceHeadTran.position.x;
        Vector3 movementVector = transform.right * directionMultiplier * moveSpeed;
        while (Mathf.Abs(transform.position.x - stanceHeadTran.position.x) > 0.1f)
        {
            stanceHand1Tran.position += movementVector;
            stanceHand2Tran.position += movementVector;
            stanceHeadTran.position += movementVector;
            yield return null;
        }
        SwapHingeAngles();
        TurnBody();
        controlsEnabled = true;
        //Debug.Log("facing right: " + facingRight);
    }
    public void TurnTo(string direction)
    {
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
    public void InterruptAllAnimations()
    { // break animation coroutines
        StopAllCoroutines();
    }
    public void Attack(string attackType) // attackType = "arms" or "legs"
    {
        string[] sectors = {
        "bottom back", "bottom", "bottom forward",
        "center back", "true center", "center forward",
        "top back", "top", "top forward"
        };
        controlsEnabled = false;

        int sector = GetHeadSector();
        //Debug.Log("Attacking from " + transform.position.x + "," + transform.position.y);
        //Debug.Log("Head at " + stanceHeadTran.position.x + "," + stanceHeadTran.position.y);
        switch (attackType) // attackType is either arms or legs attack
        {
            case "arms":
                switch (sector)
                {
                    case 0: // bottom back
                        StartCoroutine(Uppercut());
                        break;
                    case 1: // bottom 
                        StartCoroutine(Uppercut());
                        break;
                    case 2: // bottom forward
                        StartCoroutine(Uppercut());
                        break;
                    case 3: // center back
                        StartCoroutine(Hook());
                        break;
                    case 4: // center 
                        StartCoroutine(Hook());
                        break;
                    case 5: // center forward
                        StartCoroutine(JabCombo("aggressive"));
                        break;
                    case 6: // top back
                        StartCoroutine(Hook());
                        break;
                    case 7:  // top middle
                        StartCoroutine(Hook());
                        break;
                    case 8: // top forward
                        StartCoroutine(JabCombo("defensive"));
                        break;
                }
                break;
            case "legs":
                switch (sector)
                {
                    case 0: // bottom back
                        StartCoroutine(JumpingFrontKick());
                        break;
                    case 1: // bottom 
                        StartCoroutine(JumpingFrontKick());
                        break;
                    case 2: // bottom forward
                        StartCoroutine(JumpingFrontKick());
                        break;
                    case 3: // center back
                        StartCoroutine(FrontKick("grounded"));
                        break;
                    case 4: // center 
                        StartCoroutine(RoundhouseKick());
                        break;
                    case 5: // center forward
                        StartCoroutine(RoundhouseKick());
                        break;
                    case 6: // top back
                        StartCoroutine(FrontKick("grounded"));
                        break;
                    case 7:  // top middle
                        StartCoroutine(RoundhouseKick());
                        break;
                    case 8: // top forward
                        StartCoroutine(RoundhouseKick());
                        break;
                }
                break;
        }
        //Debug.Log(sectors[sector] + " attack");
    }
    private void StrikeThisLocation(int power, Vector3 targetLocation, Vector3 startOfBodyPart, GameObject strikingStanceObject, GameObject limbObject, float xScale, float yScale) // creates an AttackArea, be it enemy or friendly
    // if it is an enemy attack, has 1 second delay before damage
    // if it is a player attack, immediately does damage
    {
        if (!isGhost && !isPlayer)
        {
            return;
        }

        GameObject newWarning = Instantiate(
            AttackWarningPrefab,
            (targetLocation + startOfBodyPart) / 2f,
            transform.rotation);

        AttackAreaScript newWarningScript = newWarning.GetComponent<AttackAreaScript>();
        newWarningScript.creator = this.gameObject;
        newWarningScript.strikeDirection = targetLocation - startOfBodyPart;

        Vector3 angles = limbObject.transform.eulerAngles;
        newWarning.transform.eulerAngles = new Vector3(0f, 0f, angles.z + 90f);
        newWarning.transform.localScale = new Vector3(xScale, yScale, 1f);

        newWarningScript.attackDamage = power;

        if (isGhost) // is the ghost of an enemy, creates visble attack area and warning, invokes creation of visible warning after framesUntilStrike frames
        {
            newWarningScript.creatorType = "enemy";
            return;
        }

        if (isPlayer)
        { // is the player, creates invisible attack area that checks for collision with enemies
            newWarningScript.lifespan = 1;
            newWarningScript.creatorType = "player";
            newWarningScript.UpdateSprites();
            return;
        }
    }
    IEnumerator KeepHandsInDefaultStance()
    {
        while (true)
        {
            UpdateDefaultStancePositions();
            lowerArm1Tran.position = hand1DefaultVector;
            lowerArm2Tran.position = hand2DefaultVector;
            stanceHand1Tran.position = hand1DefaultVector;
            stanceHand2Tran.position = hand2DefaultVector;
            yield return null;
        }
    }
    public Vector3 GetAttackVector(string attackName)
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
            case "jabdefensive":
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
                    stanceHand2Tran.position
                    + orientedTran.up * 2f
                    + orientedTran.right * 1f;
                break;
            case "roundhousekick":
                output =
                    jointPelvis2Tran.position
                    + orientedTran.right * 4f;
                break;
            case "frontkick":
                break;
            case "jumpingfrontkick":
                break;
        }
        return output;
    }
    IEnumerator Hook()
    {
        int energyCost = (int)(10f * powerMultiplier);
        int power = (int)(6f * powerMultiplier);
        if (currentEnergy < energyCost)
        {
            controlsEnabled = true;
            yield break;
        }
        else
        {
            currentEnergy -= energyCost;
            UpdateEnergyBar();
        }
        float timeTaken = .2f / speedMultiplier; //seconds
        int framesTaken = (int)(timeTaken * 60);
        Vector3 attackTarget = GetAttackVector("Hook");

        yield return attackTarget;
        float distance = Vector3.Distance(attackTarget, stanceHand1Tran.position);
        float distancePerFrame = distance / framesTaken;

        // throw punch animation
        for (int i = 0; i < framesTaken; i++)
        {
            stanceHeadTran.position += orientedTran.right * distancePerFrame * .4f;

            // punching hand
            stanceHand1Tran.LookAt(attackTarget);
            stanceHand1Tran.position += stanceHand1Tran.forward * distancePerFrame;

            // guard hand
            stanceHand2Tran.LookAt(stanceHeadTran.position + orientedTran.right * 1f);
            stanceHand2Tran.position += stanceHand2Tran.forward * distancePerFrame * 0.5f;
            upperArm1Tran.position = jointElbow1Tran.position;
            yield return null;
        }
        StrikeThisLocation(power, attackTarget, jointElbow1Tran.position, stanceHand1, lowerArm1, 1f, 1f);

        // return to default stance fast before regaining control
        while (Vector3.Distance(stanceHand2Tran.position, hand2DefaultVector) > 0.1f)
        {
            MoveTowardsDefaultStance();
            MoveTowardsDefaultStance();
            yield return null;
        }
        controlsEnabled = true;
    }
    IEnumerator JabCombo(string type) // type = "defensive" or "aggressive". defensive is a 2 hit combo, aggressive is a single far jab
    {
        int energyCost = (int)(8f * powerMultiplier);
        int power = (int)(3f * powerMultiplier);
        switch (type)
        {
            case "defensive":
                energyCost = 8;
                break;
            case "aggressive":
                energyCost = 25;
                break;
        }
        if (currentEnergy < energyCost)
        {
            controlsEnabled = true;
            yield break;
        }
        else
        {
            currentEnergy -= energyCost;
            UpdateEnergyBar();
        }

        Vector3 attackTarget = GetAttackVector("Jab" + type);
        float timeTaken = .12f / speedMultiplier; //seconds
        int framesTaken = (int)(timeTaken * 60);
        float hand2DistancePerFrame = Mathf.Abs(stanceHand2Tran.position.x - attackTarget.x) / framesTaken;
        float headDistancePerFrame = 0.3f * hand2DistancePerFrame;

        Vector3 headMoveVector = orientedTran.right * headDistancePerFrame;
        if (type == "defensive")
            headMoveVector = -1f * orientedTran.right * headDistancePerFrame;

        // jab animation
        for (int i = 0; i < framesTaken; i++)
        {
            stanceHeadTran.position += headMoveVector;

            //moves jabbing hand 
            stanceHand2Tran.LookAt(attackTarget);
            stanceHand2Tran.position += stanceHand2Tran.forward * hand2DistancePerFrame;

            // moves nonjab hand to guard
            stanceHand1Tran.LookAt(stanceHeadTran.position + orientedTran.right * 1f);
            stanceHand1Tran.position += stanceHand1Tran.forward * hand2DistancePerFrame;
            upperArm1Tran.position = jointElbow1Tran.position;
            yield return null;
        }
        StrikeThisLocation(power, attackTarget, jointElbow2Tran.position, stanceHand2, lowerArm2, 1f, 1f);

        // pull jabbing hand back
        while (Vector3.Distance(stanceHand2Tran.position, hand2DefaultVector) > 0.1f)
        {
            MoveTowardsDefaultStance();
            MoveTowardsDefaultStance();
            yield return null;
        }

        // start hook if defensive
        if (type == "defensive")
        {
            yield return StartCoroutine(Hook());
            yield return null;
        }
        if (type == "aggressive") // end animation if aggressive
        {
            controlsEnabled = true;
        }
        //Debug.Log("controls re-enabled");
    }
    IEnumerator RoundhouseKick()
    {
        int energyCost = (int)(30f * powerMultiplier);
        if (currentEnergy < energyCost)
        {
            controlsEnabled = true;
            yield break;
        }
        else
        {
            currentEnergy -= energyCost;
            UpdateEnergyBar();

        }
        int power = (int)(8f * powerMultiplier);
        float timeTaken = .25f / speedMultiplier; //seconds
        int framesTaken = (int)(timeTaken * 60);

        Transform kickingFootTran = stanceFoot1Tran;

        stanceTorsoBotActive = true;
        Vector3 origHeadPosition = stanceHeadTran.position;
        Vector3 origBotTorsoPosition = torsoBottom.transform.position;

        Vector3 attackTarget = GetAttackVector("RoundhouseKick");


        float footMovingDistance = Vector3.Distance(kickingFootTran.position, attackTarget);
        float distancePerFrame = footMovingDistance / framesTaken;

        Vector3 torsoMoveDistance = orientedTran.right * distancePerFrame * 0.15f;

        // kick animation
        for (int i = 0; i < framesTaken; i++)
        {
            stanceHeadTran.position += torsoMoveDistance * .5f;
            stanceTorsoBotTran.position += torsoMoveDistance;

            kickingFootTran.LookAt(attackTarget);
            kickingFootTran.position += kickingFootTran.forward * distancePerFrame;

            // guarding
            Vector3 hand1Guard = stanceHeadTran.position + orientedTran.right * 1f;
            float distance = Vector3.Distance(hand1Guard, stanceHand1Tran.position);
            stanceHand1Tran.LookAt(hand1Guard);
            stanceHand1Tran.position += stanceHand1Tran.forward * Mathf.Min(distancePerFrame * distance, moveSpeed);

            yield return null;
        }
        StrikeThisLocation(power, attackTarget, jointKnee1Tran.position, stanceFoot1, calf1, 2f, 1f);
        stanceTorsoBotActive = false;

        while (Vector3.Distance(kickingFootTran.position, foot1DefaultVector) > .25f)
        {
            MoveTowardsDefaultStance();
            yield return null;
        }
        controlsEnabled = true;
        //Debug.Log("controls re-enabled");
    }
    IEnumerator FrontKick(string type) // type = "grounded" or "flying"
    {
        int energyCost = (int)(30f * powerMultiplier);
        int power = 0; // to be set in below switch statement
        bool grounded = true;
        switch (type)
        {
            case "grounded":
                energyCost = 30;
                break;
            case "flying":
                energyCost = 0;
                break;
        }
        if (currentEnergy < energyCost)
        {
            controlsEnabled = true;
            yield break;
        }
        else
        {
            currentEnergy -= energyCost;
            UpdateEnergyBar();
        }

        switch (type)
        {
            case "grounded":
                power = (int)(7f * powerMultiplier);
                grounded = true;
                stanceTorsoBotActive = true;
                break;
            case "flying":
                power = (int)(10f * powerMultiplier);
                grounded = false;
                SetStances("none");
                stanceFoot2Active = true;
                stanceHeadActive = true;
                stanceHand1Active = true;
                stanceHand2Active = true;
                break;
        }
        float range = 4.5f;
        float raiseFootTime = .15f;

        float timeTaken = .25f / speedMultiplier; //seconds
        if (type == "flying")
        {
            timeTaken /= speedMultiplier;
        }

        float kickTime = .2f; //seconds
        int raiseFootFrames = (int)(raiseFootTime * 60);
        int kickFramesTaken = (int)(kickTime * 60);

        Transform kickingFootTran = stanceFoot2Tran;

        stanceTorsoBotActive = true;
        Vector3 origHeadPosition = stanceHeadTran.position;
        Vector3 origBotTorsoPosition = torsoBottom.transform.position;
        Vector3 attackTarget = jointPelvis2Tran.position + orientedTran.right * range;

        float raiseLegDistance = Mathf.Abs(kickingFootTran.position.y - attackTarget.y);

        float kickDistance = Vector3.Distance(kickingFootTran.position, attackTarget);
        float kickDistancePerFrame = kickDistance / kickFramesTaken;

        Vector3 torsoMoveDistance = orientedTran.right * kickDistancePerFrame * 0.25f;

        // raise front leg
        Vector3 origKickFootPos = kickingFootTran.position;
        Vector3 raisedKickPosition = jointPelvis2Tran.position + orientedTran.right * 0.5f;
        float timeElapsed = 0;
        for (int i = 0; i < raiseFootFrames; i++)
        {
            //kickingFootTran.position += orientedTran.up * kickDistancePerFrame;
            kickingFootTran.position = Vector3.Lerp(origKickFootPos, raisedKickPosition, timeElapsed / raiseFootFrames);
            timeElapsed++;
            yield return null;
        }
        // extend and kick
        for (int i = 0; i < kickFramesTaken; i++)
        {
            attackTarget = jointPelvis2Tran.position + orientedTran.right * range;

            if (grounded)
            {
                stanceHeadTran.position += torsoMoveDistance;
                stanceTorsoBotTran.position += torsoMoveDistance;
            }
            else
            {
                stanceHeadTran.position += new Vector3(
                    -1.5f * transform.localScale.x * moveSpeed,
                    0f,
                    0f
                );
            }


            kickingFootTran.LookAt(attackTarget);
            kickingFootTran.position += kickingFootTran.forward * kickDistancePerFrame;

            // guarding
            Vector3 hand1Guard = stanceHeadTran.position + orientedTran.right * 1f;
            float distance = Vector3.Distance(hand1Guard, stanceHand1Tran.position);
            stanceHand1Tran.LookAt(hand1Guard);
            stanceHand1Tran.position += stanceHand1Tran.forward * Mathf.Max(kickDistancePerFrame * distance, moveSpeed);
            yield return null;
        }
        SetStances("combat");
        StrikeThisLocation(power, kickingFootTran.position, jointKnee2Tran.position, stanceFoot2, calf2, 2f, 1f);

        while (Vector3.Distance(kickingFootTran.position, foot2DefaultVector) > .25f)
        {
            MoveTowardsDefaultStance();
            yield return null;
        }
        controlsEnabled = true;
        //Debug.Log("controls re-enabled");
    }
    IEnumerator JumpingFrontKickPart2(float jumpSpeed)
    {
        IEnumerator handStanceCoroutine = KeepHandsInDefaultStance();
        StartCoroutine(handStanceCoroutine);
        controlsEnabled = false;
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
        StartCoroutine(FrontKick("flying"));
        controlsEnabled = false;

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
    IEnumerator JumpingFrontKick()
    {
        int energyCost = 65;
        // no power listed here, rather it is listed on the front kick
        if (currentEnergy < energyCost)
        {
            controlsEnabled = true;
            yield break;
        }
        else
        {
            currentEnergy -= energyCost;
            UpdateEnergyBar();
        }

        controlsEnabled = false;
        int sector = GetHeadSector();

        // move head to top forward section
        Vector3 headTargetPosition = orientedTran.position + orientedTran.right + orientedTran.up;
        Vector3 initHeadPosition = fighterHead.transform.position;
        float headMoveTime = 20f / speedMultiplier;
        IEnumerator keepHandsInPlace = KeepHandsInDefaultStance();
        StartCoroutine(keepHandsInPlace);
        for (int i = 0; i < (int)headMoveTime; i++)
        {
            stanceHeadTran.position = Vector3.Lerp(initHeadPosition, headTargetPosition, ((float)(i)) / headMoveTime);
            yield return null;
        }
        StopCoroutine(keepHandsInPlace);

        // jump and kick at the same time
        yield return StartCoroutine(JumpingFrontKickPart2(10f));
        yield return null;
    }
    IEnumerator Uppercut()
    {
        int energyCost = (int)(15f * powerMultiplier);
        int power = (int)(6f * powerMultiplier);
        if (currentEnergy < energyCost)
        {
            controlsEnabled = true;
            yield break;
        }
        float timeTaken = .2f / speedMultiplier; //seconds
        int framesTaken = (int)(timeTaken * 60);
        Vector3 attackTarget = GetAttackVector("Uppercut");

        yield return attackTarget;
        float distance = Vector3.Distance(attackTarget, stanceHand1Tran.position);
        float distancePerFrame = distance / framesTaken;

        // throw punch animation
        for (int i = 0; i < framesTaken; i++)
        {
            stanceHeadTran.position += orientedTran.up * distancePerFrame * .4f;
            stanceHeadTran.position += orientedTran.right * distancePerFrame * .25f;

            // punching hand
            stanceHand1Tran.LookAt(attackTarget);
            stanceHand1Tran.position += stanceHand1Tran.forward * distancePerFrame;

            // guard hand
            stanceHand2Tran.LookAt(stanceHeadTran.position + orientedTran.right * 1f);
            stanceHand2Tran.position += stanceHand2Tran.forward * distancePerFrame * 0.5f;
            upperArm1Tran.position = jointElbow1Tran.position;
            yield return null;
        }
        StrikeThisLocation(power, attackTarget, jointElbow1Tran.position, stanceHand1, lowerArm1, 1f, 1f);

        // return to default stance fast before regaining control
        while (Vector3.Distance(stanceHand2Tran.position, hand2DefaultVector) > 0.1f)
        {
            MoveTowardsDefaultStance();
            MoveTowardsDefaultStance();
            yield return null;
        }
        controlsEnabled = true;
    }
}
