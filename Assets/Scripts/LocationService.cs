﻿using System.Collections;
using UnityEngine;

public class LocationService : MonoBehaviour
{
    private bool initialized = false;

    IEnumerator Start()
    {
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
			Output("GPS isn't enabled");
            yield break;
        }

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            print("GPS initialization timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
			Output("Unable to determine device location");
            yield break;
        }

        initialized = true;
    }

	void Update()
	{
        if (initialized)
        {
            SendMessage("GpsUpdated", new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude));
        }
	}

	private void Output(string str){
        SendMessage("GpsError", str);
	}
}