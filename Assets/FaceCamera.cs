using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public TextMesh text = new TextMesh();
    public Ship guideShip;
    public Ship consortOne;
    public bool DisplayRange = false;
    public Camera mainCamera;
	// Use this for initialization
	void Start ()
    {
       
    }
	
	// Update is called once per frame
	void Update ()
    {
        transform.LookAt(mainCamera.transform);
        this.transform.Rotate(0, 180, 0);
        if (DisplayRange == true)
        {
            text.text = "Rng to Guide: " + Vector3.Distance(this.transform.position, guideShip.transform.position).ToString() + "\nRng to Consort: " + Vector3.Distance(this.transform.position, consortOne.transform.position).ToString() + "\n Guide Bears: " + Vector2.SignedAngle(new Vector2(guideShip.transform.position.x, guideShip.transform.position.z), new Vector2(this.transform.position.x, this.transform.position.z));
        }
    }
}
