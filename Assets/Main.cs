using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class Main : MonoBehaviour
{
    //This program is designed to calculate RELVEL for RCN students
    // Use this for initialization


    //controls the guide ship
    public StationMark marker;
    public Ship OwnShip;
    public Ship GuideShip;
    public Ship[] formation = new Ship[3];
    public Canvas UI;
    public RelvelVectorDisplay VectorDisplay;

    public Text CTSText;
    public Text SpeedText;
    public Text KB;
    public Text KR;

    public Dropdown DesiredSpeed;
    public Dropdown RangeToGuideField;
    public Dropdown NewGuideDistanceField;


    public InputField GWBField;
    public InputField GuideBearsField;
    public InputField GuideSpdField;
    public InputField GuideCRSField;
    public InputField SSpeedField;
    public InputField StandardDistanceField;

    public CompassRing compassRing;

    public List<InputField> FormationStateFields;
    public List<InputField> RelVelCalcFields;

    //public InputField[] FormationStateFields;

    public Button CalculateButton;
    public Button ExecuteButton;
    public Button ResetButton;
    public Button SetFormationStateButton;

    //Private Variables necessary for relvel calculations
    private float StandardDistance;
    private float SSpeed;
    private float GuideCRS;
    private float GuideSPD;
    private float GuideBears;
    private float RangeToGuide;
    private float GWB;
    private float NewGuideDistance;

    private bool solutionExists;
    private bool formationStateSet;

    //Output Values - Move these later
    public float CTSAngle = new float();
    public Vector2 CTS;

    float distancetoguide;
    float guidebearingfromownship;



    //Initialize values here
    void Start()
    {
        //turnCompleteEvt.advanceChanged += new ShipEventHandler(CalculateKeyRangeAndBearing);

        UI = GetComponent<Canvas>();
        CalculateButton.interactable = false;
        ExecuteButton.interactable = false;
        ResetButton.interactable = false;
        formationStateSet = false;
        ResetInterfaceText();
        ToggleRelvelControls(false);

        FormationStateFields.Add(GuideCRSField);
        FormationStateFields.Add(GuideSpdField);
        FormationStateFields.Add(SSpeedField);
        FormationStateFields.Add(StandardDistanceField);
        marker.MoveToPosition(new Vector3(OwnShip.transform.position.x, 55, OwnShip.transform.position.z));
        RelVelCalcFields.Add(GWBField);
        RelVelCalcFields.Add(GuideBearsField);
        SetFormationState();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Gets called by the unity canvas event system every time one of the Formation State field values are altered
    /// </summary>
    public void FieldValueAltered()
    {
        ResetInterfaceText();
        AllInputFieldsValid(FormationStateFields);
        SceneSetup();
        compassRing.ChangeRadius(SSpeed * 100);
        ExecuteButton.interactable = false;
    }

    /// <summary>
    /// Gets called via the Unity Canvas event system every time it detects an alteration of the "maneuvering board" values
    /// </summary>
    public void RelvelValueAltered()
    {
        ResetInterfaceText();
        float.TryParse(GuideBearsField.text, out GuideBears);
        float.TryParse(GWBField.text, out GWB);

        FormationData.Instance.guideWillBear = GWB;
        FormationData.Instance.guideBears = GuideBears;

        if (RangeToGuideField.value == 1)
        {
            RangeToGuide = StandardDistance * 2;
        }
        else
        {
            RangeToGuide = StandardDistance;
        }
        if (NewGuideDistanceField.value == 1)
        {
            NewGuideDistance = StandardDistance * 2;
        }
        else
        {
            NewGuideDistance = StandardDistance;
        }

        FormationData.Instance.rangeToGuideStartInYards = RangeToGuide;
        FormationData.Instance.rangetoGuideEndInYards = NewGuideDistance;

        AllInputFieldsValid(FormationStateFields);
        AllInputFieldsValid(RelVelCalcFields);
        MoveShipToStartPosition(OwnShip);
        ExecuteButton.interactable = false;
        SceneSetup();

        Vector2 temp = InputToVector(GWB - GuideCRS, NewGuideDistance);
        marker.MoveToPosition(new Vector3(temp.x * -1, 55, temp.y * -1));
        //VectorDisplay.clearTrueCTSVector();
        VectorDisplay.EditRelativeVector(InputToVector(GuideBears - GuideCRS, RangeToGuide), InputToVector(GWB - GuideCRS, NewGuideDistance));

    }

    /// <summary>
    /// Checks to ensure none of the Formation State fields have been left blank,  if the fields are blank, the calculate button will remain locked, if all fields have valid inputs the calculate button becomes unlocked
    /// </summary>
    /// <returns></returns>
    private bool AllInputFieldsValid(List<InputField> list)
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

    /// <summary>
    /// Disables or enables all interface controls relating to the "maneuvering board"
    /// </summary>
    /// <param name="state"></param>
    public void ToggleRelvelControls(bool state)
    {
        GWBField.interactable = state;
        GuideBearsField.interactable = state;
        NewGuideDistanceField.interactable = state;
        RangeToGuideField.interactable = state;
        DesiredSpeed.interactable = state;
    }

    /// <summary>
    /// Disables or enables the execute and calculate buttons
    /// </summary>
    /// <param name="state"></param>
    public void ToggleActionControls(bool state)
    {
        ExecuteButton.interactable = state;
        CalculateButton.interactable = state;
    }

    public void ToggleFormationStateControls(bool state)
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

    /// <summary>
    /// Resets the position of all ships to their pre-maneuver position, but doesn't clear the variables in the input fields
    /// </summary>
    public void ResetButtonPress()
    {
        ResetInterfaceText();
        //OwnShip.NewCrs = GuideCRS;
       // OwnShip.SpdInKTS = GuideSPD;
        ToggleRelvelControls(true);
        CalculateButton.interactable = true;
        ExecuteButton.interactable = false;
        SceneSetup();
        MoveShipToStartPosition(OwnShip);
        SetFormationStateButton.interactable = true;
        ResetButton.interactable = false;
        ResetAllShips();
    }

    public void ResetAllShips()
    {
        foreach (Ship ship in formation)
        {
            ship.transform.localEulerAngles = new Vector3(ship.transform.localEulerAngles.x, Mathf.Abs(GuideCRS % 360), ship.transform.localEulerAngles.z);
          //  ship.NewCrs = GuideCRS;
            //ship.SpdInKTS = GuideSPD;
        }
    }

    /// <summary>
    /// This is Step 1
    /// When the calculate button is pressed, this method will first check all text fields to ensure none are left empty, then pull all the values from those fields and place them in variables that will be used to calculate a relvel solution
    /// It will call scene setup to place everything in its starting positions, and then call Calculate Relvel and attempt to calculate a solution, if no valid relvel solution exists (EG inifinity, null, or  Not a Number type error) an errpr message will be displayed.
    /// </summary>
    public void CalculateButtonPress()
    {
        CalculateRVel();
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
    /// ALters the displayed values for the text on the right hand interface
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="speed"></param>
    /// <param name="keyRange"></param>
    /// <param name="keyBearing"></param>
    private void UpdateInterfaceText(float angle, float speed, float keyRange, float keyBearing)
    {
        CTSText.text = "CTS: " + CTSAngle.ToString() + "°";
        SpeedText.text = "SPD: " + speed.ToString() + "KTS";
        KR.text = "KR: " + keyRange.ToString() + " yards";
        KB.text = "KB: " + keyBearing.ToString() + "°";
    }

    /// <summary>
    /// First checks to make sure that none of the fields in the Formation State menu have been left blank, then pulls those values and uses them to initialize the variables that are used by the calculator. 
    /// Then locks the fields so that they cannot be accidentally altered mid calculation. Calling this function a second time unlocks the fields for editing. While the fields are being edited, no other commands can be called from the interface. 
    /// </summary>
    public void SetFormationState()
    {
        if (!formationStateSet)
        {
            ResetInterfaceText();
            //Check to make sure there are no blank fields/invalid inputs
            if (AllInputFieldsValid(FormationStateFields) == false)
            {
                //if fields are empty display error message and exit the function
                print("Error, Formation State Fields have been left blank");
                return;
            }

            //Pull numerical values from the interface input fields, and store them in the variables that are used by the rel vel calculation formula
            float.TryParse(SSpeedField.text, out SSpeed);
            float.TryParse(GuideCRSField.text, out GuideCRS);
            float.TryParse(GuideSpdField.text, out GuideSPD);
            float.TryParse(StandardDistanceField.text, out StandardDistance);
            float.TryParse(GuideBearsField.text, out GuideBears);
            float.TryParse(GWBField.text, out GWB);

            FormationData.Instance.guideCRS = GuideCRS;
            FormationData.Instance.guideSpd = GuideSPD;
            FormationData.Instance.guideWillBear = GWB;
            FormationData.Instance.guideBears = GuideBears;

            //If double standard distance is selected, multiply standard distance by 2
            if (RangeToGuideField.value == 1)
            {
                RangeToGuide = StandardDistance * 2;
            }
            else
            {
                RangeToGuide = StandardDistance;
            }
            if (NewGuideDistanceField.value == 1)
            {
                NewGuideDistance = StandardDistance * 2;
            }
            else
            {
                NewGuideDistance = StandardDistance;
            }

            FormationData.Instance.rangeToGuideStartInYards = RangeToGuide;
            FormationData.Instance.rangetoGuideEndInYards = NewGuideDistance;

            //Reset all ships gets called to ensure that the ships start off in the correct positions and orientations (for example, if guide course isn't north, or an unusual standard distance is being used)
            ResetAllShips();

            //When set to true, this boolean value causes this function to execute the code in the else statement next time it is called
            formationStateSet = true;

            //enable the calculate button and the relvel menu controls, lock the formation state fields from futher editing
            CalculateButton.interactable = true;
            ToggleRelvelControls(true);
            ToggleFormationStateControls(false);

            //VectorDisplay.clearRelativeVector();
           // VectorDisplay.clearTrueCTSVector();
            VectorDisplay.EditGuideVector(GuideSPD, GuideCRS);
        }
        else
        {
            //unlock the formation state fields for editing, lock all other interface controls so they cannot be triggered erroneously
            ToggleRelvelControls(false);
            ToggleActionControls(false);
            formationStateSet = false;
            ToggleFormationStateControls(true);
        }
    }

    /// <summary>
    /// Move a Specific ship to its specified starting position, RIGHT NOW IS ONLY DESIGNED TO WORK WITH OWNSHIP
    /// </summary>
    /// <param name="ship"></param>
    public void MoveShipToStartPosition(Ship ship)
    {
        Vector2 tempV2 = InputToVector(GuideBears, RangeToGuide) * -1;
        Vector3 tempV3 = new Vector3(tempV2.x, 0, tempV2.y);
        tempV3 += GuideShip.transform.position;
        ship.transform.position = tempV3;
    }


    /// <summary>
    /// Should be called prior to execution any time the Variables change
    /// </summary>
    public void SceneSetup()
    {
        Vector2 tempV2 = InputToVector(GuideBears, RangeToGuide) * -1;
        Vector3 tempV3 = new Vector3(tempV2.x, 0, tempV2.y);
        tempV3 += GuideShip.transform.position;
        solutionExists = false;

        foreach (Ship unit in formation)
        {
            TrailRenderer temp = unit.GetComponent<TrailRenderer>();
            temp.Clear();
        }
    }



    /// <summary>
    /// this is a simple utility  function I wrote which takes compass bearings and velocities (values which we work with in reality) and converts them into velocity vectors usable in trigenometry and vector math
    /// </summary>
    /// <param name="bearing"></param> Any compass bearing between 0 and 360 deg
    /// <param name="velocity"></param> Velocity  in Knots
    /// <returns></returns>
    public Vector2 InputToVector(float bearing, float velocity)
    {
        //This ensures values in excess of 360 degrees are truncated to values that only appear on a compass ring, eg. 369 degrees becomes 9 degrees 720 degrees becomes 0  degrees, etc... I will also clamp the input value so it doesn't exceed 360, this is just an added layer of protection
        if (bearing >= 360)
        {
            bearing = bearing % 360;
        }

        float brgInRadians = bearing * Mathf.Deg2Rad;
        Vector2 velocityVector = new Vector2();
        velocityVector.x = Mathf.Sin(brgInRadians);
        velocityVector.y = Mathf.Cos(brgInRadians);
        velocityVector = velocityVector * velocity;
        return velocityVector;
    }

    /// <summary>
    /// Vector math and trigenometry are  used to determine which course and speed are needed to complete the maneuver
    /// </summary>
    public void CalculateRVel()
    {
        solutionExists = false;

        //Calculate the distance and angle of the relative
       // GuideSPD = GuideShip.SpdInKTS;

        Vector2 guideCrsAndSpd = new Vector2();
        Vector2 guidePos = new Vector2();
        Vector2 guideWillBear = new Vector2();


        guideCrsAndSpd = InputToVector(GuideCRS, GuideSPD);
        guidePos = InputToVector(GuideBears, RangeToGuide);
        guideWillBear = InputToVector(GWB, NewGuideDistance);

        float relDistance = Vector2.Distance(guidePos, guideWillBear);

        //find the cartesian slope of the relative
        Vector2 direction = guideWillBear - guidePos;

        //convert the slope into degrees
        float relAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        if (relAngle < 0)
        {
            relAngle += 360;
        }

        //Convert relative slope to same scale as guide velocity, assume 100:1 scale for now CHANGE LATER
        direction = direction / 100;

        //Calculate the Course to Steer
        //We need the reciprocal of the relative direction vector, it works on the relvel board, but not for vector addition equations
        direction = direction * -1;

        //find the angle between the guide course and speed velocity vector and the relative course and speed velocity vector
        Vector2 test = new Vector2();
        test = guideCrsAndSpd * -1;

        float angleOne = Mathf.Atan2(test.x, test.y) * Mathf.Rad2Deg;
        if (angleOne < 0)
        {
            angleOne += 360;
        }
        float angleTwo = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        float angleTheta = angleOne - angleTwo;
        // print(angleTheta);

        //Now we can use Sin law to find the optimal relative velocity vector for standard solution
        //Initialize the variables, We know two sides and one angle in advance
        float A = 0;
        float B = 0;
        float C = angleTheta;
        float a = 0;
        float b = guideCrsAndSpd.magnitude;
        float c = 0;

        //refactor this code later to make it more neat
        if (DesiredSpeed.value == 0)
        {
            c = SSpeed;
        }
        else if (DesiredSpeed.value == 1)
        {
            c = GuideSPD;
        }
        else if (DesiredSpeed.value == 2)
        {
            c = 4;
        }
        else
        {
            print("Possible off by one or null reference error for desired speed");
        }


        //solve for the sin constant
        float constant = c / Mathf.Sin(C * Mathf.Deg2Rad);
        //Now we have enough variables to solve for angle B
        B = Mathf.Asin(b / constant) * Mathf.Rad2Deg;
        //now we can find the missing angle A
        A = 180 - (B + C);
        //now we have enough information to Solve for a
        a = Mathf.Sin(A * Mathf.Deg2Rad) * constant;

        //adjust the direction vector for optimal speed
        Vector2 speedAdjust = direction.normalized * a;
        CTS = guideCrsAndSpd + speedAdjust;

        //sanity check to ensure valid numbers are being output
        if (float.IsNaN(CTS.x) || float.IsNaN(CTS.y) || float.IsNaN(c))
        {
            return;
        }


        CTSAngle = (Mathf.Atan2(CTS.x, CTS.y) * Mathf.Rad2Deg);
        if (CTSAngle < 0)
        {
            CTSAngle += 360;
        }

        VectorDisplay.EditCTSVector(InputToVector(CTSAngle - GuideCRS, c));

        //Quick sanity check to eliminate revel solutions which do not result in a course or speed alteration. (this can occur when for example, the starting bearing and destination bearing are set to be the same, or in cases where the relvel "triangle" calculated  is actually a straight line)
        float angleCheck = GuideCRS - CTSAngle;
        angleCheck = Mathf.Abs(angleCheck);
        float speedCheck = GuideSPD - CTS.magnitude;
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

            solutionExists = true;
            ExecuteButton.interactable = true;
        }
        else
        {
            //change this to a popup message once that functionality is in place
            print("error! the paramers entered do not allow for a valid relvel solution (physically impossible)");
            ResetInterfaceText();
            solutionExists = false;
            ExecuteButton.interactable = false;
        }
    }


    /// <summary>
    /// Checks to see if calculate revel was able to calculate a valid maneuver, and if so, tells our ship to execute the calculated maneuver
    /// </summary>
    public void Execute()
    {
        if (solutionExists == true)
        {
            //if a valid relvel solution exists, then have the player ship execute that set of maneuvers
           // OwnShip.NewCrs = CTSAngle;
           // OwnShip.SpdInKTS = CTS.magnitude;

            //Lock all interface elements except for the reset button during the simulation so that values cannot be erroneously altered potentially creating bugs
            ResetButton.interactable = true;
            ToggleRelvelControls(false);
            ToggleActionControls(false);
            SetFormationStateButton.interactable = false;
        }
        else
        {
            //change this to a popup message once that functionality is in place
            print("No valid relvel solution for these given variables, adjust parameters and try again");
        }
    }
}
