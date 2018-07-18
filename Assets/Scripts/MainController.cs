using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class MainController : MonoBehaviour {

    public Int32 Id;

    public GameObject canvas;

    private PositionScript canvasPosition;

    private System.IO.StreamWriter clickDataWriter;

    private bool clickVisible;
    private bool clickBlocked;
    private float clickCountTime;

    // Use this for initialization
    void Start()
    {
        Debug.Log("Creating click data writer...");
        //looks for existing files and increases the version counter
        int dataWriterCounter = 0;
        while (true)
        {
            if (File.Exists("Assets/OutputData/ClickData/" + Id + "clickData_" + dataWriterCounter + ".csv"))
            {
                dataWriterCounter++;
            }
            else
            {
                break;
            }
        }
        clickDataWriter = new System.IO.StreamWriter("Assets/OutputData/ClickData/" + Id + "clickData_" + dataWriterCounter + ".csv", true);
        Debug.Log("Successfully created click data writer.");
        clickDataWriter.WriteLine("take start: " + getCurrentTimeMillis());
        clickDataWriter.WriteLine("id; click time; relative target position (x; y; z)");

        if (canvas != null)
        {
            canvasPosition = (PositionScript)canvas.GetComponent(typeof(PositionScript));
        }
        else
        {
            Debug.LogError("canvas not found");
        }

        canvasPosition.setTargetVisibility(false);
        clickVisible = false;
        clickBlocked = false;

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.PageUp) || Input.GetKeyDown(KeyCode.PageDown))
        {
            if(!clickBlocked)
            {
                if (clickVisible)
                {
                    long clickTime = getCurrentTimeMillis();
                    Debug.Log("pressed: " + clickTime);
                    Vector3 currentPos = canvasPosition.getCurrentTargetPos();
                    clickDataWriter.WriteLine(Id + "; " + clickTime + "; " + currentPos.x + "; " + currentPos.y + "; " + currentPos.z);
                    clickBlocked = true;
                    clickCountTime = Time.time;
                }
                else
                {
                    canvasPosition.nextPosition();
                    canvasPosition.setTargetVisibility(true);
                    clickVisible = true;
                }
            }
        }
        if(clickBlocked)
        {
            if (Time.time - clickCountTime >= 1)
            {
                clickBlocked = false;
                canvasPosition.setTargetVisibility(false);
                clickVisible = false;
            }
        }
    }

    void OnApplicationQuit()
    {
        if(clickDataWriter != null)
        {
            clickDataWriter.Close();
        }
        Debug.Log("End of application, writer closed.");
    }

    private string getFrame()
    {
        string result = "";

        //add id
        result += Id + ";";

        //add timestamp (from startpoint of the software)
        result += Time.time + ";";

        return result;
    }

    private long getCurrentTimeMillis()
    {
        DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return (long)(DateTime.UtcNow - startTime).TotalMilliseconds;
    }

}
