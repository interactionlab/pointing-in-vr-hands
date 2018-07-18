using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
/**
    this class is able to load a recordet data file and playback the laoded data
    */
public class PlayerScript : MonoBehaviour {

    public class TrackingDataPoint
    {
        public int id;
        public int condition;
        public long time;
        public Vector3 hmdPosition;
        public Quaternion hmdRotation;
        public Vector3 shoulder1Position;
        public Vector3 shoulder2Position;
        public Quaternion shoulder1Rotation;
        public Quaternion shoulder2Rotation;
        public Vector3 forearmPosition;
        public Quaternion forearmRotation;
        public Vector3 handPosition;
        public Quaternion handRotation;
        public Vector3 indexPosition;
        public Quaternion indexRotation;
    }

    public class Click
    {
        public long time;
        public float x;
        public float y;
    }

    public GameObject canvas;

    //recorded data objects

    public Int32 Id;
    public Int32 versionId;
    //0: outside project; 1: test data; 2: inside project
    public int fileType;
    public string date;

    public GameObject hmd;
    public GameObject TshirtRoot;
    public GameObject TshirtForearmBone;
    public GameObject TshirtUpperArm;
    public GameObject forearmBone;
    public GameObject handBone;
    public GameObject indexFingerBone;
    public GameObject TestShoulder;

    private PositionScript canvasPosition;
    private List<TrackingDataPoint> trackingData;
    private List<Click> clickData;
    private int currentDataPoint = 0;
    private int currentClickIndex = 0;
    private string FilePath;

	// Use this for initialization
	void Start () {
        Application.targetFrameRate = 90;

        if(fileType == 0)
        {
            FilePath = "C:\\Users\\schweigern\\Desktop\\Builds\\OutputData\\";
        }
        else if(fileType == 1)
        {
            FilePath = "C:\\Users\\schweigern\\Desktop\\PintingInVR\\pointinginvr\\TestData\\" + date + "\\";
        }
        else if(fileType == 2)
        {
            FilePath = "Assets/OutputData/";
        }

        canvasPosition = (PositionScript)canvas.GetComponent(typeof(PositionScript));
        canvasPosition.setTargetVisibility(true);
        trackingData = new List<TrackingDataPoint>();
        clickData = new List<Click>();
        FileStream dataFile;
        FileStream clickFile;
        FileStream rawFile;
        if (fileType == 1)
        {
            dataFile = File.OpenRead(FilePath + Id + "boneData_" + versionId + ".csv");
            clickFile = File.OpenRead(FilePath + Id + "clickData_" + versionId + ".csv");
            rawFile = File.OpenRead(FilePath + Id + "rawData_" + versionId + ".csv");
        }
        else
        {
            dataFile = File.OpenRead(FilePath + "BoneData\\" +  Id + "boneData_" + versionId + ".csv");
            clickFile = File.OpenRead(FilePath + "ClickData\\" + Id + "clickData_" + versionId + ".csv");
            rawFile = File.OpenRead(FilePath + "RawData\\" + Id + "rawData_" + versionId + ".csv");
        }

        var dataReader = new StreamReader(dataFile);
        var clickReader = new StreamReader(clickFile);
        var rawReader = new StreamReader(rawFile);
        bool firstLine = true;
        while (!dataReader.EndOfStream && !rawReader.EndOfStream)
        {
            if (firstLine)
            {
                dataReader.ReadLine();
                rawReader.ReadLine();
                firstLine = false;
            }
            else
            {
                var readLine = dataReader.ReadLine();
                var splitValues = readLine.Split(';');

                var rawReadLine = rawReader.ReadLine();
                var rawSplitValues = rawReadLine.Split(';');

                var tempData = new TrackingDataPoint();
                tempData.id = Int32.Parse(splitValues[0]);
                tempData.time = long.Parse(splitValues[1]);

                //hmd, position 2 and 3
                String[] tempHmdPosition = splitValues[2].Replace("(", "").Replace(")", "").Split(',');
                String[] tempHmdRotation = splitValues[3].Replace("(", "").Replace(")", "").Split(',');

                tempData.hmdPosition = new Vector3(float.Parse(tempHmdPosition[0]), float.Parse(tempHmdPosition[1]), float.Parse(tempHmdPosition[2]));
                tempData.hmdRotation = new Quaternion(float.Parse(tempHmdRotation[0]), float.Parse(tempHmdRotation[1]), float.Parse(tempHmdRotation[2]), float.Parse(tempHmdRotation[3]));

                //shoulder, position 5 and 6
                String[] tempShoulder1Position = splitValues[5].Replace("(", "").Replace(")", "").Split(',');
                String[] tempShoulder2Position = splitValues[6].Replace("(", "").Replace(")", "").Split(',');

                tempData.shoulder1Position = new Vector3(float.Parse(tempShoulder1Position[0]), float.Parse(tempShoulder1Position[1]), float.Parse(tempShoulder1Position[2]));
                tempData.shoulder2Position = new Vector3(float.Parse(tempShoulder2Position[0]), float.Parse(tempShoulder2Position[1]), float.Parse(tempShoulder2Position[2]));

                //shoulder rot from raw, position 5 and 7
                String[] tempShoulder1Rotation = rawSplitValues[9].Replace("(", "").Replace(")", "").Split(',');
                String[] tempShoulder2Rotation = rawSplitValues[11].Replace("(", "").Replace(")", "").Split(',');

                tempData.shoulder1Rotation = new Quaternion(float.Parse(tempShoulder1Rotation[0]), float.Parse(tempShoulder1Rotation[1]), float.Parse(tempShoulder1Rotation[2]), float.Parse(tempShoulder1Rotation[3]));
                tempData.shoulder2Rotation = new Quaternion(float.Parse(tempShoulder2Rotation[0]), float.Parse(tempShoulder2Rotation[1]), float.Parse(tempShoulder2Rotation[2]), float.Parse(tempShoulder2Rotation[3]));

                //forearm, position 7 and 8
                String[] tempForearmPosition = splitValues[7].Replace("(", "").Replace(")", "").Split(',');
                String[] tempForearmRotation = splitValues[8].Replace("(", "").Replace(")", "").Split(',');

                tempData.forearmPosition = new Vector3(float.Parse(tempForearmPosition[0]), float.Parse(tempForearmPosition[1]), float.Parse(tempForearmPosition[2]));
                tempData.forearmRotation = new Quaternion(float.Parse(tempForearmRotation[0]), float.Parse(tempForearmRotation[1]), float.Parse(tempForearmRotation[2]), float.Parse(tempForearmRotation[3]));

                //hand, position 10 and 11
                String[] tempHandPosition = splitValues[10].Replace("(", "").Replace(")", "").Split(',');
                String[] tempHandRotation = splitValues[11].Replace("(", "").Replace(")", "").Split(',');

                tempData.handPosition = new Vector3(float.Parse(tempHandPosition[0]), float.Parse(tempHandPosition[1]), float.Parse(tempHandPosition[2]));
                tempData.handRotation = new Quaternion(float.Parse(tempHandRotation[0]), float.Parse(tempHandRotation[1]), float.Parse(tempHandRotation[2]), float.Parse(tempHandRotation[3]));

                //index finger, position 13 and 14
                String[] tempIndexPosition = splitValues[13].Replace("(", "").Replace(")", "").Split(',');
                String[] tempIndexRotation = splitValues[14].Replace("(", "").Replace(")", "").Split(',');

                tempData.indexPosition = new Vector3(float.Parse(tempIndexPosition[0]), float.Parse(tempIndexPosition[1]), float.Parse(tempIndexPosition[2]));
                tempData.indexRotation = new Quaternion(float.Parse(tempIndexRotation[0]), float.Parse(tempIndexRotation[1]), float.Parse(tempIndexRotation[2]), float.Parse(tempIndexRotation[3]));
                /*
                Debug.Log("");
                Debug.Log("pos: " + tempHmdPosition);
                Debug.Log("rot: " + tempHmdRotation);
                Debug.Log("");
                */

                trackingData.Add(tempData);
            }
        }
        Debug.Log("number of loaded data points: " + trackingData.Count);

        int labelCounter = 0;
        while(!clickReader.EndOfStream)
        {
            if (labelCounter < 2)
            {
                labelCounter++;
                clickReader.ReadLine();
            }
            else
            {
                Click tempClick = new Click();
                String[] tempData = clickReader.ReadLine().Split(';');
                tempClick.time = long.Parse(tempData[2]);
                tempClick.x = float.Parse(tempData[3]);
                tempClick.y = float.Parse(tempData[4]);
                clickData.Add(tempClick);
            }
        }
        Debug.Log("loaded " + clickData.Count + " click points");
    }
	
