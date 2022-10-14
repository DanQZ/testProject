using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterScript : MonoBehaviour
{
    float speed;
    public int hp;
    public int attackInterval; //frames between each attack
    bool facingRight;
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

    public GameObject stancePelvis;
    private Transform stancePelvisTran;
    bool stancePelvisActive = false;

    public GameObject spriteObject;
    SpriteRenderer fighterSprite;

    // public List<string> movementQueue = new List<string>();

    public float reach; // distance the fighterHead can move from fighter object position

    bool controlsEnabled; // enabled, fighter can move head around + limbs move to default positions. disable when in an animation

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

    void InitTransforms()
    {
        upperArm1Tran = upperArm1.transform;
        upperArm2Tran = upperArm2.transform;
        lowerArm1Tran = lowerArm1.transform;
        lowerArm2Tran = lowerArm2.transform;

        thigh1Tran = thigh1.transform;
        thigh2Tran = thigh2.transform;
        calf1Tran = calf1.transform;
        calf2Tran = calf2.transform;

        stanceHand1Tran = stanceHand1.transform;
        stanceHand2Tran = stanceHand2.transform;
        stanceFoot1Tran = stanceFoot1.transform;
        stanceFoot2Tran = stanceFoot2.transform;
        stancePelvisTran = stancePelvis.transform;

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
        InitLineRenderers();
        InitTransforms();

        expectedElbowDistanceToNeck1 = Vector3.Distance(jointElbow1Tran.position, jointShoulder1Tran.position);
        expectedElbowDistanceToNeck2 = Vector3.Distance(jointElbow2Tran.position, jointShoulder2Tran.position);

        controlsEnabled = true;
        fighterSprite = spriteObject.gameObject.GetComponent<SpriteRenderer>();
        facingRight = true;
        hp = 100;
        Application.targetFrameRate = 60; // sets frame rate to 60fps, i will likely move this to another script later
        speed = 3f / 60f; // x units per 60 frames
        attackInterval = 60; //once per x frames
        reach = 1f;
    }

    void UpdateDefaultStancePositions()
    // used within MoveTowardsDefaultStance()
    {
        hand1DefaultVector = fighterHead.transform.position + Vector3.right * 0.25f + Vector3.down * 1f;
        hand2DefaultVector = fighterHead.transform.position + Vector3.right * 1.5f + Vector3.down * 0.5f;
        foot1DefaultVector = transform.position - Vector3.up * 4f - Vector3.right * 1f;
        foot2DefaultVector = transform.position - Vector3.up * 4f + Vector3.right * 1f;
    }

    void MoveTowardsDefaultStance()
    //moves at speed towards default positions of hands and feet
    {

        float fighterX = transform.position.x;
        float fighterY = transform.position.y;
        float fighterHeadX = fighterHead.transform.position.x;
        float fighterHeadY = fighterHead.transform.position.y;

        if (fighterHead.transform.position.x > fighterX + reach)
        {
            fighterHead.transform.position -= Vector3.right * speed;
        }
        if (fighterHead.transform.position.x < fighterX - reach)
        {
            fighterHead.transform.position += Vector3.right * speed;
        }
        if (fighterHead.transform.position.y < fighterX - reach)
        {
            fighterHead.transform.position += Vector3.up * speed;
        }
        if (fighterHead.transform.position.y > fighterX + reach)
        {
            fighterHead.transform.position -= Vector3.up * speed;
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
        calf1Tran.position = transform.position + Vector3.down * 4f - Vector3.right * 1f;
        calf2Tran.position = transform.position + Vector3.down * 4f + Vector3.right * 1f;
    }

    void MoveAndDrawBody() //moves limbs to desired location, then positions the LineRenderers to where the joints are
    {
        if(stanceHand1Active){
            lowerArm1Tran.position = stanceHand1Tran.position;
        }   
        else{
            stanceHand1Tran.position = lowerArm1Tran.position;
        } 
        if(stanceHand2Active){
            lowerArm2Tran.position = stanceHand2Tran.position;
        }
        else{
            stanceHand2Tran.position = lowerArm1Tran.position;
        }
        
        // forces the shoulder to connect to the elbow
        upperArm1Tran.position = jointElbow1Tran.position;
        upperArm2Tran.position = jointElbow2Tran.position;

        if(stancePelvisActive){
            torsoBottom.transform.position = stancePelvisTran.position;
        }
        else{
            stancePelvisTran.position = torsoBottom.transform.position;
        }

        calf1Tran.position = stanceFoot1Tran.position;
        calf2Tran.position = stanceFoot2Tran.position;

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

    void MoveHeadIfInput()
    //WASD moves head within allowed boundaries of size range
    {

        float fighterX = transform.position.x;
        float fighterY = transform.position.y;
        float fighterHeadX = fighterHead.transform.position.x;
        float fighterHeadY = fighterHead.transform.position.y;

        if (Input.GetKey("w"))
        {
            if (fighterHeadY < fighterY + reach)
            {
                fighterHead.transform.position += Vector3.up * speed;
            }
        }
        if (Input.GetKey("s"))
        {
            if (fighterHeadY > fighterY - reach)
            {
                fighterHead.transform.position += Vector3.down * speed;
            }
        }
        if (Input.GetKey("a"))
        {
            if (fighterHeadX > fighterX - reach)
            {
                fighterHead.transform.position += Vector3.left * speed;
            }
        }
        if (Input.GetKey("d"))
        {
            if (fighterHeadX < fighterX + reach)
            {
                fighterHead.transform.position += Vector3.right * speed;
            }
        }
    }
    bool IsHeadWithinSectors() // checks if head is within boundaries
    {
        if (fighterHead.transform.position.x >= 1 || fighterHead.transform.position.y >= 1)
        {
            return false;
        }
        if (fighterHead.transform.position.x <= -1 || fighterHead.transform.position.y <= -1)
        {
            return false;
        }
        return true;
    }


    void Update()
    {
        MoveAndDrawBody(); // important this is called first
        if (!controlsEnabled)
        {
            return;
        }
        MoveHeadIfInput();
        MoveTowardsDefaultStance();
        if (controlsEnabled && IsHeadWithinSectors())
        {
            if (Input.GetKey("space"))
            {
                controlsEnabled = false;
                Attack("arms");
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                controlsEnabled = false;
                Attack("legs");
            }
        }
    }

    // finds what sector the head is in, in order to do a
    int GetHeadSector()
    {

        string[] attacks = {
        "bottom left", "bottom", "bottom right",
        "center left", "true center", "center right",
        "top left", "top", "top right"
        };

        int xSector = 1;
        float fighterHeadX = fighterHead.transform.position.x;
        float fighterX = transform.position.x;
        if (fighterHeadX < fighterX - reach / 3)
        {
            xSector = 0;
        }
        if (fighterHeadX > reach / 3)
        {
            xSector = 2;
        }

        int ySector = 1;
        float fighterHeadY = fighterHead.transform.position.y;
        float fighterY = transform.position.y;
        if (fighterHeadY < fighterY - reach / 3)
        {
            ySector = 0;
        }
        if (fighterHeadY > fighterY + reach / 3)
        {
            ySector = 2;
        }

        int sector = ySector * 3 + xSector;
        Debug.Log(attacks[sector] + " attack");
        return sector;
    }

    void Attack(string attackType)
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
                        StartCoroutine(FrontKick());
                        break;
                    case 1: // bottom 
                        StartCoroutine(FrontKick());
                        break;
                    case 2: // bottom right
                        StartCoroutine(FrontKick());
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


    IEnumerator Hook()
    {
        float sideRange = 4f;
        float timeTaken = .2f; //seconds
        int framesTaken = (int)(timeTaken * 60);

        GameObject newfighterAttack = Instantiate(
            fighterAttackArea,
            new Vector3(
                jointShoulder1Tran.position.x + sideRange * 1f,
                jointShoulder1Tran.position.y + .25f,
                0),
            fighterHead.transform.rotation
        );
        float distance = Vector3.Distance(newfighterAttack.transform.position, stanceHand1Tran.position);
        float distancePerFrame = distance / framesTaken;

        newfighterAttack.GetComponent<PlayerAttackAreaScript>().lifespan = timeTaken * 60f;

        for (int i = 0; i < framesTaken; i++)
        {
            fighterHead.transform.position = new Vector3(
                fighterHead.transform.position.x + distancePerFrame * .4f,
                fighterHead.transform.position.y,
                0);
            stanceHand1Tran.LookAt(newfighterAttack.transform.position);
            stanceHand1Tran.position += stanceHand1Tran.forward * distancePerFrame;

            stanceHand2Tran.LookAt(fighterHead.transform.position + Vector3.right * 1f);
            stanceHand2Tran.position += stanceHand2Tran.forward * distancePerFrame * 0.5f;
            upperArm1Tran.position = jointElbow1Tran.position;
            yield return null;
        }
        while (Vector3.Distance(stanceHand2Tran.position, hand2DefaultVector) > 0.5f)
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
            new Vector3(
                stanceHand2Tran.position.x + punchDistance * 1f,
                stanceHand2Tran.position.y,
                0),
            fighterHead.transform.rotation
        );

        float headDistancePerFrame = 0.2f * distancePerFrame;
        Vector3 headMoveVector = Vector3.right * headDistancePerFrame;
        if (type == "defensive")
        {
            headMoveVector = -1f * Vector3.right * headDistancePerFrame;
        }

        newfighterAttack.GetComponent<PlayerAttackAreaScript>().lifespan = timeTaken * 60f;

        for (int i = 0; i < framesTaken; i++)
        {
            fighterHead.transform.position += headMoveVector;

            //moves jabbing hand right
            stanceHand2Tran.LookAt(newfighterAttack.transform.position);
            stanceHand2Tran.position += stanceHand2Tran.forward * distancePerFrame;

            // moves nonjab hand to guard
            stanceHand1Tran.LookAt(fighterHead.transform.position + Vector3.right * 1f);
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
        controlsEnabled = true;
    }

    IEnumerator FrontKick()
    {
        float range = 4.5f;
        float timeTaken = .33f; //seconds
        int framesTaken = (int)(timeTaken * 60);

        stancePelvisActive = true;
        Vector3 origHeadPosition = fighterHead.transform.position;
        Vector3 origBotTorsoPosition = torsoBottom.transform.position;

        GameObject newfighterAttack = Instantiate(
            fighterAttackArea,
            new Vector3(
                jointPelvis1Tran.position.x + range * 1f,
                jointPelvis1Tran.position.y,
                0),
            fighterHead.transform.rotation
        );


        float footMovingDistance = Vector3.Distance(stanceFoot1Tran.position, newfighterAttack.transform.position);
        float distancePerFrame = footMovingDistance / framesTaken;

        Vector3 torsoMoveDistance = Vector3.right * distancePerFrame * 0.15f;

        newfighterAttack.GetComponent<PlayerAttackAreaScript>().lifespan = timeTaken * 60f;

        for (int i = 0; i < framesTaken; i++)
        {
            controlsEnabled = false;
            fighterHead.transform.position += torsoMoveDistance * .5f;
            stancePelvisTran.position += torsoMoveDistance;

            stanceFoot1Tran.LookAt(newfighterAttack.transform.position);
            stanceFoot1Tran.position += stanceFoot1Tran.forward * distancePerFrame;

            // guarding
            Vector3 hand1Guard = fighterHead.transform.position + Vector3.right * 0.75f;
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
    }
}
