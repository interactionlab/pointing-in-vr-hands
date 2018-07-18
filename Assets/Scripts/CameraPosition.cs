using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour {

    public GameObject TrackingSpace;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        TrackingSpace.transform.position = -1.0f * this.transform.position + new Vector3(0.0f, 1.5f, 0.0f);
        Debug.Log(TrackingSpace.transform.position);
	}
}
