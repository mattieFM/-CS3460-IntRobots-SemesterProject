using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.AI;

public class cheat : MonoBehaviour
{
    bool should_cheat = false;
    float time = 0;
    bool cheating = false;
    float on = 9;
    float max = 11;
    // Start is called before the first frame update
    void Start()
    {
        time = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!should_cheat) return;
        if(time > on && time < max*.9)
        {
            if (!global.aStar)
            {
                print("cheating");
                cheating = true;
                global.navMesh = true;
                global.aStar = true;
            }            
        } else if (time > max * .9 && cheating)
        {
            print("stop cheat");
            cheating = false;
            global.aStar = false;
            global.navMesh = false;
            global.robot.GetComponent<NavMeshAgent>().ResetPath();
        }

        time = Time.time % max;


    }
}
