using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResumeGui : MonoBehaviour {

    public InputField FilePathInput;
    public InputField UserIdInput;
    public InputField VersionNumberInput;
    public GameObject NewParticipantGuiCanvas;
    public GameObject ResumeGuiCanvas;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ResumeButtonClicked()
    {
        string FilePath;
        Int32 tempId;
        int tempVersionNumber;

        bool allRight = true;
        
        FilePath = FilePathInput.text;
        FilePath = "C:\\Users\\schweigern\\Desktop\\Builds\\OutputData\\";
        
        if (!Int32.TryParse(UserIdInput.text, out tempId))
        {
            allRight = false;
            Debug.LogError("Could not parse user id");
        }

        if (!int.TryParse(VersionNumberInput.text, out tempVersionNumber))
        {
            allRight = false;
            Debug.LogError("Could not parse version number");
        }
        if(allRight)
        {
            bool allFiles = true;
            if(!File.Exists(FilePath + "UserData\\" + tempId + "userData_" + tempVersionNumber + ".csv"))
            {
                allFiles = false;
                Debug.LogError("Could not find user data file.");
            }
            if (!File.Exists(FilePath + "BoneData\\" + tempId + "boneData_" + tempVersionNumber + ".csv"))
            {
                allFiles = false;
                Debug.LogError("Could not find bone data file.");
            }
            if (!File.Exists(FilePath + "ClickData\\" + tempId + "clickData_" + tempVersionNumber + ".csv"))
            {
                allFiles = false;
                Debug.LogError("Could not find click data file.");
            }
            if (!File.Exists(FilePath + "RawData\\" + tempId + "rawData_" + tempVersionNumber + ".csv"))
            {
                allFiles = false;
                Debug.LogError("Could not find raw data file.");
            }

            if (allFiles)
            {
                MainGui.resuming = true;

                var userDataFile = File.OpenRead(FilePath + "userData/" + tempId + "userData_" + tempVersionNumber + ".csv");
                var userDataReader = new StreamReader(userDataFile);
                userDataReader.ReadLine();
                string rawUserData = userDataReader.ReadLine();
                String[] tempUserData = rawUserData.Split(';');

                //loading saved values
                MainGui.UserId = int.Parse(tempUserData[0]);
                MainGui.FilePath = tempUserData[1];
                MainGui.ShoulderHeight = float.Parse(tempUserData[2]);
                MainGui.UpperArmScope = float.Parse(tempUserData[3]);
                MainGui.UpperArmLength = float.Parse(tempUserData[4]);
                MainGui.EllbowScope = float.Parse(tempUserData[5]);
                MainGui.ForearmLength = float.Parse(tempUserData[6]);
                MainGui.WristScope = float.Parse(tempUserData[7]);
                MainGui.HandLength = float.Parse(tempUserData[8]);
                MainGui.HandDiameter = float.Parse(tempUserData[9]);
                MainGui.FingerScope = float.Parse(tempUserData[10]);
                MainGui.FingerLength = float.Parse(tempUserData[11]);
                MainGui.UpperArmMarkerDist = float.Parse(tempUserData[12]);
                MainGui.ForearmMarkerDist = float.Parse(tempUserData[13]);
                MainGui.versionCounter = tempVersionNumber;

                //counting current clicks
                var clickDataFile = File.OpenRead(FilePath + "clickData/" + tempId + "clickData_" + tempVersionNumber + ".csv");
                var clickDataReader = new StreamReader(clickDataFile);
                int lineCounter = -2; //-2 cause of the two head lines in the save file
                while(!clickDataReader.EndOfStream)
                {
                    clickDataReader.ReadLine();
                    lineCounter++;
                }
                MainGui.clickCounter = lineCounter;

                //resuming study
                SceneManager.LoadScene("RecordingScene");
            }
            else
            {
                Debug.LogError("Could not find some files, please try again.");
            }
        }
    }

    public void CancelButtonClicked()
    {
        ResumeGuiCanvas.SetActive(false);
        NewParticipantGuiCanvas.SetActive(true);
    }

}
