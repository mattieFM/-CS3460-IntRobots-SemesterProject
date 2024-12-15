using UnityEngine;
using System.Linq;

public class PotentialField : MonoBehaviour
{
    public float repulsionStrength = 35f;  // Strength of repulsion from obstacles
    public float attractionStrength = 1.1f;  // Strength of attraction towards the goal
    public float maxSensorRange = 10f;  // Max sensor range

    public LidarSensor[] lidarSensors;
    public bool showDebugLines = true;  // Toggle to show debug lines
    private float wall_thresh = 2;

    public void Start()
    {
        lidarSensors = global.robot.GetComponent<robotController>().lidarSensors;  // Array of sensors
    }

    void Update()
    {
        // Calculate the total force
        Vector3 totalForce = CalculateAttractiveForce() + CalculateRepulsiveForce();

        // Apply the total force to move the robot
        // MoveRobot(totalForce);

        // Optionally, show debug lines for visualization
        if (showDebugLines)
        {
            ShowDebugLines();
        }
    }

    // Attractive Force: Pulls the robot towards the goal
    public Vector3 CalculateAttractiveForce()
    {
        Vector3 directionToGoal = global.goal.transform.position - transform.position;
        float distanceToGoal = directionToGoal.magnitude;
        directionToGoal.Normalize();

        // The force is inversely proportional to the distance to the goal
        Vector3 attractiveForce = directionToGoal * attractionStrength * distanceToGoal;
        return attractiveForce;
    }

    // Repulsive Force: Pushes the robot away from obstacles detected by the sensors
    public Vector3 CalculateRepulsiveForce()
    {
        Vector3 repulsiveForce = Vector3.zero;

        foreach (var sensor in lidarSensors)
        {
            if (sensor.getDistanceToWall() < wall_thresh)  // 30 degrees is an example
            {
                Vector3 sensorPosition = sensor.transform.position;
                Vector3 directionToSensor = sensorPosition - transform.position;
                float distanceToSensor = sensor.getDistanceToWall();

                // Calculate the repulsive force
                float forceMagnitude = repulsionStrength / distanceToSensor;
                repulsiveForce -= directionToSensor.normalized * forceMagnitude;
            }
        }

        return repulsiveForce;
    }

    void ShowDebugLines()
    {
        // Draw attractive force direction
        Debug.DrawLine(transform.position, transform.position + CalculateAttractiveForce(), Color.green);

        // Draw repulsive force direction
        Debug.DrawLine(transform.position, transform.position + CalculateRepulsiveForce(), Color.red);

        // Draw total force direction
        Debug.DrawLine(transform.position, transform.position + CalculateRepulsiveForce()+ CalculateAttractiveForce(), Color.magenta);
    }
}
