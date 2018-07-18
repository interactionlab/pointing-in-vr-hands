using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibMarkerAverage : MonoBehaviour {

    private int SampleCounter;
    private Vector3 average;

	// Use this for initialization
	void Start () {
        SampleCounter = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (SampleCounter < 2000)
        {
            if(SampleCounter % 100 == 0){
                Debug.Log("count: " + SampleCounter);
                Debug.Log("vec: " + average.ToString("F8"));
            }
            average += this.transform.position;
            SampleCounter++;
        }
        else
        {
            Debug.Log("average: " + (average / (float)SampleCounter).ToString("F8"));
        }
	}
}