	// Update is called once per frame
	void Update () {
        if (currentDataPoint < trackingData.Count)
        {
            //Debug.Log("fps: " + (1 / Time.deltaTime));
            //Debug.Log("curr: " + currentDataPoint);
            var tempData = trackingData[currentDataPoint];

            if(TestShoulder != null)
            {
                TestShoulder.transform.position = tempData.shoulder2Position;
            }

            if(hmd != null)
            {
                hmd.transform.position = tempData.hmdPosition;
                hmd.transform.rotation = tempData.hmdRotation;
            }

            if(TshirtRoot != null)
            {
                Quaternion newRot = Quaternion.Lerp(tempData.shoulder1Rotation, tempData.shoulder2Rotation, 0.5f);
                Vector3 shoulderUp = newRot * Vector3.up;
                Vector3 tempForward = newRot * Vector3.forward;


                Vector3 newPos = tempData.shoulder1Position + (0.5f * (tempData.shoulder2Position - tempData.shoulder1Position)) - (0.426f * Vector3.Normalize(shoulderUp)) + (0.12f * Vector3.Normalize(tempForward));

                TshirtRoot.transform.position = newPos;
                TshirtRoot.transform.rotation = newRot;
            }

           if(TshirtForearmBone != null)
           {
               Vector3 forearmUp = tempData.forearmRotation * Vector3.up;
               Vector3 forearmRight = tempData.forearmRotation * Vector3.right;
               Vector3 forearmForward = tempData.forearmRotation * Vector3.forward;
               TshirtForearmBone.transform.rotation = Quaternion.AngleAxis(154.0f, forearmUp) * Quaternion.AngleAxis(180.0f, forearmRight) * tempData.forearmRotation;

               TshirtForearmBone.transform.position = tempData.forearmPosition - (0.05f * Vector3.Normalize(forearmRight)) + 0.01f * Vector3.Normalize(forearmForward);
           }

            if (forearmBone != null)
            {
                forearmBone.transform.position = tempData.forearmPosition;
                forearmBone.transform.rotation = tempData.forearmRotation;
            }

            if (handBone != null)
            {
                handBone.transform.position = tempData.handPosition;
                handBone.transform.rotation = tempData.handRotation;
            }

            if (indexFingerBone != null)
            {
                indexFingerBone.transform.position = tempData.indexPosition;
                indexFingerBone.transform.rotation = tempData.indexRotation;
            }
            currentDataPoint++;
            if(currentClickIndex < clickData.Count && clickData[currentClickIndex].time <= tempData.time)
            {
                canvasPosition.SetTargetPos(new Vector2(clickData[currentClickIndex].x, clickData[currentClickIndex].y));
                //Debug.Log("clicked");
                currentClickIndex++;
            }
        }
	}
}
