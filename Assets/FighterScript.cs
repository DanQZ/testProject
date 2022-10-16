using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class FighterScript : MonoBehaviour
{
    Rigidbody2D fighterRB;
    public float groundLevel;
    public GameObject fighterOrienter;
    private Transform orientedTran;
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
    private Transform stanceHeadTran;
    bool stanceHeadActive = true;

    public GameObject stanceHand1;
    private Transform stanceHand1Tran;
    bool stanceHand1Active = true;
    public GameObject stanceHand2;
    private Transform stanceHand2Tran;
    bool stanceHand2Active = true;

    public GameObject stanceFoot1;
    private Transform stanceFoot1Tran;
    bool stanceFoot1Active = true;
    public GameObject stanceFoot2;
    private Transform stanceFoot2Tran;
    bool stanceFoot2Active = true;
    public GameObject stanceRibs;
    private Transform stanceRibsTran;
    bool stanceRibsActive = false;

    public GameObject stancePelvis;
    private Transform stancePelvisTran;
    bool stancePelvisActive = false;

    public GameObject spriteObject;
    SpriteRenderer fighterSprite;

    // public List<string> movementQueue = new List<string>();

    public float reach; // distance the fighterHead can move from fighter object position

    public bool controlsEnabled = true; // enabled, fighter can move head around + limbs move to default positions. disable when in an animation

    float expectedElbowDistanceToNeck1;
    float expectedElbowDistanceToNeck2;

    private LineRenderer arm1Renderer;
    private LineRenderer arm2Renderer;
    private LineRenderer leg1Renderer;
    private LineRenderer leg2Renderer;
    private LineRenderer TorsoRenderer;

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
    private HingeJoint2D[] allHinges = new HingeJoint2D[10];
    void InitPathFollowers()
    {

    }
    void InitHinges()
    {
        torsoTopHinge = torsoTop.GetComponent<HingeJoint2D>();
        torsoBottomHinge = torsoBottom.GetComponent<HingeJoint2D>();
        upperArm1Hinge = upperArm1.GetComponent<HingeJoint2D>();
        lowerArm1Hinge = lowerArm1.GetComponent<HingeJoint2D>();
        upperArm2Hinge = upperArm2.GetComponent<HingeJoint2D>();
        lowerArm2Hinge = lowerArm2.GetComponent<HingeJoint2D>();
        thigh1Hinge = thigh1.GetComponent<HingeJoint2D>();
        calf1Hinge = calf1.GetComponent<HingeJoint2D>();
        thigh2Hinge = thigh2.GetComponent<HingeJoint2D>();
        calf2Hinge = calf2.GetComponent<HingeJoint2D>();
        allHinges[0] = (torsoTopHinge);
        allHinges[1] = (torsoBottomHinge);
        allHinges[2] = (upperArm1Hinge);
        allHinges[3] = (upperArm2Hinge);
        allHinges[4] = (lowerArm1Hinge);
        allHinges[5] = (lowerArm2Hinge);
        allHinges[6] = (thigh1Hinge);
        allHinges[7] = (thigh2Hinge);
        allHinges[8] = (calf1Hinge);
        allHinges[9] = (calf2Hinge);
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
        stanceRibsTran = stanceRibs.transform;
        stancePelvisTran = stancePelvis.transform;

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

        List<LineRenderer> allLineRenderers = new List<LineRenderer>();

        arm1Renderer = jointShoulder1.GetComponent<LineRenderer>();
        allLineRenderers.Add(arm1Renderer);
        arm2Renderer = jointShoulder2.GetComponent<LineRenderer>();
        allLineRenderers.Add(arm2Renderer);
        leg1Renderer = jointPelvis1.GetComponent<LineRenderer>();
        allLineRenderers.Add(leg1Renderer);
        leg2Renderer = jointPelvis2.GetComponent<LineRenderer>();
        allLineRenderers.Add(leg2Renderer);
        TorsoRenderer = jointNeck.GetComponent<LineRenderer>();
        allLineRenderers.Add(TorsoRenderer);

        foreach (LineRenderer element in allLineRenderers)
        {
            element.startColor = bodyColor;
            element.endColor = bodyColor;
            element.startWidth = thickness;
            element.endWidth = thickness;
            element.positionCount = defaultPositionCount;
            element.useWorldSpace = true;
            element.numCornerVertices = jointVertices;
            element.numCapVertices = jointVertices;
        }
        TorsoRenderer.numCornerVertices = 0;
        TorsoRenderer.positionCount = 7;
        TorsoRenderer.startColor = Color.gray;
        TorsoRenderer.endColor = Color.gray;

        leg1Renderer.startWidth = thickness * 2;
        leg2Renderer.startWidth = thickness * 2;

    }

    // Start is called before the first frame update
    void Start()
    {
        fighterRB = this.gameObject.GetComponent<Rigidbody2D>();
        InitHinges();
        InitLineRenderers();
        InitTransforms();
        UpdateDefaultStancePositions(); // need this to set up groundLevel
        groundLevel = Mathf.Min(foot1DefaultVector.y, foot2DefaultVector.y);

        expectedElbowDistanceToNeck1 = Vector3.Distance(jointElbow1Tran.position, jointShoulder1Tran.position);
        expectedElbowDistanceToNeck2 = Vector3.Distance(jointElbow2Tran.position, jointShoulder2Tran.position);

        controlsEnabled = true;
        fighterSprite = spriteObject.gameObject.GetComponent<SpriteRenderer>();
        facingRight = true;
        hp = 100;
        Application.targetFrameRate = 60; // sets frame rate to 60fps, i will likely move this to another script later
        speed = 5f / 60f; // x units per 60 frames
        reach = 1f;
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

    void GroundedFeet()
    //keeps feet on the ground 
    {
        calf1Tran.position = transform.position + Vector3.down * 4f - orientedTran.right * 1f;
        calf2Tran.position = transform.position + Vector3.down * 4f + orientedTran.right * 1f;
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

        if (stancePelvisActive)
        {
            torsoBottom.transform.position = stancePelvisTran.position;
        }
        else
        {
            stancePelvisTran.position = torsoBottom.transform.position;
        }

        if (stanceRibsActive)
        {
            torsoTop.transform.position = stanceRibsTran.position;
        }
        else
        {
            stanceRibsTran.position = torsoTop.transform.position;
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

        Vector3[] torsoPoints = {
            jointNeckTran.position,
            jointShoulder1Tran.position,
            jointRib1Tran.position,
            jointPelvis1Tran.position,
            jointPelvis2Tran.position,
            jointRib2Tran.position,
            jointShoulder2Tran.position,
            };
        TorsoRenderer.SetPositions(torsoPoints);
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

    void FixHinges()
    {
        float change = 1f;
        if (facingRight)
        {
            change = -1f;
        }
        foreach (var hinge in allHinges)
        {
            JointAngleLimits2D newLimits = hinge.limits;
            newLimits.min = 0 - hinge.limits.min;
            newLimits.max = 0 - hinge.limits.max;
            hinge.limits = newLimits;
        }
    }
    IEnumerator GoToCenterXAndTurn()
    {
        controlsEnabled = false;
        float directionMultipler = transform.position.x - stanceHeadTran.position.x;
        while (Mathf.Abs(transform.position.x - stanceHeadTran.position.x) > 0.25f)
        {
            stanceHeadTran.position += transform.right * directionMultipler * speed;
            yield return null;
        }
        float targetScale = 0-transform.localScale.x; // can't use directionMultiplier for this!!!
        transform.localScale = new Vector3(targetScale, 1 ,1);
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
        controlsEnabled = true;
        Debug.Log("facing right: " + facingRight);
    }
    public void TurnTo(string direction)
    {
        StartCoroutine(GoToCenterXAndTurn());
        FixHinges();
        MoveAndDrawBody();
    }

    void Update()
    {
        MoveAndDrawBody(); // important this is called first
        if (controlsEnabled)
        {
            MoveTowardsDefaultStance();
        }
    }

    // finds what sector the head is in, in order to do a
    int GetHeadSector()
    {
        string[] attacks = {
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
        if (fighterHeadToCenterY < 0-reach / 3) // on bottom side
        {
            ySector = 0;
        }
        if (fighterHeadToCenterY > reach / 3) // on top side
        {
            ySector = 2;
        }

        int sector = ySector * 3 + xSector;
        Debug.Log(attacks[sector] + " attack");
        return sector;
    }

    public void Attack(string attackType)
    {
        controlsEnabled = false;

        int sector = GetHeadSector();
        switch (attackType) // attackType is either arms or legs attack
        {
            case "arms":
                switch (sector)
                {
                    case 0: // bottom left
                        StartCoroutine(Hook());
                        break;
                    case 1: // bottom 
                        StartCoroutine(Hook());
                        break;
                    case 2: // bottom right
                        StartCoroutine(Jab("aggressive"));
                        break;
                    case 3: // center left
                        StartCoroutine(Hook());
                        break;
                    case 4: // center 
                        StartCoroutine(Hook());
                        break;
                    case 5: // center right
                        StartCoroutine(Jab("aggressive"));
                        break;
                    case 6: // top left
                        StartCoroutine(Hook());
                        break;
                    case 7:  // top middle
                        StartCoroutine(Hook());
                        break;
                    case 8: // top right
                        StartCoroutine(Jab("defensive"));
                        break;
                }
                break;
            case "legs":
                switch (sector)
                {
                    case 0: // bottom left
                        StartCoroutine(JumpingFrontKick());
                        break;
                    case 1: // bottom 
                        StartCoroutine(JumpingFrontKick());
                        break;
                    case 2: // bottom right
                        StartCoroutine(JumpingFrontKick());
                        break;
                    case 3: // center left
                        StartCoroutine(FrontKick());
                        break;
                    case 4: // center 
                        StartCoroutine(FrontKick());
                        break;
                    case 5: // center right
                        StartCoroutine(FrontKick());
                        break;
                    case 6: // top left
                        StartCoroutine(FrontKick());
                        break;
                    case 7:  // top middle
                        StartCoroutine(FrontKick());
                        break;
                    case 8: // top right
                        StartCoroutine(FrontKick());
                        break;
                }
                break;
        }
    }
    IEnumerator MoveTransToTarget(Transform transformToBeMoved, Vector3 target, float secondsTaken)
    {
        int framesTaken = (int)(secondsTaken * 60f);
        float distance = Vector3.Distance(target, transformToBeMoved.position);
        float distancePerFrame = distance / framesTaken;
        transformToBeMoved.LookAt(target);
        for (int i = 0; i < framesTaken; i++)
        {
            transformToBeMoved.position += transformToBeMoved.forward * distancePerFrame;
            yield return null;
        }
    }

    IEnumerator Hook()
    {
        float sideRange = 4f;
        float timeTaken = .2f; //seconds
        int framesTaken = (int)(timeTaken * 60);

        GameObject newfighterAttack = Instantiate(
            fighterAttackArea,
                jointShoulder1Tran.position
                + orientedTran.right * sideRange
                - transform.up * .25f,
            transform.rotation
        );
        float distance = Vector3.Distance(newfighterAttack.transform.position, stanceHand1Tran.position);
        float distancePerFrame = distance / framesTaken;

        newfighterAttack.GetComponent<FighterAttackAreaScript>().lifespan = timeTaken * 60f;

        for (int i = 0; i < framesTaken; i++)
        {
            stanceHeadTran.position += orientedTran.right * distancePerFrame * .4f;
            stanceHand1Tran.LookAt(newfighterAttack.transform.position);
            stanceHand1Tran.position += stanceHand1Tran.forward * distancePerFrame;

            stanceHand2Tran.LookAt(stanceHeadTran.position + orientedTran.right * 1f);
            stanceHand2Tran.position += stanceHand2Tran.forward * distancePerFrame * 0.5f;
            upperArm1Tran.position = jointElbow1Tran.position;
            yield return null;
        }
        while (Vector3.Distance(stanceHand2Tran.position, hand2DefaultVector) > 0.1f)
        {
            MoveTowardsDefaultStance();
            MoveTowardsDefaultStance();
            yield return null;
        }
        controlsEnabled = true;
    }

    IEnumerator Jab(string type)
    {
        float punchDistance = 0f;
        switch (type)
        {
            case "aggressive":
                punchDistance = 1.6f;
                break;
            case "defensive":
                punchDistance = 1.2f;
                break;
        }
        float timeTaken = .12f; //seconds
        int framesTaken = (int)(timeTaken * 60);
        float distancePerFrame = punchDistance / framesTaken;

        GameObject newfighterAttack = Instantiate(
            fighterAttackArea,
            stanceHand2Tran.position + orientedTran.right * punchDistance,
            fighterHead.transform.rotation
        );

        float headDistancePerFrame = 0.3f * distancePerFrame;
        Vector3 headMoveVector = orientedTran.right * headDistancePerFrame;
        if (type == "defensive")
        {
            headMoveVector = -1f * orientedTran.right * headDistancePerFrame;
        }

        newfighterAttack.GetComponent<FighterAttackAreaScript>().lifespan = timeTaken * 60f;

        for (int i = 0; i < framesTaken; i++)
        {
            stanceHeadTran.position += headMoveVector;

            //moves jabbing hand right
            stanceHand2Tran.LookAt(newfighterAttack.transform.position);
            stanceHand2Tran.position += stanceHand2Tran.forward * distancePerFrame;

            // moves nonjab hand to guard
            stanceHand1Tran.LookAt(stanceHeadTran.position + orientedTran.right * 1f);
            stanceHand1Tran.position += stanceHand1Tran.forward * distancePerFrame;
            upperArm1Tran.position = jointElbow1Tran.position;
            yield return null;
        }
        while (Vector3.Distance(stanceHand2Tran.position, hand2DefaultVector) > 0.1f)
        {
            MoveTowardsDefaultStance();
            MoveTowardsDefaultStance();
            yield return null;
        }
        if (type == "defensive")
        {
            yield return StartCoroutine(Hook());
            yield return null;
        }
        controlsEnabled = true;
        Debug.Log("controls re-enabled");
    }

    IEnumerator FrontKick()
    {
        float range = 4.5f;
        float timeTaken = .25f; //seconds
        int framesTaken = (int)(timeTaken * 60);

        stancePelvisActive = true;
        Vector3 origHeadPosition = stanceHeadTran.position;
        Vector3 origBotTorsoPosition = torsoBottom.transform.position;

        GameObject newfighterAttack = Instantiate(
            fighterAttackArea,
            jointPelvis1Tran.position + orientedTran.right * range,
            stanceHeadTran.rotation
        );


        float footMovingDistance = Vector3.Distance(stanceFoot1Tran.position, newfighterAttack.transform.position);
        float distancePerFrame = footMovingDistance / framesTaken;

        Vector3 torsoMoveDistance = orientedTran.right * distancePerFrame * 0.15f;

        newfighterAttack.GetComponent<FighterAttackAreaScript>().lifespan = timeTaken * 60f;

        for (int i = 0; i < framesTaken; i++)
        {
            stanceHeadTran.position += torsoMoveDistance * .5f;
            stancePelvisTran.position += torsoMoveDistance;

            stanceFoot1Tran.LookAt(newfighterAttack.transform.position);
            stanceFoot1Tran.position += stanceFoot1Tran.forward * distancePerFrame;

            // guarding
            Vector3 hand1Guard = stanceHeadTran.position + orientedTran.right * 0.75f;
            float distance = Vector3.Distance(hand1Guard, stanceHand1Tran.position);
            stanceHand1Tran.LookAt(hand1Guard);
            stanceHand1Tran.position += stanceHand1Tran.forward * Mathf.Max(distancePerFrame * distance, speed);
            yield return null;
        }
        stancePelvisActive = false;

        while (Vector3.Distance(stanceFoot1Tran.position, foot1DefaultVector) > .25f)
        {
            MoveTowardsDefaultStance();
            yield return null;
        }
        controlsEnabled = true;
        Debug.Log("controls re-enabled");
    }

    public IEnumerator Jump(float jumpSpeed)
    {/*
        while(stanceHeadTran.position.y > -0.5f){
            stanceHeadTran.position -= orientedTran.up * speed;
            yield return null;
        }        
        while(stanceHeadTran.position.y < 0.5f){
            stanceHeadTran.position += orientedTran.up * jumpSpeed;
            yield return null;
        }*/
        Debug.Log("jumped into air");
        fighterRB.velocity = new Vector2(0, jumpSpeed);
        fighterRB.gravityScale = 2f;
        stanceFoot1Tran.position += Vector3.up * 0.01f;
        stanceFoot2Tran.position += Vector3.up * 0.01f;

        while (stanceFoot1Tran.position.y >= groundLevel 
            && stanceFoot2Tran.position.y >= groundLevel)
        {
            yield return null;
        }
        Debug.Log("landed");
        fighterRB.velocity = new Vector2(0, 0);
        fighterRB.gravityScale = 0f;
        transform.position = new Vector3(
            transform.position.x,
            0,
            0
        );
    }
    IEnumerator JumpingFrontKick()
    {
        StartCoroutine(Jump(10f));
        /*for (int i = 0; i < 10; i++)
        {
            //stanceHeadTran.position -= Vector3.up * 1/10f;
            yield return null;
        }*/
        controlsEnabled = true;
        yield return null;
    }
}
