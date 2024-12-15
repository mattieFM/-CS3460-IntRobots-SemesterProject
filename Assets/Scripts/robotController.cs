using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.GlobalIllumination;

public class robotController : MonoBehaviour
{
    /// <summary>
    /// the left wheel of the robot
    /// </summary>
    [SerializeField]
    public GameObject leftWheel;

    /// <summary>
    /// the right wheel of the robot
    /// </summary>
    [SerializeField]
    public GameObject rightWheel;


    public LidarSensor[] lidarSensors;

    /// <summary>
    /// the ridgid body for this object
    /// </summary>
    private Rigidbody rb;

    /// <summary>
    /// should be above 1
    /// </summary>
    private float speed = 5f;

    /// <summary>
    /// the force to be applied to the left wheel each frame
    /// </summary>
    private float _left = 0f;
    /// <summary>
    /// the force to be applied to the right wheel each frame
    /// </summary>
    private float _right = 0f;

    private LeftWallFollower leftWallFollower;

    /// <summary>
    /// allow a and d to control left and right
    /// </summary>
    bool enableUserInput = true;

    /// <summary>
    /// check if there is a wall in the specified direction, that is if a wall is seen by all sensors
    /// within x degrees of that direction at a distance equal to or lessthan wallThreshhold
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="degrees"></param>
    /// <param name="wallThreshold"></param>
    /// <returns></returns>
    public bool checkForWall(Vector3 direction, int degrees, float wallThreshold=2f)
    {
        int sensorsThatSeeTheWall = 0;

        //make a subbset only of the sensors that face that direction
        LidarSensor[] directionSubset = lidarSensors.Where(item => Vector3.Angle(item.getDirection(), direction) < degrees).ToArray();
        foreach (var item in directionSubset)
        {
            if (item.getDistanceToWall() <= wallThreshold)
            {
                sensorsThatSeeTheWall++;
            }
        }
        if (global.showDebugLines)
        {
            if (sensorsThatSeeTheWall == directionSubset.Length)
            {
                foreach (var item in directionSubset)
                {
                    item.drawDebugLine(Color.red,.1f);
                }

            }
        }

        return sensorsThatSeeTheWall == directionSubset.Length;
    }

    /// <summary>
    /// check if robot is aligned with a direction
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="tolerance"></param>
    /// <returns></returns>
    public bool IsAlignedWithDirection(Vector3 direction, float tolerance = 25f)
    {
        Vector3 forward = transform.forward;

        if (Vector3.Angle(forward.normalized, direction.normalized) < 1.0f - tolerance)
        {
            return true;
        }

        return false;
    }

    public void vector3ToLeftAndRight(Vector3 vec)
    {
        // Calculate forward and turning components
        float forwardComponent = Vector3.Dot(vec, transform.forward);
        float turningComponent = Vector3.Dot(vec, transform.right) / 1;

        // Calculate left and right wheel forces
        left = forwardComponent - turningComponent;
        right = forwardComponent + turningComponent;
    }

    /// <summary>
    /// the force to be applied to the right wheel each frame
    /// </summary>
    public float right
    {
        get { return _right; }
        set { _right = value; }
    }

