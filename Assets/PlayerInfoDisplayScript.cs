using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required when Using UI elements.

public class PlayerInfoDisplayScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Text playerInfoText;
    public GameObject player;
    FighterScript playerScript;
    void Start()
    {
        playerScript = player.GetComponent<FighterScript>();
        playerInfoText.text = "HP ";
    }

    // Update is called once per frame
    void Update()
    {
        playerInfoText.text = "HP " + playerScript.hp;
    }
}
