using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {PORT, STARBOARD, MIDSHIPS }
public enum Helm { STANDARD, DOUBLE_STANDARD}
public enum Engine { PORT, STARBOARD, BOTH}
public enum DriveMode { TWO_GT_CROSSCON, PORT_GT_CROSSCON, SBD_GT_CROSSCON, PDE_CROSSCON}


interface IAzipod
{
    void RotatePort(float direction);
    void RotateStarboard(float  direction);
}



public abstract class Ship : MonoBehaviour
{
    // Use this for initialization
    public abstract void Start();
    // Update is called once per frame
    public abstract void Update();

    /// <summary>
    /// Use a propulsion method to move the ship
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="newSpeed"></param>
    public abstract void SetSpeed(Engine engine, float newSpeed);

    /// <summary>
    /// steers the ship at an exact heading
    /// </summary>
    /// <param name="helmAmount"></param>
    /// <param name="course"></param>
    public abstract void Steer(float course);

    public abstract void Steer(Direction direction, float course);

    public abstract void Accelerate();
}
