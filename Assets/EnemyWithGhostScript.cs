using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWithGhostScript : MonoBehaviour {
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
    Transform enemyCustomElbow1Tran;
    Transform enemyCustomElbow2Tran;
    Transform enemyCustomKnee1Tran;
    Transform enemyCustomKnee2Tran;
    public FighterScript enemyFighterScript;
    public FighterScript ghostFighterScript;
    public GameObject playerFighter;
    public GameObject playerHead;


    Queue<bool> invulnerabilityQ = new Queue<bool>();

    Queue<Vector3> bodPosQ = new Queue<Vector3>();
    Queue<Vector3> headPosQ = new Queue<Vector3>();
    Queue<Vector3> torsoTopPosQ = new Queue<Vector3>();
    Queue<Vector3> torsoBotPosQ = new Queue<Vector3>();
    Queue<Vector3> hand1PosQ = new Queue<Vector3>();
    Queue<Vector3> hand2PosQ = new Queue<Vector3>();
    Queue<Vector3> foot1PosQ = new Queue<Vector3>();
    Queue<Vector3> foot2PosQ = new Queue<Vector3>();
    Queue<bool> facingRightQ = new Queue<bool>();
    Queue<float> currentEnergyQ = new Queue<float>();
    Queue<bool> drawNormalElbow1Q = new Queue<bool>();
    Queue<bool> drawNormalElbow2Q = new Queue<bool>();
    Queue<bool> drawNormalKnee1Q = new Queue<bool>();
    Queue<bool> drawNormalKnee2Q = new Queue<bool>();
    Queue<bool> notInAnimationQ = new Queue<bool>();
    Queue<Vector3> customKnee1Q = new Queue<Vector3>();
    Queue<Vector3> customKnee2Q = new Queue<Vector3>();
    Queue<Vector3> customElbow1Q = new Queue<Vector3>();
    Queue<Vector3> customElbow2Q = new Queue<Vector3>();

    private IEnumerator realEnemyActionsCoroutine;

    public void StopAll() {
        Destroy(ghostFighter);
        StopCoroutine(realEnemyActionsCoroutine);
    }

    void Awake() {
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

    void Start() {
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

        enemyCustomElbow1Tran = enemyFighterScript.customElbow1Tran;
        enemyCustomElbow2Tran = enemyFighterScript.customElbow2Tran;
        enemyCustomKnee1Tran = enemyFighterScript.customKnee1Tran;
        enemyCustomKnee2Tran = enemyFighterScript.customKnee2Tran;
    }

    IEnumerator RealEnemyActions() {
        // waits 1 second before starting the forever while loop
        yield return new WaitForSeconds(1);

        while (true) {
            enemyFighterScript.isInvulnerable = invulnerabilityQ.Dequeue();

            enemyFighterTran.localPosition = bodPosQ.Dequeue();
            enemyHeadStanceTran.localPosition = headPosQ.Dequeue();

            enemyTorsoTopStanceTran.localPosition = torsoTopPosQ.Dequeue();
            enemyTorsoBotStanceTran.localPosition = torsoBotPosQ.Dequeue();

            enemyHand1StanceTran.localPosition = hand1PosQ.Dequeue();
            enemyHand2StanceTran.localPosition = hand2PosQ.Dequeue();

            enemyFoot1StanceTran.localPosition = foot1PosQ.Dequeue();
            enemyFoot2StanceTran.localPosition = foot2PosQ.Dequeue();

            enemyCustomElbow1Tran.localPosition = customElbow1Q.Dequeue();
            enemyCustomElbow2Tran.localPosition = customElbow2Q.Dequeue();

            enemyCustomKnee1Tran.localPosition = customKnee1Q.Dequeue();
            enemyCustomKnee2Tran.localPosition = customKnee2Q.Dequeue();

            enemyFighterScript.drawNormalElbow1 = drawNormalElbow1Q.Dequeue();
            enemyFighterScript.drawNormalElbow2 = drawNormalElbow2Q.Dequeue();
            enemyFighterScript.drawNormalKnee1 = drawNormalKnee1Q.Dequeue();
            enemyFighterScript.drawNormalKnee2 = drawNormalKnee2Q.Dequeue();

            enemyFighterScript.notInAnimation = notInAnimationQ.Dequeue();
            enemyFighterScript.currentEnergy = currentEnergyQ.Dequeue();
            enemyFighterScript.UpdateEnergyBar();

            bool toFaceRight = facingRightQ.Dequeue();
            if (enemyFighterScript.facingRight != toFaceRight) {
                //Debug.Log("successful detection of turning");
                enemyFighterScript.SwapHingeAngles();
                enemyFighterScript.TurnBody();
                enemyFighterScript.facingRight = toFaceRight;
            }

            yield return null;
        }
    }

    // Update is called once per frame
    void Update() {
        invulnerabilityQ.Enqueue(ghostFighterScript.isInvulnerable);

        bodPosQ.Enqueue(ghostFighterTran.localPosition);
        headPosQ.Enqueue(ghostHeadStanceTran.localPosition);

        torsoTopPosQ.Enqueue(ghostFighterScript.stanceTorsoTopTran.localPosition);
        torsoBotPosQ.Enqueue(ghostFighterScript.stanceTorsoBotTran.localPosition);

        hand1PosQ.Enqueue(ghostFighterScript.stanceHand1Tran.localPosition);
        hand2PosQ.Enqueue(ghostFighterScript.stanceHand2Tran.localPosition);

        foot1PosQ.Enqueue(ghostFighterScript.stanceFoot1Tran.localPosition);
        foot2PosQ.Enqueue(ghostFighterScript.stanceFoot2Tran.localPosition);

        facingRightQ.Enqueue(ghostFighterScript.facingRight);
        notInAnimationQ.Enqueue(ghostFighterScript.notInAnimation);
        currentEnergyQ.Enqueue(ghostFighterScript.currentEnergy);

        drawNormalElbow1Q.Enqueue(ghostFighterScript.drawNormalElbow1);
        drawNormalElbow2Q.Enqueue(ghostFighterScript.drawNormalElbow2);
        drawNormalKnee1Q.Enqueue(ghostFighterScript.drawNormalKnee1);
        drawNormalKnee2Q.Enqueue(ghostFighterScript.drawNormalKnee2);

        customElbow1Q.Enqueue(ghostFighterScript.customElbow1Tran.localPosition);
        customElbow2Q.Enqueue(ghostFighterScript.customElbow2Tran.localPosition);
        customKnee1Q.Enqueue(ghostFighterScript.customKnee1Tran.localPosition);
        customKnee2Q.Enqueue(ghostFighterScript.customKnee2Tran.localPosition);

    }
}