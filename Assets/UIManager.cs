using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


class FormationData
{
    private static FormationData instance;
    private FormationData() { }

    public static FormationData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new FormationData();
            }
            return instance;
        }
    }

    public void DebugDump()
    {
        Debug.ClearDeveloperConsole();
        Debug.Log("Guide Ship: " + GuideShip);
        Debug.Log("Own Ship: : " + OwnShip);
        Debug.Log("Consort List: " + AllConsorts);
        Debug.Log("Standard Distance: " + StandardDistance.ToString());
        Debug.Log("Stationing Speed: " + StationingSpeed);
        Debug.Log("Fallback Speed: " + FallbackSpeed);
        Debug.Log("Guide Speed: " + GuideSpd);
        Debug.Log("Guide Crs: " + GuideCRS);
        Debug.Log("Guide Bears: " + GuideBears);
        Debug.Log("Guide Starting Range: " + RangeToGuideStartInYards);
        Debug.Log("Guide Will Bear: " + GuideWillBear);
        Debug.Log("Guide Ending Range: " + RangeToGuideEndInYards);
    }

    public static Ship GuideShip;
    public static Ship OwnShip;
    public static ArrayList AllConsorts;

    private float StandardDistance;
    private float StationingSpeed = 15;
    private float FallbackSpeed = 4;

    private float GuideSpd;
    private float GuideCRS;

    private float GuideBears;
    private float RangeToGuideStartInYards;
    private float RangeToGuideStartInNM;

    private float GuideWillBear;
    private float RangeToGuideEndInYards;
    private float RangeToGuideEndInNM;

    public float standardDistance { get { return StandardDistance; } set { StandardDistance = value; } }
    public float stationingSpeed { get { return StationingSpeed; } set { StationingSpeed = value; } }
    public float fallbackSpeed { get { return FallbackSpeed; } set { FallbackSpeed = value; } }

    public float guideSpd { get { return GuideSpd; } set { GuideSpd = value; } }
    public float guideCRS { get { return GuideCRS; } set { GuideCRS = value; } }

    public float guideBears { get { return GuideBears; } set { GuideBears = value; } }
    public float rangeToGuideStartInYards { get { return RangeToGuideStartInYards; } set { RangeToGuideStartInYards = value; RangeToGuideStartInNM = RelVelCalc.Yards2NM(value); } }
    public float rangeToGuideStartinNM { get { return RangeToGuideStartInNM; }set { RangeToGuideStartInNM = value; RangeToGuideStartInYards = RelVelCalc.NM2Yards(value); } }

    public float guideWillBear { get { return GuideWillBear; } set { GuideWillBear = value; } }
    public float rangetoGuideEndInYards { get { return RangeToGuideEndInYards; } set { RangeToGuideEndInYards = value; RangeToGuideEndInNM = RelVelCalc.Yards2NM(value); } }
    public float rangeToGuideEndinNM { get { return RangeToGuideEndInNM; } set { RangeToGuideEndInNM = value; RangeToGuideEndInYards = RelVelCalc.NM2Yards(value); } }
}

public interface IUserInterFaceAction
{
    void ValueChanged(float value);
}

public class UIManager : MonoBehaviour
{
    //User Interface Objects
    //UI CANVAS
    public Canvas UI;
    //REL VEL LINES THAT GET DRAWN ON THE BOARD
    public RelvelVectorDisplay VectorDisplay;
    //THE STATION MARKER
    public StationMark marker;
    //THE COMPASS RING which also shows stationing speed
    public CompassRing compassRing;
    //Ring for Equal Speed
    public SpeedRing EqualSpeedRing;
    //Ring for fallback speed
    public SpeedRing FallbackSpeedRing;

    //ALL TEXT BOXES
    public Text CTSText;
    public Text SpeedText;
    public Text KB;
    public Text KR;

    //ALL DROPDOWN MENUS
    public Dropdown SpeedOfManeuverField;
    public Dropdown RangeToGuideStartUnits;
    public Dropdown RangetoGuideEndUnits;

    //ALL BUTTONS
    public Button CalculateButton;
    public Button ExecuteButton;
    public Button ResetButton;
    public Button SetFormationStateButton;

