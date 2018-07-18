using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class fixPos : MonoBehaviour {

    public GameObject cam;

    public OptitrackStreamingClient StreamingClient;
    public Int32 RigidBodyId;

	// Use this for initialization
	void Start () {
        UnityEngine.XR.InputTracking.disablePositionalTracking = true;

        if (this.StreamingClient == null)
        {
            this.StreamingClient = OptitrackStreamingClient.FindDefaultClient();

            // If we still couldn't find one, disable this component.
            if (this.StreamingClient == null)
            {
                Debug.LogError(GetType().FullName + ": Streaming client not set, and no " + typeof(OptitrackStreamingClient).FullName + " components found in scene; disabling this component.", this);
                this.enabled = false;
                return;
            }
        }
        
	}
	
	// Update is called once per frame
	void Update () {
       

        //transform.rotation = Quaternion.Inverse(InputTracking.GetLocalRotation(VRNode.CenterEye));
        //transform.position = -InputTracking.GetLocalPosition(VRNode.CenterEye);   

        //transform.position = ;
        //transform.rotation = Quaternion.Inverse(cam.transform.localRotation);

        //this.transform.rotation = Quaternion.Inverse(cam.transform.localRotation);
        //this.transform.position = -cam.transform.localPosition;
        //this.transform.rotation = 
        //Debug.Log(  Quaternion.ToEulerAngles(InputTracking.GetLocalRotation(VRNode.Head)));// = false.transform.localRotation).ToString("F2"));
        //Debug.Log(  Quaternion.ToEulerAngles(InputTracking.GetLocalRotation(VRNode.CenterEye)));

        /*
        var x = -Quaternion.ToEulerAngles(InputTracking.GetLocalRotation(VRNode.Head));
        this.transform.localRotation = Quaternion.Euler(x * 180.0f / (float)Math.PI);
        Debug.Log((x * 180.0f / (float)Math.PI).ToString("F2"));
        */
        
        OptitrackRigidBodyState rbState = StreamingClient.GetLatestRigidBodyState(RigidBodyId);
        if (rbState != null)
        {
            this.transform.localPosition = rbState.Pose.Position;
            //this.transform.localRotation = rbState.Pose.Orientation * Quaternion.Inverse(cam.transform.localRotation);
        }
        else
        {
            //this.transform.localRotation = Quaternion.Inverse(cam.transform.localRotation);
            //Debug.Log("nope");
        }
    }
}
