using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideInfoText : MonoBehaviour {

    public TextMesh infotext;
    public Ship ownShip;
    public Ship guideShip;

    public float maxCharSize = 2;
    public float minCharSize = 0.5f;
    float maxFontResizeDistance = 3000;
    float minFontResizeDistance = 1000;

    private float bearingFromGuide;
    private float rangeToGuide;

    // Use this for initialization
    void Start ()
    {
        infotext = GetComponent<TextMesh>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        transform.LookAt(Camera.main.transform);
        this.transform.Rotate(0, 180, 0);
        dynamicFontSizeAdjust();
        RelVelCalc.TrueBearingAndRangeFrom(ownShip.transform.position, guideShip.transform.position, out bearingFromGuide, out rangeToGuide);
        //infotext.text = "Guide Bears: " + bearingFromGuide.ToString() + "°\nGuide Range: " + rangeToGuide.ToString()+ "x" + "\nKey Range: " + ownShip.keyRange.ToString() + "\nKey Bearing: " + ownShip.keyBearing.ToString();
    }
    
    /// <summary>
    /// Dynamically resize the cardinal bearing text based on the distance from the camera
    /// </summary>
    protected void dynamicFontSizeAdjust()
    {
        float cameraDistance = Vector3.Distance(this.transform.position, Camera.main.transform.position);
        if (cameraDistance > 3000)
        {
            cameraDistance = 3000;
        }

        float coefficient = maxCharSize / maxFontResizeDistance;
        float charSize = coefficient * cameraDistance;
        infotext.characterSize = Mathf.Clamp(charSize, minCharSize, maxCharSize);
    }

}
