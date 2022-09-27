using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript2235 : MonoBehaviour
{
    // Start is called before the first frame update
    public SpriteRenderer illustration1;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position += Vector3.right * 0.001f;   
    }

    void newMethod(){

    }
}