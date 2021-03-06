using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is essentially a collection of common nautical and rel-vel calculations
/// </summary>
public static class RelVelCalc
{
    /// <summary>
    /// self explanatory
    /// </summary>
    /// <param name="yards">The value in yards to be converted</param>
    /// <returns></returns>
    public static float Yards2NM(float yards)
    {
        return yards/2025.373f;
    }

    /// <summary>
    /// self explanatory
    /// </summary>
    /// <param name="NM">The value in nautical miles to be converted</param>
    /// <returns></returns>
    public static float NM2Yards(float NM)
    {
        return NM * 2025.373f;
    }

    /// <summary>
    /// A function which takes an 2d direction and a magnitude and creates a vector
    /// </summary>
    /// <param name="angle">Any compass bearing between 0 and 360 deg</param> 
    /// <param name="magnitude">The magnitude of the vector. Eg. A velocity or distance</param> 
    /// <returns></returns>
    public static Vector2 AngleAndMagnitudeToVector2D(float angle, float magnitude)
    {
        //This ensures values in excess of 360 degrees are truncated to values that only appear on a compass ring, eg. 369 degrees becomes 9 degrees 720 degrees becomes 0  degrees, etc... I will also clamp the input value so it doesn't exceed 360, this is just an added layer of protection
        if (angle >= 360)
        {
            angle = angle % 360;
        }

        float angleInRads = angle * Mathf.Deg2Rad;
        Vector2 outputVector = new Vector2();
        outputVector.x = Mathf.Sin(angleInRads);
        outputVector.y = Mathf.Cos(angleInRads);
        outputVector = outputVector * magnitude;
        return outputVector;
    }

    /// <summary>
    /// Calculates the true bearing and range of one object from another
    /// </summary>
    /// <param name="bearingFrom">the object from which the bearing and range is being measured</param>
    /// <param name="bearingTo">the object to which the bearing and range is being measured</param>
    /// <param name="bearing">the bearing between objects</param>
    /// <param name="range">the range between objects</param>
    public static void TrueBearingAndRangeFrom(Vector3 bearingFrom, Vector3 bearingTo, out float bearing, out float range)
    {
        Vector2 fromObject;
        Vector2 toObject;

        fromObject.x = bearingFrom.x;
        fromObject.y = bearingFrom.z;

        toObject.x = bearingTo.x;
        toObject.y = bearingTo.z;

        Vector2 bearingVector = toObject - fromObject;


        bearing = (Mathf.Rad2Deg * Mathf.Atan2(bearingVector.x, bearingVector.y));
        range = Vector3.Distance(bearingFrom, bearingTo);
        if (bearing < 0)
        {
            bearing += 360;
        }
    }


    /// <summary>
    /// A method which calculates the key range and bearing of a relvel solution, the point along the line of a ships relative movement where it must return to its base course
    /// </summary>
    /// <param name="offset">The Advance, DNC, or Transfer of the turn in yards</param>
    /// <param name="GuideCRS">The guides present course</param>
    /// <param name="GuideBears">what the guide bears at the beginning of the maneuver</param>
    /// <param name="RangeToGuide">the range to the guide at the beginning of the maneuver</param>
    /// <param name="GuideWIllBear">What the Guide will bear at the end of the maneuver</param>
    /// <param name="NewRangeToGuide">The range to the guide at the end of the maneuver</param>
    public static void CalculateKeyRangeAndBearing(float offset, float GuideCRS, float GuideBears, float RangeToGuide, float GuideWIllBear, float NewRangeToGuide, out float keyRange, out float keyBearing)
    {
        //Debug.Log("offset: " + offset.ToString());
        //establish the start and end points
        Vector2 relStart = AngleAndMagnitudeToVector2D(GuideBears - GuideCRS, RangeToGuide);
        Vector2 relEnd = AngleAndMagnitudeToVector2D(GuideWIllBear - GuideCRS, NewRangeToGuide);

        //find the slope of the line between two points (the relative)
        Vector2 relativeDelta = relEnd - relStart;

        //take the overall length of this line and subtract advance/transfer/dnc (offset)
        float magnitude = relativeDelta.magnitude - offset;
        

        //build a new line using the normalized slope of the old line and the calculated length of the new line
        Vector2 tempRelative = relativeDelta.normalized;
        tempRelative = tempRelative * magnitude;

        //fuck I should have paid closer attention in grade 12 calculus... this finds the end point of the line... I think?
        Vector2 finalPoint = tempRelative + relStart;
        //Debug.Log(finalPoint);
        //the angle from the origin to this point is the key bearing, and the distance from the origin to this point should be key range...
        keyRange = Vector2.Distance(Vector2.zero, finalPoint);
        keyBearing = Mathf.Rad2Deg * (Mathf.Atan2(finalPoint.x, finalPoint.y));
        if (keyBearing < 0)
        {
            keyBearing += 360;
        }
    }

