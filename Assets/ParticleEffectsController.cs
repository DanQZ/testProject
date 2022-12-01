using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectsController : MonoBehaviour
{
    public GameObject groundSlamObject;
    private ParticleSystem groundSlamParticles; 
    void Awake()
    {
        groundSlamParticles = groundSlamObject.GetComponent<ParticleSystem>();
    }

    public void PlayEffect(string effectNameArg, bool destroyAfterFinishing){
        string effectName = effectNameArg.ToLower();
        switch(effectName){
            case "groundslam":
                groundSlamParticles.Play();
            break;
        }
        if(destroyAfterFinishing){
            StartCoroutine(SuicideAfterEffectIsDone(groundSlamParticles));
        }
    }
    private IEnumerator SuicideAfterEffectIsDone(ParticleSystem particles){
        while(particles.IsAlive()){
            yield return null;
        }
        Destroy(this.gameObject);
    }
}
