using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSlider : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void Shift(Vector3 relativeOrigin, Vector2 normalizedSlope, float amount)
    {
        Vector3 temp = relativeOrigin;
        temp += new Vector3(normalizedSlope.x, 0, normalizedSlope.y) * amount;
        this.transform.position = temp;
    }
}
