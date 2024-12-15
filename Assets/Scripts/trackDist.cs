using UnityEngine;

public class trackDist : MonoBehaviour
{
    private Vector3 lastPosition; // Store the last position
    private float totalDistance = 0f; // Store the total distance traveled

    void Start()
    {
        // Initialize the last position to the current position at the start
        lastPosition = transform.position;
    }

    void Update()
    {
        // Calculate the distance moved this frame
        float distanceThisFrame = Vector3.Distance(lastPosition, transform.position);

        // Add the distance moved this frame to the total distance
        totalDistance += distanceThisFrame;

        // Update the last position to the current position
        lastPosition = transform.position;

        // Optionally, you can print the total distance to the console for debugging
        global.dist = totalDistance;
        //Debug.Log("Total distance traveled: " + totalDistance);
    }

    // Optional: A public method to get the total distance traveled
    public float GetTotalDistance()
    {
        return totalDistance;
    }
}
