using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Progress;

public class LeftWallFollower {

    private LidarSensor[] lidarSensors;
    private GameObject gameObject;
    private robotController robot;

    /// <summary>
    /// how close can the robot get to a wall before the robot trys to turn.
    /// </summary>
    public float turnThreshold = 3.8f;

    /// <summary>
    /// how close can the robot get to a wall before it trys to back away
    /// </summary>
    public float backupThreshhold = 1.5f;
    public LeftWallFollower(LidarSensor[] lidarSensors, GameObject gameObject, robotController robot)
    {
        this.lidarSensors = lidarSensors;
        this.gameObject = gameObject;
        this.robot = robot;
    }

    private string state = "";
    private string lastState = "";

    int msInThisState = 0;
    float targetRot;



    /// <summary>
    /// return the left and right velocity values for the wheels this tick
    /// </summary>
    /// <returns>tuiple (left,right)</returns>
    public (float, float) getDirection()
    {

        return(0f,0f);
        global.navMesh = true;

        float speed = .5f;

        //by default go forward
        Vector3 target_destination = robot.transform.position + robot.transform.forward * speed;

        //if wall ahead and right go left
        if(robot.checkForWall(gameObject.transform.forward, 30,4) && robot.checkForWall(gameObject.transform.right, 30,4))
        {
            target_destination = robot.transform.position + -gameObject.transform.right * speed;
        }

        //if wall ahead and left go right
        if (robot.checkForWall(gameObject.transform.forward, 30,4) && robot.checkForWall(-gameObject.transform.right, 30,4))
        {
            target_destination = robot.transform.position + gameObject.transform.right * speed;
        }

        


        global.robot.GetComponent<NavMeshAgent>().enabled = true;
        global.robot.GetComponent<NavMeshAgent>().destination = target_destination;





        return (0, 0);
    }
}
