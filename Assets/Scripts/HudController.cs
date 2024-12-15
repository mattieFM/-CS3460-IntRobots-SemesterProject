using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    [SerializeField]
    private GameObject mazePrefab;

    [SerializeField]
    private GameObject robotPrefab;

    [SerializeField]
    private GameObject camera;

    [SerializeField]
    private TextMeshProUGUI speed_label;

    [SerializeField]
    private TextMeshProUGUI battery_power_entry;

    [SerializeField]
    private TextMeshProUGUI battery_consumption_this_tick;

    [SerializeField]
    private TextMeshProUGUI battery_use_total;

    [SerializeField]
    private TextMeshProUGUI path_dist_label;

    /// <summary>
    /// the method to be called when the value of the potential field toggle is changed
    /// </summary>
    /// <param name="value"></param>
    public void onPotentialFieldToggleValueChanged(bool value)
    {
        global.potentialFieldEnabled = value;
        global.robot.AddComponent<PotentialField>();
    }

    /// <summary>
    /// the method to be called when the value of the partial a* search toggle is changed
    /// </summary>
    /// <param name="value"></param>
    public void onPartialAStarToggle(bool value)
    {
        global.partialAStar = value;
        //global.robot.AddComponent<PartialAStar>();
    }

    /// <summary>
    /// the method to be called when the value of the full a* search toggle is changed
    /// </summary>
    /// <param name="value"></param>
    public void onFullAStarToggle(bool value)
    {
        global.aStar = value;

        if(value == false)
        {
            global.robot.GetComponent<NavMeshAgent>().ResetPath();
            global.robot.GetComponent<NavMeshAgent>().enabled = false;
        }
    }

    /// <summary>
    /// the method to be called when the value of the leftwall follower search toggle is changed
    /// </summary>
    /// <param name="value"></param>
    public void onLeftWallFollowerToggle(bool value)
    {
        global.leftWallFollower = value;
    }
    float GetObjectWidth(GameObject obj)
    {
        // Get all Renderer components in the object and its children
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.LogWarning("No renderers found!");
            return 0f;
        }

        // Initialize the bounds to the first renderer's bounds
        Bounds combinedBounds = renderers[0].bounds;

        // Expand the bounds to include all renderers' bounds
        foreach (Renderer renderer in renderers)
        {
            combinedBounds.Encapsulate(renderer.bounds);
        }

        // Return the width of the combined bounds
        return combinedBounds.size.x;
    }

    /// <summary>
    /// update every frame to make sure our maze is in the middle of the screen
    /// </summary>
    public void Update()
    {
        //try {
        //float width = GetObjectWidth(global.mazeObj);
        //camera.transform.position = new Vector3(
        //    global.mazeObj.transform.position.x + width / 2,
        //    global.mazeObj.transform.position.y + 60,
        //    global.mazeObj.transform.position.z + width / 2
        //    );
        //} catch { }

        try
        {
            speed_label.text = $"Speed: {global.speed.ToString()}/Tick";
            battery_consumption_this_tick.text =$"Energy This Tick: {global.energy_consumed_this_tick.ToString()} Wh";
            battery_use_total.text = $"Total Energy Used:  {global.energy_consumed.ToString()} Wh";
            battery_power_entry.text = $"Battery Power: {((global.energy_max - global.energy_consumed) / global.energy_max)*100}%";
            path_dist_label.text = $"Path Length: {global.dist}";
        } catch (Exception e) { print(e); }
    }

    /// <summary>
    /// spawn the robot into the current maze
    /// </summary>
    public void spawnRobot()
    {
        if (global.robot) Destroy(global.robot);
        global.robot = Instantiate(robotPrefab, global.spawn, Quaternion.identity);
    }

    /// <summary>
    /// the function to be called when the map is restarted
    /// </summary>
    public void restart()
    {
        global.spawn = Vector3.zero;
        if (mazePrefab != null)
        {
            if(global.mazeObj) Destroy(global.mazeObj);
            global.mazeObj = Instantiate(mazePrefab, Vector3.zero, Quaternion.identity);
            
            
        }
    }
}
