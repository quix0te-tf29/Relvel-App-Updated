using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orca : Ship
{
    //Ships physics body
    private Rigidbody shipPhysicsBody;
    
    //The ships speed made good in Knots
    private float speedMadeGood;
    //The ships ordered speed in Knots
    private float orderedSpeed;
    private float maxSpeed = 21f;
    private float maxReverseSpeed = -7f;

    private float shipsHead;
    private float orderedCourse;

    private float rateOfTurn;

    private bool helmOrderOn = false;

    public float SpeedMadeGood { get { return speedMadeGood; } }
    public float OrderedSpeed { get { return orderedSpeed; } set { orderedSpeed = Mathf.Clamp(value, maxReverseSpeed, maxSpeed); } }
    public float MaxSpeed { get { return maxSpeed; } }

    public float ShipsHead { get { return shipsHead; } }

    //prevent negative values or values greater than 360 degrees from being assigned as the ships head
    public float OrderedCourse { get { return orderedCourse; } set { orderedCourse = RelVelCalc.CompassBearingClamp(value); } }

    // Use this for initialization
    public override void Start()
    {
        shipPhysicsBody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public override void Update()
    {
        shipsHead = this.transform.rotation.eulerAngles.y;

        //if there is not a helm order already on
        if (!Mathf.Approximately(orderedCourse, shipsHead) && !helmOrderOn)
        {
            Steer(orderedCourse);
        }
        else
        {
            //Debug.Log("not maintaining the ordered course as there is presently a helm order on");
        }
        Accelerate();
    }

 

    /// <summary>
    /// ship will maintain a course using as much helm as needed, used when it is necessary to override the direction of the turn rather than letting the program decide (eg. when you need to turn the long way around)
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="course"></param>
    public override void Steer(Direction direction, float course)
    {
        switch (direction)
        {
            case Direction.STARBOARD:
                if (RelVelCalc.CalculateDifferenceOfBearing(shipsHead, course, Direction.STARBOARD) > 180f)
                {

                }
                else
                {
                    Steer(course);
                }
                break;
            case Direction.PORT:
                if (RelVelCalc.CalculateDifferenceOfBearing(shipsHead, course, Direction.PORT) > 180f)
                {

                }
                else
                {
                    Steer(course);
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Ship will maintain specified course using as much helm as is needed
    /// </summary>
    /// <param name="course"></param>
    public override void Steer(float course)
    {
        if (shipPhysicsBody != null)
        {
            Debug.Log("speed: " + speedMadeGood + "rate of turn: " + rateOfTurn);
            shipPhysicsBody.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, course, 0), Time.deltaTime * rateOfTurn);
        }
        else { App.Log(this, "warning, no ship physics body detected for: " + this.ToString()); }
    }


    /// <summary>
    /// maintains the ordered speed
    /// </summary>
    public override void Accelerate()
    {
        if (!Mathf.Approximately(speedMadeGood, orderedSpeed))
        {
            speedMadeGood =  Mathf.MoveTowards(speedMadeGood, orderedSpeed, Time.deltaTime/1);
        }
        if (shipPhysicsBody != null)
        {
            float SMGInYardsPerSec = speedMadeGood * .56f;
            shipPhysicsBody.velocity = transform.forward * SMGInYardsPerSec;
        }
        else { App.Log(this, "warning, no ship physics body detected for: " + this.ToString()); }
    }

    public override void SetSpeed(Engine engine, float speedOrder)
    {
        switch (engine)
        {
            case Engine.BOTH:
                OrderedSpeed = speedOrder;
                break;
            case Engine.PORT:
                break;
            case Engine.STARBOARD:
                break;
        }
    }
}
