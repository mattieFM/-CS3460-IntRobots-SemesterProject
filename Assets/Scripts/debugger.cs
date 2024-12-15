using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class debugger : MonoBehaviour
{
    public NavMeshAgent agent;

    void Update()
    {
        // Ensure the agent has a valid path
        if (agent.hasPath)
        {
            // Draw the path in the Scene view using Debug.DrawLine
            for (int i = 0; i < agent.path.corners.Length - 1; i++)
            {
                Vector3 start = agent.path.corners[i];
                Vector3 end = agent.path.corners[i + 1];

                // Draw a line between each corner in the path
                Debug.DrawLine(start, end, Color.green);
            }
        }
    }

    // Optional: Also draw path in Scene view with Gizmos
    private void OnDrawGizmos()
    {
        if (agent != null && agent.hasPath)
        {
            for (int i = 0; i < agent.path.corners.Length - 1; i++)
            {
                Vector3 start = agent.path.corners[i];
                Vector3 end = agent.path.corners[i + 1];

                // Draw a path with Gizmos in the Scene view
                Gizmos.color = Color.green;
                Gizmos.DrawLine(start, end);
            }
        }
    }
}
