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
    public Transform orientedTran;
    public float speed;
    public int hp;
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

    public GameObject HeadSpriteObject;
    SpriteRenderer HeadSprite;

    // public List<string> movementQueue = new List<string>();

    public float reach; // distance the fighterHead can move from fighter object position

    public bool controlsEnabled = true; // enabled, fighter can move head around + limbs move to default positions. disable when in an animation

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
    private List<LineRenderer> allLineRenderers = new List<LineRenderer>();
    //private LineRenderer[] allLineRenderers = new LineRenderer[6];

    public void SetTags(string tag)
    {
        fighterHead.tag = tag;
        HeadSpriteObject.tag = tag;
        torsoTop.tag = tag;
        torsoBottom.tag = tag;
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
    public void ChangeColor(Color newColor)
    {
        foreach (LineRenderer renderer in allLineRenderers)
        {
            renderer.startColor = newColor;
            renderer.endColor = newColor;
        }
        Color torsoColor = new Color((newColor.r + 1f) / 2f, (newColor.g + 1f) / 2f, (newColor.b + 1f) / 2f, 1);
        torsoRenderer.startColor = torsoColor;
        torsoRenderer.endColor = torsoColor;

        HeadSprite.color = newColor;
    }
    public void ChangeOpacity(float alpha) // 1f = opaque, 0f = transparent
    {
        Color bodyColor = torsoOutlineRenderer.startColor;
        Color newOpacity = new Color(bodyColor.r, bodyColor.g, bodyColor.b, alpha);
        HeadSprite.color = newOpacity;
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
    public void ChangeRenderSortingLayer(int layer)
    {
        foreach (LineRenderer renderer in allLineRenderers)
        {
            renderer.sortingOrder = layer;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        HideJointsAndStances();
        Application.targetFrameRate = 60; // sets frame rate to 60fps, i will likely move this to another script later

        fighterRB = this.gameObject.GetComponent<Rigidbody2D>();
        InitHinges();
        InitLineRenderers();
        InitTransforms();
        InitTrailRenderers();
        UpdateDefaultStancePositions(); // need this to set up groundLevel

        // defaults to enemy tags
        fighterHead.tag = "Enemy";
        torsoTop.tag = "Enemy";
        torsoBottom.tag = "Enemy";

        groundLevel = Mathf.Min(foot1DefaultVector.y, foot2DefaultVector.y);

        expectedElbowDistanceToNeck1 = Vector3.Distance(jointElbow1Tran.position, jointShoulder1Tran.position);
        expectedElbowDistanceToNeck2 = Vector3.Distance(jointElbow2Tran.position, jointShoulder2Tran.position);

        controlsEnabled = true;
        HeadSprite = HeadSpriteObject.GetComponent<SpriteRenderer>();
        facingRight = true;
        hp = 100;
        speed = 4f / 60f; // x units per 60 frames
        reach = 1f;
    }

    public void UpdateTags()
    {
        string tag = "";
        if (isPlayer)
        {
            tag = "Player";
        }
        if (isGhost)
        {
            tag = "Ghost";
            fighterHead.GetComponent<BoxCollider2D>().enabled = false;
            HeadSpriteObject.GetComponent<BoxCollider2D>().enabled = false;
            torsoTop.GetComponent<BoxCollider2D>().enabled = false;
            torsoBottom.GetComponent<BoxCollider2D>().enabled = false;
        }
        if (!isPlayer && !isGhost)
        {
            tag = "Enemy";
        }
        fighterHead.tag = tag;
        HeadSpriteObject.tag = tag;
        torsoTop.tag = tag;
        torsoBottom.tag = tag;
    }
    void UpdateDefaultStancePositions()
    // used within MoveTowardsDefaultStance()
    {
        hand1DefaultVector = stanceHeadTran.position + orientedTran.right * 0.25f - orientedTran.up * 1f;
        hand2DefaultVector = stanceHeadTran.position + orientedTran.right * 1.5f - orientedTran.up * 0.5f;
        foot1DefaultVector = transform.position - orientedTran.up * 4f - orientedTran.right * 1f;
        foot2DefaultVector = transform.position - orientedTran.up * 4f + orientedTran.right * 1f;
    }
    void MoveTowardsDefaultStance()
    //moves at speed towards default positions of hands and feet
    {
        float fighterX = transform.position.x;
        float fighterY = transform.position.y;
        float fighterHeadX = transform.position.x + stanceHeadTran.position.x;
        float fighterHeadY = transform.position.y + stanceHeadTran.position.y;

        // these need to be transform.right not orientedTran.right 
        if (stanceHeadTran.position.x > fighterX + reach)
        {
            stanceHeadTran.position -= transform.right * speed;
        }
        if (stanceHeadTran.position.x < fighterX - reach)
        {
            stanceHeadTran.position += transform.right * speed;
        }
        if (stanceHeadTran.position.y < fighterY - reach)
        {
            stanceHeadTran.position += transform.up * speed;
        }
        if (stanceHeadTran.position.y > fighterY + reach)
        {
            stanceHeadTran.position -= transform.up * speed;
        }

        UpdateDefaultStancePositions();

        // hands
        float distance = Vector3.Distance(stanceHand1Tran.position, hand1DefaultVector);
        if (distance > 0.1f)
        {
            stanceHand1Tran.LookAt(hand1DefaultVector);
            stanceHand1Tran.position += stanceHand1Tran.forward * Mathf.Max(speed * distance * 2, speed);
        }

        distance = Vector3.Distance(stanceHand2Tran.position, hand2DefaultVector);
        if (distance > 0.1f)
        {
            stanceHand2Tran.LookAt(hand2DefaultVector);
            stanceHand2Tran.position += stanceHand2Tran.forward * Mathf.Max(speed * distance * 2, speed);
        }

        // feet
        distance = Vector3.Distance(stanceFoot1Tran.position, foot1DefaultVector);
        if (distance > 0.1f)
        {
            stanceFoot1Tran.LookAt(foot1DefaultVector);
            stanceFoot1Tran.position += stanceFoot1Tran.forward * Mathf.Max(speed * distance * 2, speed);
        }

        distance = Vector3.Distance(stanceFoot2Tran.position, foot2DefaultVector);
        if (distance > 0.1f)
        {
            stanceFoot2Tran.LookAt(foot2DefaultVector);
            stanceFoot2Tran.position += stanceFoot2Tran.forward * Mathf.Max(speed * distance * 2, speed);
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
    public void EnableAllStances(){
        stanceHeadActive =true;
        stanceTorsoTopActive =true;
        stanceTorsoBotActive =true;
        stanceHand1Active =true;
        stanceHand2Active =true;
        stanceFoot1Active =true;
        stanceFoot2Active =true;
    }
    public void Move(Vector3 direction)
    {
        transform.position += Vector3.Normalize(direction) * speed /2f;
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
                if (playerHeadY < playerY + reach)
                {
                    stanceHeadTran.position += Vector3.up * speed;
                }
                break;
            case 2:
                if (playerHeadY > playerY - reach)
                {
                    stanceHeadTran.position += Vector3.down * speed;
                }
                break;
            case 3:
                if (playerHeadX > playerX - reach)
                {
                    stanceHeadTran.position += Vector3.left * speed;
                }
                break;
            case 4:
                if (playerHeadX < playerX + reach)
                {
                    stanceHeadTran.position += Vector3.right * speed;
                }
                break;
        }
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

    public void TurnBody(){
        float targetScale = 0 - transform.localScale.x; // can't use directionMultiplier for this!!!
        transform.localScale = new Vector3(targetScale, 1, 1);
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
    }
    IEnumerator GoToCenterXAndTurn()
    {
        controlsEnabled = false;
        
        float directionMultiplier = orientedTran.position.x - stanceHeadTran.position.x;
        Vector3 movementVector = transform.right * directionMultiplier * speed;
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

    void Update()
    {
        MoveAndDrawBody(); // for some reason important this is called first
        if (controlsEnabled) // if the fighter is not currently in an animation
        {
            MoveTowardsDefaultStance();
        }

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

    public void Attack(string attackType) // attackType = "arms" or "legs"
    {
        string[] sectors = {
        "bottom back", "bottom", "bottom forward",
        "center back", "true center", "center forward",
        "top back", "top", "top forward"
        };
        controlsEnabled = false;

        int sector = GetHeadSector();
        switch (attackType) // attackType is either arms or legs attack
        {
            case "arms":
                switch (sector)
                {
                    case 0: // bottom back
                        StartCoroutine(Hook());
                        break;
                    case 1: // bottom 
                        StartCoroutine(Hook());
                        break;
                    case 2: // bottom forward
                        StartCoroutine(JabCombo("aggressive"));
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
                        StartCoroutine(FrontKick());
                        break;
                    case 4: // center 
                        StartCoroutine(RoundhouseKick());
                        break;
                    case 5: // center forward
                        StartCoroutine(RoundhouseKick());
                        break;
                    case 6: // top back
                        StartCoroutine(FrontKick());
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

    private void StrikeThisLocation(Vector3 targetLocation) // creates an AttackArea, be it enemy or friendly
    {
        if (!isGhost && !isPlayer)
        {
            return;
        }
        GameObject newWarning = Instantiate(
            AttackWarningPrefab,
            targetLocation,
            transform.rotation);
        AttackAreaScript newWarningScript = newWarning.GetComponent<AttackAreaScript>();

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
    IEnumerator Hook()
    {
        float sideRange = 4f;
        float timeTaken = .2f; //seconds
        int framesTaken = (int)(timeTaken * 60);
        Vector3 attackTarget =
                jointShoulder1Tran.position
                + orientedTran.right * sideRange
                - transform.up * .25f;

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
        StrikeThisLocation(attackTarget);

        // return to default stance fast
        while (Vector3.Distance(stanceHand2Tran.position, hand2DefaultVector) > 0.1f)
        {
            MoveTowardsDefaultStance();
            MoveTowardsDefaultStance();
            yield return null;
        }
        controlsEnabled = true;
    }
    IEnumerator JabCombo(string type)
    {
        float punchDistance = 1.8f;
        if (type == "defensive")
            punchDistance = 1.2f;

        Vector3 attackTarget = stanceHand2Tran.position + orientedTran.right * punchDistance;
        float timeTaken = .12f; //seconds
        int framesTaken = (int)(timeTaken * 60);
        float hand2DistancePerFrame = punchDistance / framesTaken;
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
        StrikeThisLocation(attackTarget);

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
        float range = 4f;
        float timeTaken = .25f; //seconds
        int framesTaken = (int)(timeTaken * 60);

        Transform kickingFootTran = stanceFoot1Tran;

        stanceTorsoBotActive = true;
        Vector3 origHeadPosition = stanceHeadTran.position;
        Vector3 origBotTorsoPosition = torsoBottom.transform.position;

        Vector3 attackTarget = jointPelvis2Tran.position + orientedTran.right * range;


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
            stanceHand1Tran.position += stanceHand1Tran.forward * Mathf.Max(distancePerFrame * distance, speed);
            yield return null;
        }
        StrikeThisLocation(attackTarget);
        stanceTorsoBotActive = false;

        while (Vector3.Distance(kickingFootTran.position, foot1DefaultVector) > .25f)
        {
            MoveTowardsDefaultStance();
            yield return null;
        }
        controlsEnabled = true;
        //Debug.Log("controls re-enabled");
    }
    IEnumerator FrontKick() // type = "normal" or "flying"
    {
        float range = 4.5f;
        float raiseFootTime = .15f;
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
            stanceHeadTran.position += torsoMoveDistance;
            stanceTorsoBotTran.position += torsoMoveDistance;

            kickingFootTran.LookAt(attackTarget);
            kickingFootTran.position += kickingFootTran.forward * kickDistancePerFrame;

            // guarding
            Vector3 hand1Guard = stanceHeadTran.position + orientedTran.right * 1f;
            float distance = Vector3.Distance(hand1Guard, stanceHand1Tran.position);
            stanceHand1Tran.LookAt(hand1Guard);
            stanceHand1Tran.position += stanceHand1Tran.forward * Mathf.Max(kickDistancePerFrame * distance, speed);
            yield return null;
        }
        StrikeThisLocation(kickingFootTran.position);
        stanceTorsoBotActive = false;

        while (Vector3.Distance(kickingFootTran.position, foot2DefaultVector) > .25f)
        {
            MoveTowardsDefaultStance();
            yield return null;
        }
        controlsEnabled = true;
        //Debug.Log("controls re-enabled");
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
        fighterRB.velocity = new Vector2(2f * transform.localScale.x, jumpSpeed);
        fighterRB.gravityScale = 3f;
        stanceFoot1Tran.position += Vector3.up * 0.05f;
        stanceFoot2Tran.position += Vector3.up * 0.05f;
        StopCoroutine(handStanceCoroutine);

        // front kick
        StartCoroutine(FrontKick());
        controlsEnabled = false;

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
        controlsEnabled = false;
        int sector = GetHeadSector();

        // move head to top forward section
        Vector3 headTargetPosition = orientedTran.position + orientedTran.right + orientedTran.up;
        Vector3 initHeadPosition = fighterHead.transform.position;
        float headMoveTime = 20f;
        IEnumerator keepHandsInPlace = KeepHandsInDefaultStance();
        StartCoroutine(keepHandsInPlace);
        for (int i = 0; i < 20; i++)
        {
            stanceHeadTran.position = Vector3.Lerp(initHeadPosition, headTargetPosition, ((float)(i)) / headMoveTime);
            yield return null;
        }
        StopCoroutine(keepHandsInPlace);

        // jump and kick at the same time
        yield return StartCoroutine(JumpingFrontKickPart2(10f));
        controlsEnabled = true;
        yield return null;
    }
}
