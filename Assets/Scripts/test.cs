using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class test : MonoBehaviour
{
    // List to store the world space positions of the clicks
    private List<Vector3> clickPositions = new List<Vector3>();

    // Optionally, for visualization purposes (to show path using line renderer)
    public LineRenderer lineRenderer;

    private int currentPointIndex = 0;

    void Update()
    {
        // Detect mouse clicks and convert screen space to world space
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            // Raycast to get the world position of the click
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Capture the world space position where the raycast hit
                Vector3 clickPosition = hit.point;
                clickPositions.Add(clickPosition);
                print(hit.point);

                // Optionally, visualize the path as a line in the editor
                UpdatePathVisualization();
            }
        }

        try
        {
            NavMeshAgent agent = global.robot.GetComponent<NavMeshAgent>();

            if (clickPositions.Count == 0 || agent.pathPending)
                return;

            // Check if the agent has reached the current destination
            if (agent.remainingDistance <= agent.stoppingDistance && global.partialAStar)
            {
                global.navMesh = true;
                currentPointIndex++;

                // If there are more points to follow
                if (currentPointIndex < clickPositions.Count)
                {
                    // Set the next point as the new destination
                    agent.SetDestination(clickPositions[currentPointIndex]);
                }
                else
                {
                    // Path is complete
                    Debug.Log("Path completed!");
                }
            }
        }
        catch { }
    }

    // This function can be used to get the path as a List of Vector3s
    public List<Vector3> GetPath()
    {
        return new List<Vector3>(clickPositions);
    }

    // This function can be used to visualize the path with a LineRenderer
    private void UpdatePathVisualization()
    {
        if (lineRenderer != null)
        {
            // Set the number of points in the line renderer
            lineRenderer.positionCount = clickPositions.Count;
            // Set the positions in the line renderer
            for (int i = 0; i < clickPositions.Count; i++)
            {
                lineRenderer.SetPosition(i, clickPositions[i]);
            }
        }
    }

    // Optional: Reset the path when needed (e.g., on button press or event)
    public void ResetPath()
    {
        clickPositions.Clear();
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }
    }

    // Draw the path using Gizmos
    void OnDrawGizmos()
    {
        if (clickPositions.Count > 1)
        {
            // Set the Gizmo color for path visualization
            Gizmos.color = Color.black;

            // Loop through the points and draw a line between them
            for (int i = 0; i < clickPositions.Count - 1; i++)
            {
                Gizmos.DrawLine(clickPositions[i], clickPositions[i + 1]);
            }

            // Optionally, draw spheres at each click position
            foreach (Vector3 position in clickPositions)
            {
                Gizmos.DrawSphere(position, 0.1f); // Size of the sphere
            }
        }
    }
}
