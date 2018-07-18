using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitchTest : MonoBehaviour {

    public Camera hmdCam;
    public Camera beamerCam;

	// Use this for initialization
	void Start () {
        hmdCam.enabled = true;
        beamerCam.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.PageUp) || Input.GetKeyDown(KeyCode.PageDown))
        {
            hmdCam.enabled = !hmdCam.enabled;
            beamerCam.enabled = !beamerCam.enabled;
        }
	}
}
