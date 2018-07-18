using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamActivation : MonoBehaviour {

    public int camNumber;

	// Use this for initialization
	void Start () {
        if (Display.displays.Length > camNumber)
        {
            if (!Display.displays[camNumber].active)
            {
                Display.displays[camNumber].Activate();
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
