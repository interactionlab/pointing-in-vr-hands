using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class MainGui : MonoBehaviour {

    public InputField UserIdInput;
    public InputField FilePathInput;
    public InputField ShoulderHeightInput;
    public InputField UpperArmScopeInput;
    public InputField UpperArmLengthInput;
    public InputField EllbowScopeInput;
    public InputField ForearmLengthInput;
    public InputField WristScopeInput;
    public InputField HandLengthInput;
    public InputField HandDiameterInput;
    public InputField FingerScopeInput;
    public InputField FingerLengthInput;
    public InputField UpperArmMarkerDistInput;
    public InputField ForearmMarkerDistInput;
    public InputField HipHeightInput;

    public GameObject NewParticipantCanvas;
    public GameObject ResumeGuiCanvas;

    public static Int32 UserId;
    public static string FilePath;
    public static float ShoulderHeight;
    public static float UpperArmScope;
    public static float UpperArmLength;
    public static float EllbowScope;
    public static float ForearmLength;
    public static float WristScope;
    public static float HandLength;
    public static float HandDiameter;
    public static float FingerScope;
    public static float FingerLength;
    public static float UpperArmMarkerDist;
    public static float ForearmMarkerDist;
    public static float HipHeight;

    public static bool resuming;
    public static int clickCounter;

    //for same version number over all files
    public static int versionCounter;

	private String fileLocation = "OutputData/";
	
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ResumeButtonClicked()
    {
        NewParticipantCanvas.SetActive(false);
        ResumeGuiCanvas.SetActive(true);
    }

    public void StartButtonClicked()
    {
        resuming = false;
        bool allRight = true;

        if (!Int32.TryParse(UserIdInput.text, out UserId))
        {
            allRight = false;
            Debug.LogError("Could not parse user id");
        }

        FilePath = FilePathInput.text;

        if (!float.TryParse(ShoulderHeightInput.text, out ShoulderHeight))
        {
            allRight = false;
            Debug.LogError("Could not parse shoulder height");
        }

        if (!float.TryParse(UpperArmScopeInput.text, out UpperArmScope))
        {
            allRight = false;
            Debug.LogError("Could not parse upper arm scope");
        }
        
        if (!float.TryParse(UpperArmLengthInput.text, out UpperArmLength))
        {
            allRight = false;
            Debug.LogError("Could not parse upper arm length");
        }

        if (!float.TryParse(EllbowScopeInput.text, out EllbowScope))
        {
            allRight = false;
            Debug.LogError("Could not parse ellbow scope");
        }

        if (!float.TryParse(ForearmLengthInput.text, out ForearmLength))
        {
            allRight = false;
            Debug.LogError("Could not parse forearm length");
        }

        if (!float.TryParse(WristScopeInput.text, out WristScope))
        {
            allRight = false;
            Debug.LogError("Could not parse wrist scope");
        }

        if (!float.TryParse(HandLengthInput.text, out HandLength))
        {
            allRight = false;
            Debug.LogError("Could not parse hand length");
        }

        if (!float.TryParse(HandDiameterInput.text, out HandDiameter))
        {
            allRight = false;
            Debug.LogError("Could not parse hand diameter");
        }

        if (!float.TryParse(FingerScopeInput.text, out FingerScope))
        {
            allRight = false;
            Debug.LogError("Could not parse finger scope");
        }

        if (!float.TryParse(FingerLengthInput.text, out FingerLength))
        {
            allRight = false;
            Debug.LogError("Could not parse finger length");
        }
        
        if (!float.TryParse(UpperArmMarkerDistInput.text, out UpperArmMarkerDist))
        {
            allRight = false;
            Debug.LogError("Could not parse upper arm marker distance");
        }
        
        if (!float.TryParse(ForearmMarkerDistInput.text, out ForearmMarkerDist))
        {
            allRight = false;
            Debug.LogError("Could not parse forearm marker distance");
        }

        if (!float.TryParse(HipHeightInput.text, out HipHeight))
        {
            allRight = false;
            Debug.LogError("Could not parse hip height");
        }

		if (!Directory.Exists(fileLocation)) {
            Directory.CreateDirectory(fileLocation);
        }
		
        int tempVersion = 0;
        while (true)
        {
            if (File.Exists(fileLocation + UserId + "userData_" + tempVersion + ".csv"))
            {
                tempVersion++;
            }
            else
            {
                break;
            }
        }

        if (allRight)
        {
            Debug.Log("Creating user data writer...");
            StreamWriter userDataWriter = new System.IO.StreamWriter(fileLocation + UserId + "userData_" + tempVersion + ".csv", true);
            userDataWriter.WriteLine("UserId;FilePath;ShoulderHeight;UpperArmScope;UpperArmLength;EllbowScope;ForearmLength;WristScope;HandLength;HandDiameter;FingerScope;FingerLength;UpperArmMarkerDist;ForearmMarkerDist;HipHeight");
            Debug.Log("Successfully created user data writer.");

            string result = "";
            result += UserId + ";";
            result += FilePath + ";";
            result += ShoulderHeight + ";";
            result += UpperArmScope + ";";
            result += UpperArmLength + ";";
            result += EllbowScope + ";";
            result += ForearmLength + ";";
            result += WristScope + ";";
            result += HandLength + ";";
            result += HandDiameter + ";";
            result += FingerScope + ";";
            result += FingerLength + ";";
            result += UpperArmMarkerDist + ";";
            result += ForearmMarkerDist + ";";
            result += HipHeight;

            userDataWriter.WriteLine(result);
            userDataWriter.Close();
            SceneManager.LoadScene("HandsOnPreparationScene");
        }
        else
        {
            Debug.LogError("Some values are bad, please correct them.");
        }
    }

}
