using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSurveilanceCams : MonoBehaviour {

    private Camera[] cams;
    private int camCounter = 0;

	// Use this for initialization
	void Start () {
        cams = this.GetComponentsInChildren<Camera>();
        Debug.Log("cam count: " + cams.Length);
        for (int i = 1; i < cams.Length; i++)
        {
            cams[i].enabled = false;
        }
        camCounter = (camCounter + 1) % cams.Length;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.L))
        {
            for (int i = 1; i < cams.Length; i++)
            {
                cams[i].enabled = false;
            }
            cams[camCounter].enabled = true;
            camCounter = (camCounter + 1) % cams.Length;
        }
	}
}
