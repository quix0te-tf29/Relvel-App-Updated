using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LineComponent
{
    CTSVECTOR, RELVECTOR, GUIDECRSANDSPEEDVECTOR, ALLVECTORS
}

public class RelvelVectorDisplay : MonoBehaviour
{
    public LineRenderer RelativeVector;
    public LineRenderer GuideVector;
    public LineRenderer CTSVector;
    public float amount;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    public void ClearVector(LineComponent line)
    {
        switch (line)
        {
            case LineComponent.CTSVECTOR:
                CTSVector.SetPosition(0, Vector3.zero);
                CTSVector.SetPosition(1, Vector3.zero);
                break;
            case LineComponent.RELVECTOR:
                RelativeVector.SetPosition(0, Vector3.zero);
                RelativeVector.SetPosition(1, Vector3.zero);
                break;
            case LineComponent.GUIDECRSANDSPEEDVECTOR:
                GuideVector.SetPosition(0, Vector3.zero);
                GuideVector.SetPosition(1, Vector3.zero);
                break;
            case LineComponent.ALLVECTORS:
                CTSVector.SetPosition(0, Vector3.zero);
                CTSVector.SetPosition(1, Vector3.zero);
                RelativeVector.SetPosition(0, Vector3.zero);
                RelativeVector.SetPosition(1, Vector3.zero);
                GuideVector.SetPosition(0, Vector3.zero);
                GuideVector.SetPosition(1, Vector3.zero);
                break;
            default:
            return;
        }
    }


    public void EditRelativeVector(Vector2 start,  Vector2 end)
    {
        end = end * -1;
        end += start;
        //Start Position
        Vector3 tempStart = Vector3.zero;
        tempStart = GuideVector.GetPosition(1);
        RelativeVector.SetPosition(0, tempStart);

        //end position
        Vector3 tempEnd = new Vector3(tempStart.x + end.x, tempStart.y, tempStart.z + end.y);

        tempEnd.x = GuideVector.GetPosition(1).x + end.x * 100;
        tempEnd.z = GuideVector.GetPosition(1).z + end.y * 100;
        RelativeVector.SetPosition(1, tempEnd);
    }

    public void EditGuideVector(float magnitude, float direction)
    {
        GuideVector.SetPosition(0, Vector3.zero);
        GuideVector.SetPosition(1, Vector3.forward * magnitude * 100);
    }

    public void EditCTSVector(Vector2 direction)
    {
        print(direction.ToString());
        Vector3 temp = new Vector3(direction.x, 0, direction.y);
        CTSVector.SetPosition(0, Vector3.zero);
        CTSVector.SetPosition(1, temp * 1000);
    }
}
