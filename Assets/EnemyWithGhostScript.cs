using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWithGhostScript : MonoBehaviour
{
    public GameObject myManagerObject;
    public EnemyManagerScript myManagerScript;

    public int enemyID;
    public GameObject enemyFighter;
    public GameObject ghostFighter;
    public GameObject enemyHeadStance;
    public GameObject ghostHeadStance;
    Transform ghostFighterTran;
    Transform ghostHeadStanceTran;
    Transform enemyFighterTran;
    Transform enemyHeadStanceTran;
    Transform enemyTorsoTopStanceTran;
    Transform enemyTorsoBotStanceTran;
    Transform enemyHand1StanceTran;
    Transform enemyHand2StanceTran;
    Transform enemyFoot1StanceTran;
    Transform enemyFoot2StanceTran;
    public FighterScript enemyFighterScript;
    public FighterScript ghostFighterScript;
    public GameObject playerFighter;
    public GameObject playerHead;
    // Start is called before the first frame update
    int timer;
    int attackInterval;
    float targetDistanceToPlayer;
    float distanceToPlayer;

    Vector3 pos1sAgo;
    string method1sAgo;
    // failed attempt
    //List<string> actionsThatFrame = new List<string>();
    //Queue<List<string>> actionQueue = new Queue<List<string>>();
    Queue<Vector3> bodPosQ = new Queue<Vector3>();
    Queue<Vector3> headPosQ = new Queue<Vector3>();
    Queue<Vector3> torsoTopPosQ = new Queue<Vector3>();
    Queue<Vector3> torsoBotPosQ = new Queue<Vector3>();
    Queue<Vector3> hand1PosQ = new Queue<Vector3>();
    Queue<Vector3> hand2PosQ = new Queue<Vector3>();
    Queue<Vector3> foot1PosQ = new Queue<Vector3>();
    Queue<Vector3> foot2PosQ = new Queue<Vector3>();
    Queue<bool> facingRightQ = new Queue<bool>();
    Queue<int> currentEnergyQ = new Queue<int>();

    private IEnumerator realEnemyActionsCoroutine;

    public void StopAll(){
        Destroy(ghostFighter);
        StopCoroutine(realEnemyActionsCoroutine);
    }

    void Awake()
    {
        enemyFighterScript = enemyFighter.gameObject.GetComponent<FighterScript>();
        enemyFighterScript.isGhost = false;
        enemyFighterScript.isPlayer = false;

        ghostFighterScript = ghostFighter.gameObject.GetComponent<FighterScript>();
        ghostFighterScript.isGhost = true;
        ghostFighterScript.isPlayer = false;

        ghostFighterTran = ghostFighter.transform;
        ghostHeadStanceTran = ghostHeadStance.transform;

        realEnemyActionsCoroutine = RealEnemyActions();
        StartCoroutine(realEnemyActionsCoroutine);
    }

    void Start()
    {
        StartCoroutine(ghostFighterScript.InitBasedOnCharSettings());
        StartCoroutine(enemyFighterScript.InitBasedOnCharSettings());
        enemyFighterTran = enemyFighter.transform;
        enemyHeadStanceTran = enemyHeadStance.transform;

        enemyTorsoTopStanceTran = enemyFighterScript.stanceTorsoTopTran;
        enemyTorsoBotStanceTran = enemyFighterScript.stanceTorsoBotTran;

        enemyHand1StanceTran = enemyFighterScript.stanceHand1Tran;
        enemyHand2StanceTran = enemyFighterScript.stanceHand2Tran;

        enemyFoot1StanceTran = enemyFighterScript.stanceFoot1Tran;
        enemyFoot2StanceTran = enemyFighterScript.stanceFoot2Tran;
        
        enemyID = myManagerScript.numOfEnemiesSpawnedThisGame;
    }

    IEnumerator RealEnemyActions()
    {
        // waits 1 second before starting the forever while loop
        yield return new WaitForSeconds(1);

        while (true)
        {
            enemyFighterTran.localPosition = bodPosQ.Dequeue();
            enemyHeadStanceTran.localPosition = headPosQ.Dequeue();

            enemyTorsoTopStanceTran.localPosition = torsoTopPosQ.Dequeue();
            enemyTorsoBotStanceTran.localPosition = torsoBotPosQ.Dequeue();

            enemyHand1StanceTran.localPosition = hand1PosQ.Dequeue();
            enemyHand2StanceTran.localPosition = hand2PosQ.Dequeue();

            enemyFoot1StanceTran.localPosition = foot1PosQ.Dequeue();
            enemyFoot2StanceTran.localPosition = foot2PosQ.Dequeue();

            bool toFaceRight = facingRightQ.Dequeue();
            enemyFighterScript.currentEnergy = currentEnergyQ.Dequeue();
            enemyFighterScript.UpdateEnergyBar();
            if (enemyFighterScript.facingRight != toFaceRight)
            {
                //Debug.Log("successful detection of turning");
                enemyFighterScript.SwapHingeAngles();
                enemyFighterScript.TurnBody();
                enemyFighterScript.facingRight = toFaceRight;
            }

            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        bodPosQ.Enqueue(ghostFighterTran.localPosition);
        headPosQ.Enqueue(ghostHeadStanceTran.localPosition);

        torsoTopPosQ.Enqueue(ghostFighterScript.stanceTorsoTopTran.localPosition);
        torsoBotPosQ.Enqueue(ghostFighterScript.stanceTorsoBotTran.localPosition);

        hand1PosQ.Enqueue(ghostFighterScript.stanceHand1Tran.localPosition);
        hand2PosQ.Enqueue(ghostFighterScript.stanceHand2Tran.localPosition);

        foot1PosQ.Enqueue(ghostFighterScript.stanceFoot1Tran.localPosition);
        foot2PosQ.Enqueue(ghostFighterScript.stanceFoot2Tran.localPosition);

        facingRightQ.Enqueue(ghostFighterScript.facingRight);
        currentEnergyQ.Enqueue(ghostFighterScript.currentEnergy);
    }
}