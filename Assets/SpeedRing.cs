using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedRing : MonoBehaviour
{
    private float Radius;
    public float thetaScale = 0.01f;
    public LineRenderer circleDrawer;
    float Theta = 0f;

    public float radius { get { return Radius; } set { Radius = value; Draw(); } }

    // Use this for initialization
    void Start ()
    {
        circleDrawer = GetComponent<LineRenderer>();
        Draw();
    }
	
	// Update is called once per frame
	void Update () {
        
	}


    public void Draw()
    {
        Theta = 0f;
        int Size = (int)((1f / thetaScale) + 1f);
        circleDrawer.positionCount = Size;
        for (int i = 0; i < Size; i++)
        {
            Theta += (2.0f * Mathf.PI * thetaScale);
            float x = Radius * Mathf.Cos(Theta);
            float y = Radius * Mathf.Sin(Theta);
            circleDrawer.SetPosition(i, new Vector3(x, 0, y));
        }
    }
}
