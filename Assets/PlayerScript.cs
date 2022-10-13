using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    float speed;
    public int hp;
    public int attackInterval; //frames between each attack
    bool facingRight;
    public GameObject playerAttackArea;

    // limb gameObjects are the main player body gameObjects
    public GameObject playerHead;
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
    public GameObject stanceHand1;
    private Transform stanceHand1Tran;
    public GameObject stanceHand2;
    private Transform stanceHand2Tran;

    public GameObject stanceFoot1;
    private Transform stanceFoot1Tran;
    public GameObject stanceFoot2;
    private Transform stanceFoot2Tran;

    public GameObject spriteObject;
    SpriteRenderer playerSprite;

    // public List<string> movementQueue = new List<string>();

    public float reach; // distance the playerHead can move from player object position

    bool controlsEnabled; // enabled, player can move head around + limbs move to default positions. disable when in an animation

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

    //default stances relative to player position
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
        playerSprite = spriteObject.gameObject.GetComponent<SpriteRenderer>();
        facingRight = true;
        hp = 100;
        Application.targetFrameRate = 60; // sets frame rate to 60fps, i will likely move this to another script later
        speed = 3f / 60f; // x units per 60 frames
        attackInterval = 60; //once per x frames
        reach = 1f;
    }

    void UpdateDefaultStancePositions()
    { // used within MoveTowardsDefaultStance
        hand1DefaultVector = playerHead.transform.position + Vector3.right * 0.25f + Vector3.down * 1f;
        hand2DefaultVector = playerHead.transform.position + Vector3.right * 1.5f + Vector3.down * 0.5f;
        foot1DefaultVector = transform.position - Vector3.up * 4f - Vector3.right * 1f;
        foot2DefaultVector = transform.position - Vector3.up * 4f + Vector3.right * 1f;
    }
    void MoveTowardsDefaultStance()
    {

        float playerX = transform.position.x;
        float playerY = transform.position.y;
        float playerHeadX = playerHead.transform.position.x;
        float playerHeadY = playerHead.transform.position.y;

        if (playerHead.transform.position.x > playerX + reach)
        {
            playerHead.transform.position -= Vector3.right * speed;
        }
        if (playerHead.transform.position.x < playerX - reach)
        {
            playerHead.transform.position += Vector3.right * speed;
        }
        if (playerHead.transform.position.y < playerX - reach)
        {
            playerHead.transform.position += Vector3.up * speed;
        }
        if (playerHead.transform.position.y > playerX + reach)
        {
            playerHead.transform.position -= Vector3.up * speed;
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

    //keeps feet on the ground 
    void GroundedFeet()
    {
        calf1Tran.position = transform.position + Vector3.down * 4f - Vector3.right * 1f;
        calf2Tran.position = transform.position + Vector3.down * 4f + Vector3.right * 1f;
    }
    void MoveAndDrawBody() //moves limbs to desired location, then positions the LineRenderers to where the joints are
    {
        lowerArm1Tran.position = stanceHand1Tran.position;
        lowerArm2Tran.position = stanceHand2Tran.position;
        upperArm1Tran.position = jointElbow1Tran.position;
        upperArm2Tran.position = jointElbow2Tran.position;

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

    //on WASD move head
    void MoveHeadIfInput()
    {

        float playerX = transform.position.x;
        float playerY = transform.position.y;
        float playerHeadX = playerHead.transform.position.x;
        float playerHeadY = playerHead.transform.position.y;

        if (Input.GetKey("w"))
        {
            if (playerHeadY < playerY + reach)
            {
                playerHead.transform.position += Vector3.up * speed;
            }
        }
        if (Input.GetKey("s"))
        {
            if (playerHeadY > playerY - reach)
            {
                playerHead.transform.position += Vector3.down * speed;
            }
        }
        if (Input.GetKey("a"))
        {
            if (playerHeadX > playerX - reach)
            {
                playerHead.transform.position += Vector3.left * speed;
            }
        }
        if (Input.GetKey("d"))
        {
            if (playerHeadX < playerX + reach)
            {
                playerHead.transform.position += Vector3.right * speed;
            }
        }
    }
    bool IsHeadWithinSectors()
    {
        if (playerHead.transform.position.x >= 1 || playerHead.transform.position.y >= 1)
        {
            return false;
        }
        if (playerHead.transform.position.x <= -1 || playerHead.transform.position.y <= -1)
        {
            return false;
        }
        return true;
    }
    // Update is called once per frame
    void Update()
    {
        MoveAndDrawBody();

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
        float playerHeadX = playerHead.transform.position.x;
        float playerX = transform.position.x;
        if (playerHeadX < playerX - reach / 3)
        {
            xSector = 0;
        }
        if (playerHeadX > reach / 3)
        {
            xSector = 2;
        }

        int ySector = 1;
        float playerHeadY = playerHead.transform.position.y;
        float playerY = transform.position.y;
        if (playerHeadY < playerY - reach / 3)
        {
            ySector = 0;
        }
        if (playerHeadY > playerY + reach / 3)
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
                        StartCoroutine(Jab());
                        break;
                    case 3: // center left
                        StartCoroutine(Hook());
                        break;
                    case 4: // center 
                        StartCoroutine(Hook());
                        break;
                    case 5: // center right
                        StartCoroutine(Jab());
                        break;
                    case 6: // top left
                        StartCoroutine(Hook());
                        break;
                    case 7:  // top middle
                        StartCoroutine(Hook());
                        break;
                    case 8: // top right
                        StartCoroutine(Jab());
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
        float punchDistance = 3f;
        float timeTaken = .2f; //seconds
        int framesTaken = (int)(timeTaken * 60);
        float distancePerFrame = punchDistance / framesTaken;

        GameObject newPlayerAttack = Instantiate(
            playerAttackArea,
            new Vector3(
                stanceHand1Tran.position.x + punchDistance * 1f,
                stanceHand1Tran.position.y + punchDistance * 0.33f,
                0),
            playerHead.transform.rotation
        );

        newPlayerAttack.GetComponent<PlayerAttackAreaScript>().lifespan = timeTaken * 60f;

        for (int i = 0; i < framesTaken; i++)
        {
            playerHead.transform.position = new Vector3(
                playerHead.transform.position.x + distancePerFrame * .25f,
                playerHead.transform.position.y,
                0);
            stanceHand1Tran.LookAt(newPlayerAttack.transform.position);
            stanceHand1Tran.position += stanceHand1Tran.forward * distancePerFrame;

            stanceHand2Tran.LookAt(playerHead.transform.position + Vector3.right * 1f);
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


    IEnumerator Jab()
    {
        float punchDistance = 1.5f;
        float timeTaken = .12f; //seconds
        int framesTaken = (int)(timeTaken * 60);
        float distancePerFrame = punchDistance / framesTaken;

        GameObject newPlayerAttack = Instantiate(
            playerAttackArea,
            new Vector3(
                stanceHand2Tran.position.x + punchDistance * 1f,
                stanceHand2Tran.position.y,
                0),
            playerHead.transform.rotation
        );

        newPlayerAttack.GetComponent<PlayerAttackAreaScript>().lifespan = timeTaken * 60f;

        for (int i = 0; i < framesTaken; i++)
        {
            playerHead.transform.position = new Vector3(
                playerHead.transform.position.x + distancePerFrame * .1f,
                playerHead.transform.position.y,
                0);
            stanceHand2Tran.LookAt(newPlayerAttack.transform.position);
            stanceHand2Tran.position += stanceHand2Tran.forward * distancePerFrame;

            stanceHand1Tran.LookAt(playerHead.transform.position + Vector3.right * 1f);
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
        float range = 4f;
        float timeTaken = .33f; //seconds
        int framesTaken = (int)(timeTaken * 60);

        GameObject newPlayerAttack = Instantiate(
            playerAttackArea,
            new Vector3(
                jointPelvis1Tran.position.x + range * 1f,
                jointPelvis1Tran.position.y,
                0),
            playerHead.transform.rotation
        );

        float footMovingDistance = Vector3.Distance(stanceFoot1Tran.position, newPlayerAttack.transform.position);
        float distancePerFrame = footMovingDistance / framesTaken;

        newPlayerAttack.GetComponent<PlayerAttackAreaScript>().lifespan = timeTaken * 60f;

        for (int i = 0; i < framesTaken; i++)
        {
            controlsEnabled = false;
            playerHead.transform.position = new Vector3(
                playerHead.transform.position.x + distancePerFrame * .25f,
                playerHead.transform.position.y,
                0);
            stanceFoot1Tran.LookAt(newPlayerAttack.transform.position);
            stanceFoot1Tran.position += stanceFoot1Tran.forward * distancePerFrame;

            // guarding
            stanceHand1Tran.LookAt(playerHead.transform.position + Vector3.right * 1f);
            stanceHand1Tran.position += stanceHand1Tran.forward * distancePerFrame / 2f;
            upperArm1Tran.position = jointElbow1Tran.position;
            yield return null;
        }

        while (Vector3.Distance(stanceFoot1Tran.position, foot1DefaultVector) > 1f)
        {
            MoveTowardsDefaultStance();
            yield return null;
        }

        controlsEnabled = true;
    }
}
