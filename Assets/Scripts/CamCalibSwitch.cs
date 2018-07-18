using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamCalibSwitch : MonoBehaviour {

    public bool RoomFB;
    public bool RoomLR;
    public bool CamFB;
    public bool CamUD;
    public bool CamLR;
    public bool CamSwitch;
    public float unit;

    public GameObject hmdCam;
    public GameObject CameraRig;
    public GameObject beamerCam;
    public GameObject canvas;
    private PositionScript canvasPosition;

    // Use this for initialization
    void Start()
    {
        if(CamFB || CamUD || CamLR)
        {
            hmdCam.GetComponent<Camera>().enabled = false;
            beamerCam.GetComponent<Camera>().enabled = true;
        }
        else
        {
            hmdCam.GetComponent<Camera>().enabled = true;
            beamerCam.GetComponent<Camera>().enabled = false;
        }
        if (canvas != null)
        {
            canvasPosition = (PositionScript)canvas.GetComponent(typeof(PositionScript));
        }
        else
        {
            Debug.LogError("canvas not found");
        }
        //different points for callibration, y += 0.658
        //canvasPosition.SetTargetPos(new Vector2(1.0f, 0.9f));
        //canvasPosition.SetTargetPos(new Vector2(1.5f, 0.9f));
        //canvasPosition.SetTargetPos(new Vector2(2.0f, 0.9f));
        //canvasPosition.SetTargetPos(new Vector2(1.0f, 1.4f));
        //canvasPosition.SetTargetPos(new Vector2(1.5f, 1.4f));
        //canvasPosition.SetTargetPos(new Vector2(2.0f, 1.4f));
        canvasPosition.SetTargetPos(new Vector2(1.8f, 0.6f));
    }

    // Update is called once per frame
    void Update()
    {
        canvasPosition.setTargetVisibility(true);
        if(RoomFB)
        {
            if(Input.GetKeyDown(KeyCode.PageUp))
            {
                CameraRig.transform.position = new Vector3(CameraRig.transform.position.x, CameraRig.transform.position.y, CameraRig.transform.position.z + unit);
            }
            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                CameraRig.transform.position = new Vector3(CameraRig.transform.position.x, CameraRig.transform.position.y, CameraRig.transform.position.z - unit);
            }
        }
        if (RoomLR)
        {
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                CameraRig.transform.position = new Vector3(CameraRig.transform.position.x + unit, CameraRig.transform.position.y, CameraRig.transform.position.z);
            }
            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                CameraRig.transform.position = new Vector3(CameraRig.transform.position.x - unit, CameraRig.transform.position.y, CameraRig.transform.position.z);
            }
        }
        if(CamFB)
        {
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                beamerCam.transform.position = new Vector3(beamerCam.transform.position.x, beamerCam.transform.position.y, beamerCam.transform.position.z + unit);
            }
            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                beamerCam.transform.position = new Vector3(beamerCam.transform.position.x, beamerCam.transform.position.y, beamerCam.transform.position.z - unit);
            }
        }
        if (CamLR)
        {
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                beamerCam.transform.position = new Vector3(beamerCam.transform.position.x + unit, beamerCam.transform.position.y, beamerCam.transform.position.z);
            }
            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                beamerCam.transform.position = new Vector3(beamerCam.transform.position.x - unit, beamerCam.transform.position.y, beamerCam.transform.position.z);
            }
        }
        if (CamUD)
        {
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                beamerCam.transform.position = new Vector3(beamerCam.transform.position.x, beamerCam.transform.position.y - unit, beamerCam.transform.position.z);
            }
            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                beamerCam.transform.position = new Vector3(beamerCam.transform.position.x, beamerCam.transform.position.y + unit, beamerCam.transform.position.z);
            }
        }
        if(CamSwitch)
        {
            if (Input.GetKeyDown(KeyCode.PageUp) || Input.GetKeyDown(KeyCode.PageDown))
            {
                hmdCam.GetComponent<Camera>().enabled = !hmdCam.GetComponent<Camera>().enabled;
                beamerCam.GetComponent<Camera>().enabled = !beamerCam.GetComponent<Camera>().enabled;
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            hmdCam.GetComponent<Camera>().enabled = !hmdCam.GetComponent<Camera>().enabled;
            beamerCam.GetComponent<Camera>().enabled = !beamerCam.GetComponent<Camera>().enabled;
        }
    }
}
