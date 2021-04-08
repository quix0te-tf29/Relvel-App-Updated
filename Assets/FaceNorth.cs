using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceNorth : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
	    this.transform.rotation = Quaternion.Euler(0, -Input.compass.magneticHeading, 0);
    }
}
