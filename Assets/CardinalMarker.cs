using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardinalMarker : MonoBehaviour
{
    public TextMesh bearingText;
    public float maxCharSize = 2;
    public float minCharSize = 0.5f;
    float maxFontResizeDistance = 3000;
    float minFontResizeDistance = 1000;


    // Use this for initialization
    protected void Start()
    {
        bearingText = GetComponent<TextMesh>();
    }

    // Update is called once per frame
    protected void Update()
    {
        transform.LookAt(Camera.main.transform);
        this.transform.Rotate(0, 180, 0);
        dynamicFontSizeAdjust();
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
        bearingText.characterSize = Mathf.Clamp(charSize, minCharSize, maxCharSize);
    }
}
