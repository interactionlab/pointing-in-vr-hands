using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distance : MonoBehaviour {

    public GameObject hand;
    public GameObject arm;

	// Use this for initialization
	void Start () {
        Debug.Log("dist hand, arm: " + (arm.transform.position - hand.transform.position).ToString("F8"));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