    /// <summary>
    /// the force to be applied to the left wheel each frame
    /// </summary>
    public float left
    {
        get { return _left; }
        set { _left = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        //get our rigid body
        rb = GetComponent<Rigidbody>();

        lidarSensors = GetComponentsInChildren<LidarSensor>();

        //create our algorithm objects
        this.leftWallFollower = new LeftWallFollower(lidarSensors, gameObject, this);
    }

    /// <summary>
    /// fired every frame, runs our input checks for what input we should be using to
    /// guide the robot.
    /// </summary>
    private void Update()
    {
        updatePlayerInput();
        updateAStarInput();
        updatePotentialFieldInput();
        updateLeftWallFollowerInput();

        navMeshNavigate();

        drawDebugLines();

    }

    public void navMeshNavigate()
    {
        if (global.aStar)
        {
            global.navMesh = true;
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<NavMeshAgent>().destination = global.goal.transform.position;

        }
    }

    /// <summary>
    /// a simple helper fucntion that draws lines out
    /// from each lidar sensor
    /// </summary>
    private void drawDebugLines()
    {
        if (global.showDebugLines)
        {
            //draw lines for sensors
            foreach (var item in lidarSensors)
            {
                item.drawDebugLine(Color.blue);
            }

            //darw lines for walls
            this.checkForWall(gameObject.transform.forward, 35);
            this.checkForWall(gameObject.transform.right, 35);
            this.checkForWall(-gameObject.transform.forward, 35);
            this.checkForWall(-gameObject.transform.right, 35);
        }
    }

    /// <summary>
    /// check if a star is enabled, if it is apply the aStar algorithm's movement
    /// </summary>
    /// <returns>wether this movement was used</returns>
    private bool updateAStarInput()
    {
        if (global.partialAStar)
        {

        }
        return global.partialAStar;
    }


    /// <summary>
    /// check if left wall follower is enabled, if it is apply the left wall follower algorithm's movement
    /// </summary>
    /// <returns>wether this movement was used</returns>
    private bool updateLeftWallFollowerInput()
    {
        if (global.leftWallFollower)
        {
            (left, right) = leftWallFollower.getDirection();
        }
        return global.leftWallFollower;
    }

    /// <summary>
    /// check if potenetial field is enabled, if it is apply the potenetial field algorithm's movement
    /// </summary>
    /// <returns>wether this movement was used</returns>
    private bool updatePotentialFieldInput()
    {
        if (global.potentialFieldEnabled)
        {
            Vector3 totalForce = GetComponent<PotentialField>().CalculateAttractiveForce() + GetComponent<PotentialField>().CalculateRepulsiveForce();
            Vector3 target_destination = transform.position + totalForce.normalized*2;
            vector3ToLeftAndRight(totalForce);
            global.navMesh = true;
            global.robot.GetComponent<NavMeshAgent>().enabled = true;
            global.robot.GetComponent<NavMeshAgent>().destination = target_destination;
        }
        return global.potentialFieldEnabled;
    }

    /// <summary>
    /// check if player input is enabled, if it is process it
    /// </summary>
    ///<returns>wether this movement was used</returns>
    private bool updatePlayerInput()
    {
        if (enableUserInput)
        {
            float mult = 1;
            if (Input.GetKey(KeyCode.S)) mult = -1;
            if (Input.GetKey("a")) left = 1 * mult; else left = 0;
            if (Input.GetKey("d")) right = 1 * mult; else right = 0;
            if (Input.GetKey("w")) { right = 1; left = 1; }
            if (Input.GetKey("s")) { left = -1; right = -1; }
        }
        return enableUserInput;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (global.navMesh)
        {
            Vector3 target = GetComponent<NavMeshAgent>().steeringTarget;
            Vector3 directionTo = gameObject.transform.position - target;
            vector3ToLeftAndRight(directionTo);
        }

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        float forwardForce = (left + right) / 2f * speed;
        float turningForce = (right - left) * speed /2.0f;

        

        float f1 = forwardForce/2 - turningForce/2;
        float f2 = forwardForce/2 + turningForce/2;

        float r1 = forwardForce - (turningForce * .2f) / 2;
        float r2 = forwardForce + (turningForce * .2f) / 2;


        global.calulate_energy_used_this_tick(f1, r1, f2, r2);

        if(global.navMesh == false)
        {
            // add forward force
            rb.AddForce(transform.forward * forwardForce, ForceMode.VelocityChange);

            // Apply turning force (torque)
            rb.AddTorque(0, turningForce, 0, ForceMode.VelocityChange);
        }



        if (false)
        {
            //this code can be used to simulate the forces the wheels exert on the main body.
            if (left != 0)
            {
                rb.AddForceAtPosition(transform.forward * left * speed, leftWheel.transform.position, ForceMode.Impulse);
            }
            if (right != 0)
            {
                rb.AddForceAtPosition(transform.forward * right * speed, rightWheel.transform.position, ForceMode.Impulse);
            }
        }


        if (rb.velocity.magnitude > speed) rb.velocity = rb.velocity.normalized * speed;


    }
}