    //ALL INPUT FIELS
    public InputField GWBField;
    public InputField GuideBearsField;
    public InputField GuideSpdField;
    public InputField GuideCRSField;
    public InputField SSpeedField;
    public InputField StandardDistanceField;

    public InputField RangeToGuideStart;
    public InputField RangeToGuideEnd;

    public List<InputField> FormationStateUIObjects;
    public List<InputField> RelVelCalcUIObjects;

    //class-specific variables
    bool settingFormationState;

    private Vector2 CTS;

    public Ship GuideShip;
    public Ship Ownship;

    // Use this for initialization
    void Start()
    {
        InitialiseUI();
        PullInterfaceData();
        CTS = new Vector2();
        App.Log(this, "Disclaimer: This application is in Beta and is therefore still undergoing testing, it may at times provide incorrect solutions, this program is not intended to replace the skills you were taught on NWO III and IV, rather, it is a teaching aid which can be used to check the validity of relvel solutions, it should not be relied upon solely");
        GuideShip.SetSpeed(Engine.BOTH, 10);
        GuideShip.Steer( Direction.PORT, 90);
        Ownship.SetSpeed(Engine.BOTH, 10);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// called at the start to initialize the interface to default values
    /// </summary>
    void InitialiseUI()
    {
        //Add the following interface controls to the Formation State Control List
        FormationStateUIObjects.Add(GuideCRSField);
        FormationStateUIObjects.Add(GuideSpdField);
        FormationStateUIObjects.Add(SSpeedField);
        FormationStateUIObjects.Add(StandardDistanceField);

        //Add the following interface controls to the Rel Vel Controls list
        RelVelCalcUIObjects.Add(GWBField);
        RelVelCalcUIObjects.Add(GuideBearsField);
        RelVelCalcUIObjects.Add(RangeToGuideStart);
        RelVelCalcUIObjects.Add(RangeToGuideEnd);

        EnableActionControls(false);
        EnableFormationStateControls(false);
        EnableRelvelControls(true);

        //One time disable of reset and execute to synchronise controls
        ResetButton.interactable = false;
        ExecuteButton.interactable = false;

        //marker.MoveToPosition(new Vector3(OwnShip.transform.position.x, 55, OwnShip.transform.position.z));
       
    }

    /// <summary>
    /// Pulls data from the user interface fields and makes it accessible to the program
    /// </summary>
    void PullInterfaceData()
    {
        //if fields are blank or contain invalid data, exit the method without copying data
        if (CheckInputFieldValidity(FormationStateUIObjects) == false | CheckInputFieldValidity(RelVelCalcUIObjects) == false)
        {
            App.Log(this, "Error, Formation State Fields have been left blank or contain invalid values");
            return;
        }

        float tempCRS;
        float tempSPD;
        float tempGB;
        float tempGWB;
        float tempSSPD;

        float tempGuideStartDistance;
        float tempGuideEndDistance;

        float.TryParse(SSpeedField.text, out tempSSPD);
        float.TryParse(GuideCRSField.text, out tempCRS);
        float.TryParse(GuideSpdField.text, out tempSPD); 
        float.TryParse(GuideBearsField.text, out tempGB); 
        float.TryParse(GWBField.text, out tempGWB);
        float.TryParse(RangeToGuideStart.text, out tempGuideStartDistance);
        float.TryParse(RangeToGuideEnd.text, out tempGuideEndDistance);

        FormationData.Instance.guideCRS = tempCRS;
        FormationData.Instance.guideSpd = tempSPD;
        FormationData.Instance.guideBears = tempGB;
        FormationData.Instance.guideWillBear = tempGWB;
        FormationData.Instance.stationingSpeed = tempSSPD;


        if (RangeToGuideStartUnits.value == 0)
        {
            FormationData.Instance.rangeToGuideStartInYards = tempGuideStartDistance;
            RangeToGuideStart.text = FormationData.Instance.rangeToGuideStartInYards.ToString();
        }
        else
        {
            //convert nm to yards for storage consistency
            FormationData.Instance.rangeToGuideStartInYards = RelVelCalc.Yards2NM(tempGuideStartDistance);
            RangeToGuideStart.text = FormationData.Instance.rangeToGuideStartinNM.ToString();
        }
        if (RangetoGuideEndUnits.value == 0)
        {
            FormationData.Instance.rangetoGuideEndInYards = tempGuideEndDistance;
        }
        else
        {
            //convert nm to yards for storage consistency
            FormationData.Instance.rangetoGuideEndInYards = RelVelCalc.Yards2NM(tempGuideEndDistance);
        }

        //Initialise speed rings
        EqualSpeedRing.radius = FormationData.Instance.guideSpd * 100;
        FallbackSpeedRing.radius = FormationData.Instance.fallbackSpeed * 100;
        compassRing.ChangeRadius(FormationData.Instance.stationingSpeed * 100);

        //Reset Vector Display
        VectorDisplay.ClearVector(LineComponent.RELVECTOR);
        VectorDisplay.ClearVector(LineComponent.CTSVECTOR);
        VectorDisplay.EditGuideVector(FormationData.Instance.guideSpd, FormationData.Instance.guideCRS);

       // FormationData.Instance.DebugDump();

        if (CheckInputFieldValidity(FormationStateUIObjects) && CheckInputFieldValidity(RelVelCalcUIObjects))
        {
            CalculateButton.interactable = true;
        }
        else
        {
            App.Log(this, "error, fields are empty or contain invalid data, check all fields and try again");
        }
    }

    /// <summary>
    /// Checks to ensure none of the Formation State fields have been left blank,  if the fields are blank, the calculate button will remain locked, if all fields have valid inputs the calculate button becomes unlocked
    /// </summary>
    /// <returns></returns>
    private bool CheckInputFieldValidity(List<InputField> list)
    {
        foreach (InputField field in list)
        {
            if (field.text == "")
            {
                CalculateButton.interactable = false;
                ExecuteButton.interactable = false;
                return false;
            }
        }
        return true;
    }

    private bool CheckForValidRelvelSolution()
    {

        return false;
    }
    /// <summary>
    /// Resets the text on the right hand interface in order to avoid confusing the user.
    /// </summary>
    private void ResetInterfaceText()
    {
        CTSText.text = "CRS: N/A";
        SpeedText.text = "SPD: N/A";
        KR.text = "KR: N/A";
        KB.text = "KB: N/A";
    }

    /// <summary>
    /// Handles when the "Set Formation State" button is clicked
    /// </summary>
    public void OnSetFormationState()
    {
        if (settingFormationState == false)
        {
            if (!CheckInputFieldValidity(RelVelCalcUIObjects))
            {
                App.Log(this, "Error, relvel fields contain invalid data or are empty, check all fields and try again");
                return;
            }

            settingFormationState = true;
            ResetInterfaceText();
            EnableRelvelControls(false);
            EnableActionControls(false);
            EnableFormationStateControls(true);
        }
        else
        {
            if (CheckInputFieldValidity(FormationStateUIObjects) && CheckInputFieldValidity(RelVelCalcUIObjects))
            {
                //if all fields contain valid data then it is safe to press calculate
                CalculateButton.interactable = true;
            }
            else
            {
                App.Log(this, "Error, Formation State contains invalid or empty fields, check all fields and try again");
                return;
            }

            settingFormationState = false;
            ResetInterfaceText();
            EnableRelvelControls(true);
            EnableFormationStateControls(false);
            PullInterfaceData();
        }
    }

    /// <summary>
    /// gets called any time one of the relvel value fields get altered
    /// </summary>
    public void OnRelvelValueAltered()
    {
        ResetInterfaceText();
        EnableActionControls(false);
        PullInterfaceData();
    }

    /// <summary>
    /// gets called whenever calculate is pressed
    /// </summary>
    public void OnCalculatePress()
    {
        //refactor this code later to make it more neat
        if (SpeedOfManeuverField.value == 0)
        {
            RelVelCalc.CalculateRVel(FormationData.Instance.stationingSpeed, out CTS);
        }
        else if (SpeedOfManeuverField.value == 1)
        {
            RelVelCalc.CalculateRVel(FormationData.Instance.guideSpd, out CTS);
        }
        else if (SpeedOfManeuverField.value == 2)
        {
            RelVelCalc.CalculateRVel(FormationData.Instance.fallbackSpeed, out CTS);
        }
        else
        {
            App.Log(this, "Possible off by one or null reference error for desired speed, send bug reports to Akiros@live.com");
            return;
        }

        //sanity check to ensure valid numbers are being output
        if (float.IsNaN(CTS.x) || float.IsNaN(CTS.y))
        {
            App.Log(this, "Error! relvel calculator has returned invalid parameters, send bug reports to Akiros@live.com");
            return;
        }

        float CTSAngle = (Mathf.Atan2(CTS.x, CTS.y) * Mathf.Rad2Deg);
        if (CTSAngle < 0)
        {
            CTSAngle += 360;
        }

        //VectorDisplay.editTrueCTSVector(RelVelCalc.AngleAndMagnitudeToVector2D(CTSAngle - FormationData.Instance.guideCRS, c));

        //Quick sanity check to eliminate revel solutions which do not result in a course or speed alteration. (this can occur when for example, the starting bearing and destination bearing are set to be the same, or in cases where the relvel "triangle" calculated  is actually a straight line)
        float angleCheck = FormationData.Instance.guideCRS - CTSAngle;
        angleCheck = Mathf.Abs(angleCheck);
        float speedCheck = FormationData.Instance.guideSpd - CTS.magnitude;
        speedCheck = Mathf.Abs(speedCheck);

        //check to see if there is any appreciable change in course or speed, if neither has changed, then no solution exists for the given parameters
        if (angleCheck > 1f || speedCheck > 1f)
        {
            CTSAngle = Mathf.Round(CTSAngle);
            float roundedSpeed = Mathf.Round(CTS.magnitude);
            if (CTSAngle < 100)
            {
                CTSText.text = "CTS: 0" + CTSAngle.ToString() + "°";
                SpeedText.text = "SPD: " + roundedSpeed.ToString() + "KTS";
            }
            else
            {
                CTSText.text = "CTS: " + CTSAngle.ToString() + "°";
                SpeedText.text = "SPD: " + roundedSpeed.ToString() + "KTS";
            }

            ExecuteButton.interactable = true;
        }
        else
        {
            //change this to a popup message once that functionality is in place
            App.Log(this, "A Relvel Solution could not be calculated for the supplied values, this typically occurs either when a maneuver is physically impossible for the given parameters (eg. abeam to ahead at fallback speed) or when the start and end locations are the same (eg. Guide bears 090 @ 500 yards, Guide will bear 090 at 500 yards) there are some rare instances where a relvel solution should be possible and this error gets thrown, please report these errors by giving as much detailed information as possible to Akiros@live.com");
            ResetInterfaceText();
            ExecuteButton.interactable = false;
        }

    }

    /// <summary>
    /// gets called whenever execute is pressed
    /// </summary>
    public void OnExecutePress()
    {
        EnableActionControls(false);
        EnableFormationStateControls(false);
        EnableActionControls(false);
        ResetButton.interactable = true;
    }

    /// <summary>
    /// gets called whenever reset is pressed
    /// </summary>
    public void OnResetPress()
    {
        ResetButton.interactable = false;
    }

    /// <summary>
    /// Disables or enables all interface controls relating to the "maneuvering board"
    /// </summary>
    /// <param name="state"></param>
    public void EnableRelvelControls(bool state)
    {
        GWBField.interactable = state;
        GuideBearsField.interactable = state;
        RangeToGuideStartUnits.interactable = state;
        RangetoGuideEndUnits.interactable = state;
        RangeToGuideEnd.interactable = state;
        RangeToGuideStart.interactable = state;
        SpeedOfManeuverField.interactable = state;
    }

    /// <summary>
    /// Disables or enables the execute and calculate buttons
    /// </summary>
    /// <param name="state"></param>
    public void EnableActionControls(bool state)
    {
        ExecuteButton.interactable = state;
        CalculateButton.interactable = state;
    }



    /// <summary>
    /// enables or disables all controls relating to the formation state panel
    /// </summary>
    /// <param name="state"></param>
    public void EnableFormationStateControls(bool state)
    {
        GuideCRSField.interactable = state;
        SSpeedField.interactable = state;
        GuideSpdField.interactable = state;
        StandardDistanceField.interactable = state;

        if (state)
        {
            Text text = SetFormationStateButton.GetComponentInChildren<Text>();
            text.text = "Set Formation";
        }
        else
        {
            Text text = SetFormationStateButton.GetComponentInChildren<Text>();
            ResetInterfaceText();
            text.text = "Edit Formation";
        }
    }
}
