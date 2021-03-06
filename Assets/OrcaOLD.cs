using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class OrcaOLD : MonoBehaviour
{
    //private variables
    private Rigidbody shipPhysicsRigidBody;
    private Vector3 turnStartPosition;
    private Vector3 turnEndPosition;

    private bool useDoubleStandardHelm = false;

    private float newCRS;
    private float baseCrs;
    private float heading;

    private float spdInKTS;
    private float SpdInYardsPerSecond;

    private float RateOfTurn;

    private float advance;
    private float transfer;
    private float dnc;

    public float keyRange;
    public float keyBearing;

    //public getters and setters
    public bool IsTurning { get { return IsTurning; } set { IsTurning = value; } }

    public float Advance { get { return advance; } }
    public float Transfer { get { return transfer; } }
    public float DNC { get { return dnc; } }

    public float SpdInKTS { get { return spdInKTS; } set { spdInKTS = value; } }
    public float NewCrs { get { return (newCRS); } set { baseCrs = newCRS; newCRS = value; turnStartPosition = this.transform.position; } }


    // Use this for initialization
    void Start()
    {
        //grab the phyi
        shipPhysicsRigidBody = GetComponent<Rigidbody>();
        //Assign the ship a course to steer
        baseCrs = newCRS;

        heading = this.transform.rotation.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!Mathf.Approximately(newCRS, heading))
        {
            //IsTurning = true;
            turn(newCRS);
            turnEndPosition = this.transform.position;
            CalculateAdvanceAndTransfer(baseCrs);
            CalculateKeyRangeAndBearing(Mathf.Abs(baseCrs - NewCrs));
        }
        else
        {
            //IsTurning = false;
        }

        SetSpeed();

        //update heading to reflect the ships present compass heading
        heading = this.transform.rotation.eulerAngles.y;
    }

    //Is called continuously to turn the ship
    public void turn(float newCRS)
    {
        CalculateTurnRate();
        shipPhysicsRigidBody.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, newCRS, 0), Time.deltaTime * RateOfTurn);
    }



    /// <summary>
    /// calculates the rate of turn based on the ships speed in knots and whether standard or double standard helm is being used.
    /// presently uses line-of-best-fit to model the orca class turning data.
    /// </summary>
    private void CalculateTurnRate()
    {
        if (spdInKTS > 10)
        {
            useDoubleStandardHelm = false;
        }
        else
        {
            useDoubleStandardHelm = true;
        }
        if (!useDoubleStandardHelm)
        {
            RateOfTurn = Mathf.Abs(Mathf.Sqrt(spdInKTS) - 1.7f);
        }
        else
        {
            //piecewise function, prior to 15 KTS, the change in rate of turn resembles a sloped line, beyond 15 kts it plateus around 4.1 degrees/sec
            if (spdInKTS < 15)
            {
                RateOfTurn = (.32f * spdInKTS) - .7f;
            }
            else
            {
                RateOfTurn = 4.1f;
            }
        }
    }

    private void CalculateAdvanceAndTransfer(float baseCrs)
    {
        //find theta, unity's sin and cosine functions use radians, we must find the angle between the base course and the new course
        Vector2 direction = new Vector2(turnEndPosition.x - turnStartPosition.x, turnEndPosition.z - turnStartPosition.z);
        float thetaInRads = ((Mathf.Deg2Rad * baseCrs) - Mathf.Atan2(direction.x, direction.y)) * -1f;

        //The hypotenuse will always just be the distance between when the turn begins and ends (DNC)
        float hypotenuse = Vector3.Distance(turnStartPosition, turnEndPosition);

        //the rest can be found with trig
        float opposite = Mathf.Sin(thetaInRads) * hypotenuse;
        float adjancent = Mathf.Cos(thetaInRads) * hypotenuse;

        //assign the corresponding parts of the triangle to the relevent measurements
        advance = adjancent;
        transfer = opposite;
        dnc = hypotenuse;
    }

    public void CalculateKeyRangeAndBearing(float alterationSize)
    {
        if (alterationSize < 30)
        {
            RelVelCalc.CalculateKeyRangeAndBearing(Advance, FormationData.Instance.guideCRS, FormationData.Instance.guideBears, FormationData.Instance.rangeToGuideStartInYards, FormationData.Instance.guideWillBear, FormationData.Instance.rangetoGuideEndInYards, out keyRange, out keyBearing);
        }
        else if (alterationSize > 30 && alterationSize < 60)
        {
            RelVelCalc.CalculateKeyRangeAndBearing(DNC, FormationData.Instance.guideCRS, FormationData.Instance.guideBears, FormationData.Instance.rangeToGuideStartInYards, FormationData.Instance.guideWillBear, FormationData.Instance.rangetoGuideEndInYards, out keyRange, out keyBearing);
        }
        else if (alterationSize > 60 && alterationSize < 90)
        {
            RelVelCalc.CalculateKeyRangeAndBearing(Transfer, FormationData.Instance.guideCRS, FormationData.Instance.guideBears, FormationData.Instance.rangeToGuideStartInYards, FormationData.Instance.guideWillBear, FormationData.Instance.rangetoGuideEndInYards, out keyRange, out keyBearing);
        }
        else
        {
            RelVelCalc.CalculateKeyRangeAndBearing(DNC, FormationData.Instance.guideCRS, FormationData.Instance.guideBears, FormationData.Instance.rangeToGuideStartInYards, FormationData.Instance.guideWillBear, FormationData.Instance.rangetoGuideEndInYards, out keyRange, out keyBearing);
        }
    }
        /// <summary>
        /// Converts Speed in Knots to equivelant Unity speed  units (yards per second)
        /// </summary>
    public void SetSpeed()
    {
        SpdInYardsPerSecond = spdInKTS * 0.56f;
        shipPhysicsRigidBody.velocity = transform.forward *SpdInYardsPerSecond;
    }

}
