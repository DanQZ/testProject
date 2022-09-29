using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerControllerScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private List<GameObject> EnemySpawners;

    void Start()
    {
        foreach (GameObject obj in EnemySpawners){
            obj.GetComponent<EnemySpawnerScript>().turnOn();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
