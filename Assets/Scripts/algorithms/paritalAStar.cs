using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PartialAStar : MonoBehaviour
{
    public float repulsionStrength = 35f;
    public float attractionStrength = 1.1f;
    public float maxSensorRange = 10f;
    public LidarSensor[] lidarSensors;
    public bool showDebugLines = true;

    private List<Vector3> known_obstacles = new List<Vector3>();

    private float wall_thresh = 2;

    // A* parameters
    public Transform goal;  // Goal target
    public float gridSize = 1f;  // Grid size for pathfinding
    private Vector3 currentPosition;
    private Vector3 targetPosition;
    private List<Vector3> path = new List<Vector3>();
    private float nearby_radius = .3f;

    public void Start()
    {
        lidarSensors = global.robot.GetComponent<robotController>().lidarSensors;  // Array of sensors
        currentPosition = transform.position;
        targetPosition = global.goal.transform.position;
        goal = global.goal.transform;
    }

    private void FixedUpdate()
    {
        // Update the path every frame based on sensors and goal position
        foreach (var sensor in lidarSensors)
        {
            if (sensor.getDistanceToWall() < wall_thresh)
            {
                known_obstacles.Add(transform.position + sensor.getDistanceToWall() * sensor.getDirection());
            }
        }
    }

    void Update()
    {

        path = AStarPathfinding(transform.position, global.goal.transform.position);

        if (showDebugLines)
        {
            ShowDebugLines();
        }
    }

    // A* Pathfinding algorithm (simplified for partial pathfinding)
    public List<Vector3> AStarPathfinding(Vector3 start, Vector3 end)
    {
        List<Vector3> openList = new List<Vector3>();
        List<Vector3> closedList = new List<Vector3>();
        Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();

        // Initialize with the start position
        openList.Add(start);

        Dictionary<Vector3, float> gCost = new Dictionary<Vector3, float>();
        gCost[start] = 0;

        Dictionary<Vector3, float> fCost = new Dictionary<Vector3, float>();
        fCost[start] = Vector3.Distance(start, end);  // Heuristic: distance to goal
        return new List<Vector3>();  // No path found
        while (openList.Count > 0)
        {
            // Get the node with the lowest fCost
            Vector3 currentNode = openList.OrderBy(node => fCost[node]).First();

            if (currentNode == end)
            {
                // Reached the goal, reconstruct the path
                List<Vector3> resultPath = new List<Vector3>();
                while (cameFrom.ContainsKey(currentNode))
                {
                    resultPath.Add(currentNode);
                    currentNode = cameFrom[currentNode];
                }
                resultPath.Reverse();
                return resultPath;
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // Get neighbors (grid points around the current node)
            List<Vector3> neighbors = GetNeighbors(currentNode);

            foreach (Vector3 neighbor in neighbors)
            {
                if (closedList.Contains(neighbor) || IsObstacleNearby(neighbor)) {
                    continue;
                }

                float tentativeGCost = gCost[currentNode] + Vector3.Distance(currentNode, neighbor);
                if (!openList.Contains(neighbor) || tentativeGCost < gCost[neighbor])
                {
                    cameFrom[neighbor] = currentNode;
                    gCost[neighbor] = tentativeGCost;
                    fCost[neighbor] = gCost[neighbor] + Vector3.Distance(neighbor, end);

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        return new List<Vector3>();  // No path found
    }

    // Check if there are obstacles nearby the node using lidar data
    bool IsObstacleNearby(Vector3 position)
    {
        bool is_near = false;
        foreach(Vector3 obstacle in known_obstacles)
        {
            if(Vector3.Distance(position,obstacle) < nearby_radius)
            {
                is_near = true;
            }
        }

        return is_near;
    }

    // Generate neighboring points on the grid around the current node
    List<Vector3> GetNeighbors(Vector3 currentNode)
    {
        List<Vector3> neighbors = new List<Vector3>();

        // Add neighbors (grid-based 4 directions or 8 directions for diagonals)
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                Vector3 neighbor = currentNode + new Vector3(x * gridSize, 0, y * gridSize);
                neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }

    // Move along the calculated path (simple linear movement)
    void MoveAlongPath(List<Vector3> path)
    {
        if (path.Count > 0)
        {
            Vector3 nextTarget = path[0];
            Vector3 direction = (nextTarget - transform.position).normalized;
            transform.position += direction * Time.deltaTime * 2f;  // Adjust movement speed

            if (Vector3.Distance(transform.position, nextTarget) < 0.1f)
            {
                path.RemoveAt(0);  // Move to next waypoint
            }
        }
    }

    void ShowDebugLines()
    {
        // Visualize the path
        for (int i = 0; i < path.Count - 1; i++)
        {
            Debug.DrawLine(path[i], path[i + 1], Color.blue);
        }

      
    }

}
