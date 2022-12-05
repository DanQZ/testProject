using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageNumberSprite : MonoBehaviour
{
    public bool isCrit;
    public string type;
    public float changeNumber;
    public Text numberText;
    // Start is called before the first frame update
    void Start()
    {
        switch (type)
        {
            case "health":
                if (changeNumber > 0)
                {
                    numberText.color = Color.green;
                }
                if (changeNumber == 0)
                {
                    numberText.color = Color.grey;
                }
                if (changeNumber < 0)
                {
                    numberText.color = Color.red;
                }
                break;
            case "energy":
                if (changeNumber > 0)
                {
                    numberText.color = Color.yellow;
                }
                if (changeNumber < 0)
                {
                    numberText.color = Color.cyan;
                }
                break;
        }


        this.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector3.up * 5f, ForceMode2D.Impulse);

        numberText.text = "" + changeNumber.ToString("F1"); // gives 1 decimal point
        if(isCrit){
            numberText.text += " CRIT";
        }
    }

    // Update is called once per frame
    void Update()
    {
        numberText.color = new Color(numberText.color.r, numberText.color.g, numberText.color.b, numberText.color.a - 0.02f);
        if (this.gameObject.transform.position.y < -10f)
        {
            Destroy(this.gameObject);
        }
    }
}
