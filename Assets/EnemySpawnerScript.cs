using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject enemyPrefab;

    void Start()
    {

    }

    float time = 0;

     Vector3 RandomVec(float min, float max)
     {
         return new Vector3(UnityEngine.Random.Range(min, max),
          UnityEngine.Random.Range(min, max), 
          UnityEngine.Random.Range(min, max));
     }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > 1){
            time -= 1;
            Instantiate(enemyPrefab, RandomVec(-7.5f, 7.5f), Quaternion.identity);
        }
    }
}