    private static void CalculateAdvanceAndTransfer(float baseCrs, Vector3 turnStartPosition, Vector3 turnEndPosition, out float advance, out float transfer, out float dnc)
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

/// <summary>
/// Solves relvel problems giving a course to station for a given speed
/// </summary>
/// <param name="maneuverSpeed">Speed at which the maneuver should be conducted</param>
    public static void CalculateRVel(float maneuverSpeed, out Vector2 CTS)
    {
        //Calculate the distance and angle of the relative
        float GuideSPD = FormationData.Instance.guideSpd;

        Vector2 guideCrsAndSpd = new Vector2();
        Vector2 guidePos = new Vector2();
        Vector2 guideWillBear = new Vector2();


        guideCrsAndSpd = AngleAndMagnitudeToVector2D(FormationData.Instance.guideCRS, FormationData.Instance.guideSpd);
        guidePos = AngleAndMagnitudeToVector2D(FormationData.Instance.guideBears, FormationData.Instance.rangeToGuideStartInYards);
        guideWillBear = AngleAndMagnitudeToVector2D(FormationData.Instance.guideWillBear, FormationData.Instance.rangetoGuideEndInYards);

        float relDistance = Vector2.Distance(guidePos, guideWillBear);

        //find the cartesian slope of the relative
        Vector2 direction = guideWillBear - guidePos;

        //convert the slope into degrees
        float relAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        if (relAngle < 0f)
        {
            relAngle += 360f;
        }

        //Convert relative slope to same scale as guide velocity, assume 100:1 scale for now CHANGE LATER
        direction = direction / 100;

        //Calculate the Course to Steer
        //We need the reciprocal of the relative direction vector, it works on the relvel board, but not for vector addition equations
        direction = direction * -1f;

        //find the angle between the guide course and speed velocity vector and the relative course and speed velocity vector
        Vector2 test = new Vector2();
        test = guideCrsAndSpd * -1f;

        float angleOne = Mathf.Atan2(test.x, test.y) * Mathf.Rad2Deg;
        if (angleOne < 0f)
        {
            angleOne += 360f;
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
        float c = maneuverSpeed;

        //solve for the sin constant
        float constant = c / Mathf.Sin(C * Mathf.Deg2Rad);
        //Now we have enough variables to solve for angle B
        B = Mathf.Asin(b / constant) * Mathf.Rad2Deg;
        //now we can find the missing angle A
        A = 180f - (B + C);
        //now we have enough information to Solve for a
        a = Mathf.Sin(A * Mathf.Deg2Rad) * constant;

        //adjust the direction vector for optimal speed
        Vector2 speedAdjust = direction.normalized * a;

        CTS = guideCrsAndSpd + speedAdjust;
    }

    public static float CompassBearingClamp(float bearing)
    {
        //no such thing as a negative compass bearing
        float temp = Mathf.Abs(bearing);
        if (temp > 360f)
        {
            //compass bearings must be beteen 0 and 360
            temp = temp % 360f;
        }
        return temp;
    }

    public static float CalculateDifferenceOfBearing(float startBearing, float endBearing, Direction direction)
    {
        //Ensure supplied values are valid compass bearings
        startBearing = CompassBearingClamp(startBearing);
        endBearing = CompassBearingClamp(endBearing);

        float difference = 0;

        switch (direction)
        {
            case Direction.PORT:
                if (startBearing < endBearing)
                { difference = Mathf.Abs((startBearing + 360) - endBearing); }
                else
                { difference = Mathf.Abs(startBearing - endBearing); }
                return difference;

            case Direction.STARBOARD:
                if (startBearing < endBearing)
                { difference = Mathf.Abs(endBearing - startBearing); }
                else
                { difference = Mathf.Abs((endBearing + 360 ) - startBearing); }
                return  difference;
            default:
                difference = 0;
                return difference;
        }
    }
}
