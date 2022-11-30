using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManagerScript : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyManager;
    
    private EnemyManagerScript enemyManagerScript;
    public List<GameObject> allEnemies;
    
    void Awake()
    {
        Application.targetFrameRate = 60; // sets frame rate to 60fps, i will likely move this to another script later
        QualitySettings.vSyncCount = 0; // if vSync is on, it puts frame rate to the monitor's frame rate, ignoring the line above


        transform.position = new Vector3(0f,0f,0f);
        GameObject newPlayerObject = Instantiate(playerPrefab, transform.position, transform.rotation);
        PlayerScript newPlayerScript = playerPrefab.GetComponent<PlayerScript>();

        enemyManagerScript = enemyManager.GetComponent<EnemyManagerScript>();
        enemyManagerScript.player = newPlayerObject;
        allEnemies = enemyManagerScript.allEnemies;
        
        newPlayerScript.gameStateManager = this.gameObject;
        newPlayerScript.PCScript = newPlayerScript.playerCharacter.GetComponent<FighterScript>();

        enemyManagerScript.gameStateManager = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator RespawnPlayer(float secondsOfDelay){
        int delayFrames = (int) (secondsOfDelay * 60f);
        for(int i = 0; i < delayFrames; i++){
            yield return null;
        }
        
    }
}
