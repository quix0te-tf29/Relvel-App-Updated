using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassRing : MonoBehaviour
{
    //bearings to be drawn every X number of degrees
    public int bearingInterval  = 10;
    public GameObject linePrefab;
    public GameObject textPrefab;
    public LineRenderer circle;
    public List<GameObject> bearingLines;
    public List<GameObject> bearingText;

    public int resolution;
    public float radius;

	// Use this for initialization
	void Start ()
    {
        circle = this.gameObject.GetComponent<LineRenderer>();
        DrawBearingMarks();
    }
	
	// Update is called once per frame
	void Update ()
    {

    }

    public void ChangeRadius(float newRadius)
    {
        //set the new radius
        radius = newRadius;

        //Eliminate all previous instances of the compass before drawing a new compass
        GameObject[] GameObjects = GameObject.FindGameObjectsWithTag("clone");
        foreach (GameObject obj in GameObjects)
        {
            Destroy(obj);
        }
        //draw a new compass  using the provided radius
        DrawBearingMarks();
    }

    private void DrawBearingMarks()
    {
        linePrefab.SetActive(true);
        textPrefab.SetActive(true);

        bearingLines.Clear();
        bearingText.Clear();

        float theta;
        int Size = 360 / bearingInterval;
        float y_coord;
        float x_coord;
        circle.positionCount = Size;

        for (int i = 0; i < Size; i++)
        {
            theta = (i * bearingInterval * Mathf.PI) / 180.0f;

            y_coord = Mathf.Cos(theta) * radius;
            x_coord = Mathf.Sin(theta) * radius;

            GameObject tempLine = Instantiate<GameObject>(linePrefab);
            tempLine.tag = "clone";
            tempLine.transform.parent = this.transform;
            tempLine.transform.position = this.transform.position;
            LineRenderer line = tempLine.GetComponent<LineRenderer>();

            GameObject tempText = Instantiate<GameObject>(textPrefab);
            tempText.tag = "clone";
            tempText.transform.parent = this.transform;
            TextMesh text = tempText.GetComponent<TextMesh>();

            line.SetPosition(0, new Vector3(x_coord, -35, y_coord));
            line.SetPosition(1, new Vector3(x_coord, 0, y_coord));

            text.transform.localPosition = new Vector3(x_coord, 50, y_coord);
            float tempBearing = i * bearingInterval;
            text.text = tempBearing.ToString();

            circle.SetPosition(i, new Vector3(x_coord, 0, y_coord));
            bearingLines.Add(tempLine);
            bearingText.Add(tempText);
        }
        linePrefab.SetActive(false);
        textPrefab.SetActive(false);
    }
}
