using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePosition : MonoBehaviour {

    public GameObject fingerTip;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
    void Update()
    {
        if (fingerTip != null)
        {
            RaycastHit hit; 
            if (Physics.Raycast(fingerTip.transform.position, Quaternion.AngleAxis(-90, fingerTip.transform.up) * fingerTip.transform.forward, out hit))
            {
                this.transform.position = hit.point;
            }
        }
    }
}
