using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class LidarSensor : MonoBehaviour
{
    /// <summary>
    /// the direction from the center of the robot
    /// that this sensor is pointing in.
    /// Likely this would be precalculted and known
    /// in a real robot to save calculation speed
    /// </summary>
   
    public Vector3 pointingInDirection;

    [SerializeField]
    public float maxDistance = 10f;

    public void Start()
    {

    }

    /// <summary>
    /// check the lidar sensor for how far away a wall is
    /// if no wall is hit returns maximum distance for a wall to be at
    /// </summary>
    /// <returns></returns>
    public float getDistanceToWall()
    {
        RaycastHit hit;
        float distanceToWall = maxDistance;
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, Physics.AllLayers))
        {
            if(hit.distance > maxDistance) distanceToWall = maxDistance;
            else distanceToWall = hit.distance;
        }
        return distanceToWall;
    }

    /// <summary>
    /// draw a debug line out in the direction this lidar sensor faces, stopping when it hits a wall
    /// </summary>
    public void drawDebugLine(Color color, float yOffset = 0)
    {
        // Calculate the start position (the object's position)
        Vector3 startPosition = new Vector3(transform.position.x, transform.position.y+yOffset, transform.position.z);

        // Calculate the end position (forward direction multiplied by line length)
        Vector3 endPosition = startPosition + (getDirection() * getDistanceToWall());

        // Draw the line in the Scene view
        Debug.DrawLine(startPosition, endPosition, color);
    }

    /// <summary>
    /// get the world space direction this sensor is looking in
    /// </summary>
    /// <returns></returns>
    public Vector3 getDirection()
    {
        // Calculate the start position (the object's position)
        Vector3 startPosition = transform.position;

        // Calculate the end position (forward direction multiplied by line length)
        Vector3 endPosition = startPosition + (transform.forward * 10);

        pointingInDirection = (endPosition - startPosition).normalized;
        return pointingInDirection;
    }

}
