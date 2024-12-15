using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class global
{


    /// <summary>
    /// show blue debug lines
    /// </summary>
    public static bool showDebugLines = true;

    /// <summary>
    /// is the potential field pathfinding approach enabled
    /// </summary>
    public static bool potentialFieldEnabled = false;

    /// <summary>
    /// is the a start search algorithm enabled.
    /// </summary>
    public static bool partialAStar = false;

    /// <summary>
    /// is the a left wall follower algorithm enabled.
    /// </summary>
    public static bool leftWallFollower = false;

    /// <summary>
    /// is the a navMesh algorithm enabled.
    /// </summary>
    public static bool navMesh = false;


    /// <summary>
    /// is the a A Star algorithm enabled.
    /// </summary>
    /// 
    public static bool aStar = false;
    /// <summary>
    /// the actual maze object in the scene should one exist
    /// </summary>
    public static GameObject mazeObj;

    /// <summary>
    /// the maze generator containing the real information about the current maze
    /// obviously if the agent acsesses this that is cheating in every regard.
    /// </summary>
    public static BasicMazeGenerator mazeGenerator;

    /// <summary>
    /// the destination goal of this maze
    /// </summary>
    public static GameObject goal;

    /// <summary>
    /// an array of the possible locations to spawn our robot
    /// </summary>
    public static List<Vector3> possibleSpawns = new List<Vector3>();

    /// <summary>
    /// the private spawn loaction
    /// </summary>
    private static Vector3 _spawn;

    /// <summary>
    /// the active robot for the scene
    /// </summary>
    public static GameObject robot;


    public static float energy_consumed = 0;

    public static float speed = 0;

    public static float energy_consumed_this_tick = 0;

    public static float dist = 0;


    public static float energy_max = 1000;

    public static void calulate_energy_used_this_tick(float r1, float w1, float r2, float w2)
    {
        global.speed = Mathf.Abs((r1 + r2) / 2);
        global.energy_consumed_this_tick =( Mathf.Abs(r1 + w1) + Mathf.Abs(r2 * w2) * Time.deltaTime) /1000;
        global.energy_consumed += global.energy_consumed_this_tick;


    }


    /// <summary>
    /// the spawn location for our robot
    /// </summary>
    public static Vector3 spawn
    {
        get {
            if (_spawn == Vector3.zero) _spawn = possibleSpawns[Random.Range(0, possibleSpawns.ToArray().Length)];
            return _spawn; 
        } // Getter
        set { _spawn = value; } // Setter
    }

}
