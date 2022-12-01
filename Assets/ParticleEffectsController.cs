using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectsController : MonoBehaviour
{
    public GameObject groundSlamObject;
    public GameObject attackHitObject;
    private Quaternion defaultRotation;
    void Awake()
    {
        defaultRotation = transform.rotation;
    }
    public void PlayEffect(string effectNameArg)
    {
        string effectName = effectNameArg.ToLower();
        GameObject newEffectObject = null;
        switch (effectName)
        {
            case "groundslam":
                newEffectObject = Instantiate(
                    groundSlamObject,
                    transform.position - Vector3.up * 0.33f,
                    transform.rotation);
                break;
            case "attackhit":
                newEffectObject = Instantiate(
                    attackHitObject,
                    transform.position,
                    transform.rotation
                );
                break;
        }
        newEffectObject.GetComponent<ParticleSystem>().Play(); 
        transform.rotation = defaultRotation;

    }
}
