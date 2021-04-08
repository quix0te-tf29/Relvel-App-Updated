using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationMarkers : MonoBehaviour {

    [SerializeField] public GameObject AheadSD;
    [SerializeField] public GameObject AheadDSD;
    [SerializeField] public GameObject AsternSD;
    [SerializeField] public GameObject AsternDSD;
    [SerializeField] public GameObject PortBeamSD;
    [SerializeField] public GameObject PortBeamDSD;
    [SerializeField] public GameObject StarboardBeamSD;
    [SerializeField] public GameObject StarboardBeamDSD;
    //private int StandardDistance { get; set; }



    public void SetMarkerPositions(float standardDistance)
    {
        AheadSD.transform.localPosition = new Vector3(0, 0, standardDistance);
        AheadDSD.transform.localPosition = new Vector3(0, 0, standardDistance * 2);
        AsternSD.transform.localPosition = new Vector3(0, 0, -standardDistance);
        AsternDSD.transform.localPosition = new Vector3(0, 0, -standardDistance * 2);
        PortBeamSD.transform.localPosition = new Vector3(-standardDistance, 0, 0);
        PortBeamDSD.transform.localPosition = new Vector3(-standardDistance * 2, 0, 0);
        StarboardBeamSD.transform.localPosition = new Vector3(standardDistance, 0, 0);
        StarboardBeamDSD.transform.localPosition = new Vector3(standardDistance * 2, 0, 0);
    }

	// Use this for initialization
	void Start ()
    {

    }
	
	// Update is called once per frame
	void Update ()
    {

    }
}
