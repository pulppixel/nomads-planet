using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTests : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {

        int i = int.MaxValue - 1;
        Debug.Log("Time " + i + " gets: " + Time(i++));
        Debug.Log("Time " + i + " gets: " + Time(i++));
        Debug.Log("Time " + i + " gets: " + Time(i++));
        Debug.Log("Time " + i + " gets: " + Time(i++));
        Debug.Log("Time " + i + " gets: " + Time(i++));

        i = -1;

        Debug.Log("Time " + i + " gets: " + Time(i++));
        Debug.Log("Time " + i + " gets: " + Time(i++));
        Debug.Log("Time " + i + " gets: " + Time(i++));
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public static double Time(int ServerTimestamp)
        {

            uint u = (uint)ServerTimestamp;
            double t = u;
            double frametime =  t / 1000.0d;
            return frametime;
        }
 

}
