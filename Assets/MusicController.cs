using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource inGameSoundtrack;
    // Start is called before the first frame update
    void Start()
    {
        inGameSoundtrack.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
