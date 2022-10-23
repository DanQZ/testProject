using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWithGhostScript : MonoBehaviour
{
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

    void Awake()
    {
        enemyFighterScript = enemyFighter.gameObject.GetComponent<FighterScript>();
        ghostFighterScript = ghostFighter.gameObject.GetComponent<FighterScript>();
        ghostFighterScript.isGhost = true;

        ghostFighterTran = ghostFighter.transform;
        ghostHeadStanceTran = ghostHeadStance.transform;


        StartCoroutine(RealEnemyActions());
    }

    void Start()
    {
        ghostFighterScript.ChangeColor(Color.grey);

        enemyFighterTran = enemyFighter.transform;
        enemyHeadStanceTran = enemyHeadStance.transform;

        enemyTorsoTopStanceTran = enemyFighterScript.stanceTorsoTopTran;
        enemyTorsoBotStanceTran = enemyFighterScript.stanceTorsoBotTran;

        enemyHand1StanceTran = enemyFighterScript.stanceHand1Tran;
        enemyHand2StanceTran = enemyFighterScript.stanceHand2Tran;

        enemyFoot1StanceTran = enemyFighterScript.stanceFoot1Tran;
        enemyFoot2StanceTran = enemyFighterScript.stanceFoot2Tran;

    }

    IEnumerator RealEnemyActions()
    {
        // waits 1 second before starting the forever while loop
        yield return new WaitForSeconds(1);

        while (true)
        {
            enemyFighterTran.position = bodPosQ.Dequeue();
            enemyHeadStanceTran.position = headPosQ.Dequeue();

            enemyTorsoTopStanceTran.position = torsoTopPosQ.Dequeue();
            enemyTorsoBotStanceTran.position = torsoBotPosQ.Dequeue();

            enemyHand1StanceTran.position = hand1PosQ.Dequeue();
            enemyHand2StanceTran.position = hand2PosQ.Dequeue();

            enemyFoot1StanceTran.position = foot1PosQ.Dequeue();
            enemyFoot2StanceTran.position = foot2PosQ.Dequeue();

            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        bodPosQ.Enqueue(ghostFighterTran.position);
        headPosQ.Enqueue(ghostHeadStanceTran.position);

        torsoTopPosQ.Enqueue(ghostFighterScript.stanceTorsoTopTran.position);
        torsoBotPosQ.Enqueue(ghostFighterScript.stanceTorsoBotTran.position);

        hand1PosQ.Enqueue(ghostFighterScript.stanceHand1Tran.position);
        hand2PosQ.Enqueue(ghostFighterScript.stanceHand2Tran.position);

        foot1PosQ.Enqueue(ghostFighterScript.stanceFoot1Tran.position);
        foot2PosQ.Enqueue(ghostFighterScript.stanceFoot2Tran.position);
    }
}