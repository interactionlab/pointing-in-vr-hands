using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptiTrackConnectionTest : MonoBehaviour {

    public GameObject OptiTestObject;
    public GameObject GreenDot;
    public GameObject RedDot;
    bool tracking;

	// Use this for initialization
	void Start () {
        tracking = false;
        RedDot.SetActive(true);
        GreenDot.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if(!tracking)
        {
            if (OptiTestObject.transform.position != Vector3.zero)
            {
                RedDot.SetActive(false);
                GreenDot.SetActive(true);
                tracking = true;
            }
        }
	}
}
