using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject enemyPrefab;
    
    private bool start = false;

    void Start()
    {

    }

    public void turnOn() {start = true;}
    public void turnOff() {start = false;}
    

    float time = 0;

    // Update is called once per frame
    void Update()
    {
        if (start){
            time += Time.deltaTime;
            if (time > 1){
                time -= 1;
                Instantiate(enemyPrefab, gameObject.transform.position, Quaternion.identity);
            }
        }
    }
}
