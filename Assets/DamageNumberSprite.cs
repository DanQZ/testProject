using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageNumberSprite : MonoBehaviour
{
    public bool isCrit;
    public string type;
    public float changeNumber;
    public TextMeshProUGUI numberText;
    // Start is called before the first frame update
    void Awake()
    {
        this.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector3.up * 5f, ForceMode2D.Impulse);
    }
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
                    if (isCrit)
                    {
                        numberText.color = Color.red;
                    }
                    else
                    {
                        numberText.color = Color.white;
                    }
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

        float sizeMultiplier = Mathf.Log(Mathf.Abs(changeNumber) + 10f, 100f);
        transform.localScale = new Vector3(
            transform.localScale.x * sizeMultiplier,
            transform.localScale.y * sizeMultiplier,
            transform.localScale.z * sizeMultiplier
        );

        numberText.text = "" + changeNumber.ToString("F1"); // gives 1 decimal point
    }

    // Update is called once per frame
    void Update()
    {
        numberText.color = new Color(numberText.color.r, numberText.color.g, numberText.color.b, numberText.color.a - 0.02f);
        if (numberText.color.a < 0.01f)
        {
            Destroy(this.gameObject);
        }
    }
}
