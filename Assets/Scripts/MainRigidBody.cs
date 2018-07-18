using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class MainRigidBody : MonoBehaviour {

    public bool SaveData;

    //per user settings
    public Int32 UserId;
    public float ForearmLength;
    public float ElbowScope;
    public float ForearmMarkerDist;
    public float WristRadian;
    public float HandLength;
    public float HandDiameter;
    public float FingerLength;
    public float FingerScope;
    public float UpperArmScope;
    public float UpperArmLength;
    public float UpperArmMarkerDist;
    public float HipHeight;

    private float ElbowDiameter;
    private float WristDiameter;
    private float UpperArmDiameter;
    private float UpperArmAlpha;
    private float ElbowAlpha;

    public OptitrackStreamingClient StreamingClient;
    public Int32 HeadId;
    public Int32 ShoulderId1;
    public Int32 ShoulderId2;
    public Int32 UpperArmId;
    public Int32 ForearmId;
    public Int32 HandId;
    public Int32 IndexFingerId;
    public Int32 HmdId;
    public Int32 SceneId;
    public GameObject HmdCamera;
    public GameObject BeamerCamera;
    public GameObject onText;
    public GameObject offText;
    public GameObject tlxText;

    public GameObject HeadDebug;

    [SerializeField] private List<Hand> hands = new List<Hand>();
    [SerializeField] private List<HandsConfig> allConfigs = new List<HandsConfig>();
    public HandsConfig TestingConfig;
    public Hand currentHand;
    private int handCounter = 0;

    public GameObject ForearmBone;
    public GameObject HandBone;
    public GameObject IndexFingerBone;
    public GameObject IndexFingerTip;

    public GameObject TshirtForeArm;
    public GameObject TshirtUpperArm;
    public GameObject TshirtRoot;
    public GameObject AbdomenRoot;

    public GameObject canvas;
    public GameObject ZeroScene;
    private PositionScript canvasPosition;

    private System.IO.StreamWriter rawDataWriter;
    private System.IO.StreamWriter boneDataWriter;
    private System.IO.StreamWriter clickDataWriter;

    private bool clickVisible;
    private bool clickBlocked;
    private bool tlx;
    private float clickCountTime;
    //0: HMD, 1: beamer
    private int condition;

    private Vector3 coordinateDisplacement;

	private String fileLocation = "OutputData/";
	
    void Start()
    {
        //loading values from gui scene
        
        this.UserId = MainGui.UserId;
        this.UpperArmScope = MainGui.UpperArmScope;
        this.UpperArmLength = MainGui.UpperArmLength;
        this.UpperArmMarkerDist = MainGui.UpperArmMarkerDist;
        this.ForearmLength = MainGui.ForearmLength;
        this.ElbowScope = MainGui.EllbowScope;
        this.ForearmMarkerDist = MainGui.ForearmMarkerDist;
        this.WristRadian = MainGui.WristScope;
        this.HandLength = MainGui.HandLength;
        this.HandDiameter = MainGui.HandDiameter;
        this.FingerLength = MainGui.FingerLength;
        this.FingerScope = MainGui.FingerScope;
        this.HipHeight = MainGui.HipHeight;
        


        //Fecthing hands from config where their order is already set
        //Will retrieve automatically according to userId unless a config has been specified
        if (TestingConfig == null)
        {
            TestingConfig = allConfigs.FirstOrDefault((t) => t.ConfigId == (UserId % allConfigs.Count));
            if (TestingConfig == null)
            {
                Debug.LogError("Could not find testing configuration with matching id");
            }
        }

        foreach (var hand in TestingConfig.hands)
        {
            hands.Add(Instantiate(hand, transform, false).GetComponent<Hand>());
        }

        //Deactivate all hands prior to sim
        foreach (var hand in hands)
        {
            hand.gameObject.SetActive(false);
        }

        SwitchHand(handCounter);
		
		if (!Directory.Exists(fileLocation)) {
            Directory.CreateDirectory(fileLocation);
        }

        //get local position
        coordinateDisplacement = this.transform.position;

        tlxText.GetComponent<MeshRenderer>().enabled = false;

        //scale finger to real size (unity finger length: 7.85cm)
        IndexFingerBone.transform.localScale = new Vector3(FingerLength / 7.85f, FingerLength / 7.85f, FingerLength / 7.85f);
        if (!MainGui.resuming)
        {
            if (UserId % 2 == 0)
            {
                //condition = 0;
                //HmdCamera.GetComponent<Camera>().enabled = true;
                //BeamerCamera.GetComponent<Camera>().enabled = false;
                //onText.GetComponent<MeshRenderer>().enabled = true;
                //offText.GetComponent<MeshRenderer>().enabled = false;
            }
            else
            {
                //condition = 1;
                //HmdCamera.GetComponent<Camera>().enabled = false;
                //BeamerCamera.GetComponent<Camera>().enabled = true;
                //onText.GetComponent<MeshRenderer>().enabled = true;
                //offText.GetComponent<MeshRenderer>().enabled = false;
            }
        }
        else
        {
            //TODO set correct repetition count
            Debug.Log("clickCounter: " + MainGui.clickCounter);
            if (MainGui.clickCounter < canvasPosition.Repetitions * 35)
            {
                //if (UserId % 2 == 0)
                //{
                //    condition = 0;
                //    onText.GetComponent<MeshRenderer>().enabled = true;
                //    offText.GetComponent<MeshRenderer>().enabled = false;
                //}
                //else
                //{
                //    condition = 1;
                    //onText.GetComponent<MeshRenderer>().enabled = false;
                    //offText.GetComponent<MeshRenderer>().enabled = true;
                //}
            }
        }
        //Debug.Log("userCond: " + condition);
        // If the user didn't explicitly associate a client, find a suitable default.
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
        ElbowDiameter = ElbowScope / Convert.ToSingle(Math.PI);
        WristDiameter = WristRadian / Convert.ToSingle(Math.PI);
        UpperArmDiameter = UpperArmScope / Convert.ToSingle(Math.PI);

        //forearm angle
        float a = ((ElbowDiameter / 100.0f / 2.0f) - (WristDiameter / 100.0f / 2.0f));
        float b = ForearmLength / 100.0f;
        float c = Convert.ToSingle(Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2)));
        //calculate alpha with law of sines, convert to degrees
        ElbowAlpha = Convert.ToSingle(Math.Acos((Math.Pow(c, 2) + Math.Pow(a, 2) - Math.Pow(b, 2)) / (2.0f * c * a)));
        ElbowAlpha *= Mathf.Rad2Deg;

        //upper arm/shoulder angle
        float upperArmA = ((UpperArmDiameter / 100.0f / 2.0f) - (ElbowDiameter / 100.0f / 2.0f));
        float upperArmB = UpperArmLength / 100.0f;
        float upperArmC = Convert.ToSingle(Math.Sqrt(Math.Pow(upperArmA, 2) + Math.Pow(upperArmB, 2)));

        UpperArmAlpha = Convert.ToSingle(Math.Acos((Math.Pow(upperArmC, 2) + Math.Pow(upperArmA, 2) - Math.Pow(upperArmB, 2)) / (2.0f * upperArmC * upperArmA)));
        UpperArmAlpha *= Mathf.Rad2Deg;

        //Place hip height
        TshirtRoot.transform.position = new Vector3(0.0f, HipHeight / 100.0f, 0.0f);

        if (SaveData)
        {
            //looks for existing files and increases the version counter
            int dataWriterCounter = 0;
            if (!MainGui.resuming)
            {
                while (true)
                {
                    if (File.Exists(fileLocation + UserId + "clickData_" + dataWriterCounter + ".csv"))
                    {
                        dataWriterCounter++;
                    }
                    else
                    {
                        break;
                    }
                }
                while (true)
                {
                    if (File.Exists(fileLocation + UserId + "rawData_" + dataWriterCounter + ".csv"))
                    {
                        dataWriterCounter++;
                    }
                    else
                    {
                        break;
                    }
                }
                while (true)
                {
                    if (File.Exists(fileLocation + UserId + "boneData_" + dataWriterCounter + ".csv"))
                    {
                        dataWriterCounter++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                dataWriterCounter = MainGui.versionCounter;
            }

            //creating save writers
            Debug.Log("Creating click data writer...");
            clickDataWriter = new System.IO.StreamWriter(fileLocation + UserId + "clickData_" + dataWriterCounter + ".csv", true);
            if (!MainGui.resuming)
            {
                clickDataWriter.WriteLine("take start: " + getCurrentTimeMillis());
                clickDataWriter.WriteLine("id; handId; click time; relative target position (x; y; z)");
                //clickDataWriter.WriteLine("HandId mapping : ");
                //foreach (var hand in hands)
                //{
                //    clickDataWriter.WriteLine("\t" + hand.HandId + " " + hand.HandName);
                //}
            }
            Debug.Log("Successfully created click data writer.");

            Debug.Log("Creating raw data writer...");
            rawDataWriter = new System.IO.StreamWriter(fileLocation + UserId + "rawData_" + dataWriterCounter + ".csv", true);
            if (!MainGui.resuming)
            {
                rawDataWriter.WriteLine("id;time;hmdPos;hmdRot;shoulderPos1;shoulderRot1;shoulderPos2;shoulderRot2;upperarmPos;upperarmRot;forearmPos;forearmRot;handPos;handRot;fingerPos;fingerRot");
            }
            Debug.Log("Successfully created raw data writer.");

            Debug.Log("Creating bone data writer...");
            boneDataWriter = new System.IO.StreamWriter(fileLocation + UserId + "boneData_" + dataWriterCounter + ".csv", true);
            if (!MainGui.resuming)
            {
                boneDataWriter.WriteLine("id;time;hmdPos;hmdRot;hmdForward;shoulderPos1;shoulderPos2;forearmPos;forearmRot;forearmForward;handPos;handRot;handForward;fingerPos;fingerRot;fingerForward;fingertipPos;fingertipRot;fingertipForward");
            }
            Debug.Log("Successfully created bone data writer.");

            Debug.Log("Saving coordinate displacement and canvas position...");
            StreamWriter coordDispWriter = new System.IO.StreamWriter(fileLocation + UserId + "userData_" + dataWriterCounter + ".csv", true);
            coordDispWriter.WriteLine(coordinateDisplacement.ToString("F8") + ";" + canvas.transform.position.ToString("F8"));
            coordDispWriter.Close();
            Debug.Log("Successfully saved coordinate displacement and canvas position.");
        }

        //initialising canvas
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
        tlx = false;
    }


    void Update()
    {
        if (SaveData)
        {
            //click input
            if (Input.GetKeyDown(KeyCode.PageUp) || Input.GetKeyDown(KeyCode.PageDown))
            {
                onText.GetComponent<MeshRenderer>().enabled = false;
                offText.GetComponent<MeshRenderer>().enabled = false;
                if (!clickBlocked)
                {
                    tlxText.GetComponent<MeshRenderer>().enabled = false;
                    if (clickVisible)
                    {
                        long clickTime = getCurrentTimeMillis();
                        Debug.Log("pressed: " + clickTime);
                        Vector3 currentPos = canvasPosition.getCurrentTargetPos();
                        clickDataWriter.WriteLine(UserId + ";" + currentHand.HandId + ";" + clickTime + ";" + currentPos.x + ";" + currentPos.y + ";" + currentPos.z);
                        clickBlocked = true;
                        clickCountTime = Time.time;
                    }
                    else
                    {
                        canvasPosition.setTargetVisibility(true);
                        clickVisible = true;
                        canvasPosition.nextPosition();
                    }
                }
            }
            if (clickBlocked)
            {
                if (Time.time - clickCountTime >= 1)
                {
                    clickBlocked = false;
                    canvasPosition.setTargetVisibility(false);
                    clickVisible = false;
                    if (tlx)
                    {
                        tlxText.GetComponent<MeshRenderer>().enabled = true;
                        tlx = false;
                    }
                }
            }
            //end click input
        }
        //Debug.Log("fps1: " + (1 / Time.deltaTime));

        OptitrackRigidBodyState rbState_shoulder1 = StreamingClient.GetLatestRigidBodyState(ShoulderId1);
        OptitrackRigidBodyState rbState_shoulder2 = StreamingClient.GetLatestRigidBodyState(ShoulderId2);
        OptitrackRigidBodyState rbState_forearm = StreamingClient.GetLatestRigidBodyState(ForearmId);
        OptitrackRigidBodyState rbState_hand = StreamingClient.GetLatestRigidBodyState(HandId);
        OptitrackRigidBodyState rbState_indexFinger = StreamingClient.GetLatestRigidBodyState(IndexFingerId);
        OptitrackRigidBodyState rbState_hmd = StreamingClient.GetLatestRigidBodyState(HmdId);
        OptitrackRigidBodyState rbState_scene = StreamingClient.GetLatestRigidBodyState(SceneId);
        OptitrackRigidBodyState rbState_head = StreamingClient.GetLatestRigidBodyState(HeadId);
        OptitrackRigidBodyState rbState_upperArm = StreamingClient.GetLatestRigidBodyState(UpperArmId);

        //set head position and orientation
        if (rbState_head != null)
        {
            Vector3 newPos = rbState_head.Pose.Position;
            Quaternion newRot = rbState_head.Pose.Orientation;

            Vector3 tempUp = newRot * Vector3.up;
            Vector3 tempForward = newRot * Vector3.forward;

            //move head pos
            newPos = newPos + (0.12f * Vector3.Normalize(tempForward)) + (0.11f * Vector3.Normalize(-1.0f * tempUp));

            HeadDebug.transform.position = newPos + coordinateDisplacement;
            HeadDebug.transform.rotation = newRot;
        }


        //set tshirt pos and rot
        if (rbState_shoulder1 != null && rbState_shoulder2 != null)
        {
            Quaternion newRot = Quaternion.Lerp(rbState_shoulder1.Pose.Orientation, rbState_shoulder2.Pose.Orientation, 0.5f);
            Vector3 shoulderUp = newRot * Vector3.up;
            Vector3 tempForward = newRot * Vector3.forward;

            //measured up displacement: 0.426; better: 0.526
            Vector3 newPos = rbState_shoulder1.Pose.Position + (0.5f * (rbState_shoulder2.Pose.Position - rbState_shoulder1.Pose.Position)) - (0.526f * Vector3.Normalize(shoulderUp)) + (0.12f * Vector3.Normalize(tempForward));
            //TshirtRoot.transform.position = newPos + coordinateDisplacement;
            newPos += coordinateDisplacement;
            
            TshirtRoot.transform.position = new Vector3(newPos.x, HipHeight / 100.0f, newPos.z);// newPos + coordinateDisplacement;

            AbdomenRoot.transform.rotation = newRot;
        }


        //set forarm position and orientation
        if (rbState_forearm != null)
        {
            Vector3 newPos;
            Quaternion newRot;
            getArmBoneRoot(rbState_forearm.Pose.Position, rbState_forearm.Pose.Orientation, out newPos, out newRot);

            newPos = newPos + coordinateDisplacement;

            if (rbState_upperArm != null)
            {
                TshirtUpperArm.transform.LookAt(newPos, rbState_upperArm.Pose.Orientation * Vector3.up); //(ForearmBone.transform.position);
                //TshirtUpperArm.transform.Rotate(Vector3.up, -90);
            }

            //disable pos for better results???
            ForearmBone.transform.position = newPos;

            Vector3 forearmUp = newRot * Vector3.up;
            Vector3 forearmRight = newRot * Vector3.right;
            Vector3 forearmForward = newRot * Vector3.forward;
            ForearmBone.transform.rotation = Quaternion.AngleAxis(-50.0f, forearmRight) * newRot;
            //ForearmBone.transform.LookAt(HandBone.transform.position, rbState_forearm.Pose.Orientation * Vector3.up);

            //Debug.DrawRay(newPos, forearmUp, Color.red);
            //TshirtForeArm.transform.rotation = Quaternion.AngleAxis(154.0f, forearmUp) * Quaternion.AngleAxis(180.0f, forearmRight) * newRot;

            //TshirtForeArm.transform.position = newPos - (0.05f * Vector3.Normalize(forearmRight)) + 0.01f * Vector3.Normalize(forearmForward);
        }

        //set hand position and orientation
        if (rbState_hand != null)
        {
            Vector3 newPos = rbState_hand.Pose.Position;
            Quaternion newRot = rbState_hand.Pose.Orientation;
            getHandBoneRoot(rbState_hand.Pose.Position, rbState_hand.Pose.Orientation, out newPos, out newRot);
            newPos = newPos + coordinateDisplacement;

            HandBone.transform.position = newPos;
            //HandBone.transform.LookAt(IndexFingerBone.transform.position, rbState_hand.Pose.Orientation * Vector3.up);
            HandBone.transform.rotation = newRot;

            //Debug.DrawRay(newPos, newRot * Vector3.forward, Color.black);
        }


        //set t-shirt upper arm rotation
        //if (rbState_upperArm != null)
        //{
        //    Vector3 newPos;
        //    Quaternion newRot;
        //    getUpperArmBoneRoot(rbState_upperArm.Pose.Position, rbState_upperArm.Pose.Orientation, out newPos, out newRot);

        //    //Debug.Log("upper arm ok: " + newRot.ToString("F8"));
        //    TshirtUpperArm.transform.rotation = newRot;
        //}


        //set finger position and orientation
        if (rbState_indexFinger != null)
        {
            Vector3 newPos;
            Quaternion newRot;
            Vector3 fingerTipPos;
            getFingerBoneRoot(rbState_indexFinger.Pose.Position, rbState_indexFinger.Pose.Orientation, out newPos, out newRot, out fingerTipPos);

            newPos = newPos + coordinateDisplacement;

            //disable pos for better results???
            IndexFingerBone.transform.position = newPos;
            IndexFingerBone.transform.rotation = newRot;
        }

        //set tshirt left shoulder pos and rot
        if (rbState_shoulder2 != null)
        {
            //TshirtUpperArm.transform.position = rbState_shoulder2.Pose.Position + coordinateDisplacement;
            //TshirtUpperArm.transform.rotation = rbState_shoulder2.Pose.Orientation;
        }
        if (SaveData)
        {
            saveCalculatedData(rbState_shoulder1, rbState_shoulder2, rbState_head);
            saveRawData(rbState_shoulder1, rbState_shoulder2, rbState_hmd, rbState_forearm, rbState_hand, rbState_indexFinger, rbState_head);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            handCounter = (handCounter + 1) % hands.Count;
            SwitchHand(handCounter);
        }
    }

    private void SwitchHand(int i)
    {
        if (currentHand != null)
        {
            currentHand.gameObject.SetActive(false);
        }

        currentHand = hands[i];
        if (currentHand != null)
        {
            ForearmBone = currentHand.ForearmBone;
            HandBone = currentHand.HandBone;
            IndexFingerBone = currentHand.IndexFingerBone;
            IndexFingerTip = currentHand.IndexFingerTip;
            TshirtForeArm = currentHand.TshirtForeArm;
            TshirtRoot = currentHand.TshirtRoot;
            TshirtUpperArm = currentHand.TshirtUpperArm;
            AbdomenRoot = currentHand.AbdomenRoot;
        }

        //scale finger to real size (unity finger length: 7.85cm)
        IndexFingerBone.transform.localScale = new Vector3(FingerLength / 7.85f, FingerLength / 7.85f, FingerLength / 7.85f);

        currentHand.gameObject.SetActive(true);
    }

    private void getArmBoneRoot(Vector3 bodyPos, Quaternion bodyRot, out Vector3 bonePos, out Quaternion boneRot)
    {
        Vector3 up = bodyRot * Vector3.up;
        Vector3 forward = bodyRot * Vector3.forward;
        Vector3 right = bodyRot * Vector3.right;

        // divided every measured value by 100, because centimeters are measured but meters are needed
        //for fomulas, see documentation
        bonePos = bodyPos + (-1.0f * (ForearmMarkerDist / 100.0f) * Vector3.Normalize(forward)) + ((ElbowDiameter / 100.0f / 2.0f) * Vector3.Normalize(Quaternion.AngleAxis(-1.0f * ElbowAlpha, right) * (-1.0f * forward)));
        boneRot = Quaternion.AngleAxis(-1.0f * (90 - ElbowAlpha), right) * bodyRot;
        //rotate hand model to right orientation
        boneRot = Quaternion.AngleAxis(180, forward) * Quaternion.AngleAxis(90, up) * boneRot;
    }

    private void getUpperArmBoneRoot(Vector3 bodyPos, Quaternion bodyRot, out Vector3 bonePos, out Quaternion boneRot)
    {
        Vector3 up = bodyRot * Vector3.up;
        Vector3 forward = bodyRot * Vector3.forward;
        Vector3 right = bodyRot * Vector3.right;

        // divided every measured value by 100, because centimeters are measured but meters are needed
        //for fomulas, see documentation
        bonePos = bodyPos + (-1.0f * (UpperArmMarkerDist / 100.0f) * Vector3.Normalize(forward)) + ((UpperArmScope / 100.0f / 2.0f) * Vector3.Normalize(Quaternion.AngleAxis(-1.0f * UpperArmAlpha, right) * (-1.0f * forward)));
        boneRot = Quaternion.AngleAxis(-1.0f * (90 - UpperArmAlpha), right) * bodyRot;
        //rotate forearm model to right orientation
        boneRot = Quaternion.AngleAxis(-90, up) * boneRot;
    }

    private void getHandBoneRoot(Vector3 bodyPos, Quaternion bodyRot, out Vector3 bonePos, out Quaternion boneRot)
    {
        Vector3 up = bodyRot * Vector3.up;
        Vector3 forward = bodyRot * Vector3.forward;
        Vector3 right = bodyRot * Vector3.right;

        // divided every measured value by 100, because centimeters are measured but meters are needed
        //for fomulas, see documentation
        bonePos = bodyPos - ((HandLength / 100.0f / 2.0f) * Vector3.Normalize(forward)); //+ ((HandDiameter / 100.0f / 2.0f) * Vector3.Normalize(-1.0f * up)) + ((HandLength / 100.0f / 2.0f) * Vector3.Normalize(-1.0f * forward));
       
        //rotate hand model to right orientation
        boneRot = Quaternion.AngleAxis(180, forward) * Quaternion.AngleAxis(90, up) * bodyRot;
       
        //Debug.DrawRay(bonePos + coordinateDisplacement, forward, Color.green);
        //Debug.DrawRay(bonePos, forward, Color.blue);
        //Debug.DrawRay(bonePos, right, Color.red);
    }

    private void getFingerBoneRoot(Vector3 bodyPos, Quaternion bodyRot, out Vector3 bonePos, out Quaternion boneRot, out Vector3 fingerTipPos)
    {
        Vector3 up = bodyRot * Vector3.up;
        Vector3 forward = bodyRot * Vector3.forward;
        Vector3 right = bodyRot * Vector3.right;

        // divided every measured value by 100, because centimeters are measured but meters are needed
        //for fomulas, see documentation
        float tempFingerDiameter = FingerScope / 100.0f / Convert.ToSingle(Math.PI);
        bonePos = bodyPos + ((tempFingerDiameter / 2.0f) * Vector3.Normalize(-1.0f * up)) + ((FingerLength / 100.0f) * Vector3.Normalize(-1.0f * forward));
        fingerTipPos = bodyPos + ((tempFingerDiameter / 2.0f) * Vector3.Normalize(-1.0f * up)); // +((FingerLength / 100.0f / 2.0f) * Vector3.Normalize(forward));
        //rotate hand model to right orientation
        boneRot = Quaternion.AngleAxis(180, forward) * Quaternion.AngleAxis(90, up) * bodyRot;
    }

    private void saveCalculatedData(OptitrackRigidBodyState ShoulderState1, OptitrackRigidBodyState ShoulderState2, OptitrackRigidBodyState HeadState)
    {
        //initializ with zeros, if no data is send or bones are not set
        Vector3 shoulderPos1 = Vector3.zero;
        //Quaternion shoulderRot1 = Quaternion.identity;
        //Vector3 shoulderForward1 = Vector3.forward;

        Vector3 shoulderPos2 = Vector3.zero;

        Vector3 hmdPos = Vector3.zero;
        Quaternion hmdRot = Quaternion.identity;
        Vector3 hmdForward = Vector3.forward;

        Vector3 headPos = Vector3.zero;
        Quaternion headRot = Quaternion.identity;
        Vector3 headForward = Vector3.forward;

        Vector3 forearmPos = Vector3.zero;
        Quaternion forearmRot = Quaternion.identity;
        Vector3 forearmForward = Vector3.forward;

        Vector3 handPos = Vector3.zero;
        Quaternion handRot = Quaternion.identity;
        Vector3 handForward = Vector3.forward;

        Vector3 fingerPos = Vector3.zero;
        Quaternion fingerRot = Quaternion.identity;
        Vector3 fingerForward = Vector3.forward;

        Vector3 fingertipPos = Vector3.zero;
        Quaternion fingertipRot = Quaternion.identity;
        Vector3 fingertipForward = Vector3.forward;

        if(HmdCamera != null)
        {
            hmdPos = HmdCamera.transform.position;
            hmdRot = HmdCamera.transform.rotation;
            hmdForward = HmdCamera.transform.forward;
        }

        //from scene object
        if(HeadDebug != null)
        {
            headPos = HeadDebug.transform.position;
            headRot = HeadDebug.transform.rotation;
            headForward = HeadDebug.transform.forward;
            Debug.DrawRay(headPos, headForward, Color.black);
        }

        //from rigid body
        /*
        if(HeadState != null)
        {
            headPos = HeadState.Pose.Position + coordinateDisplacement;
            headRot = HeadState.Pose.Orientation;
            headForward = headRot * Vector3.forward;
        }
        */

        if(ShoulderState1 != null)
        {
            shoulderPos1 = ShoulderState1.Pose.Position + coordinateDisplacement;
            //shoulderRot1 = ShoulderState1.Pose.Orientation;
            //shoulderForward1 = Quaternion.AngleAxis(-90, shoulderRot1 * Vector3.up) * shoulderRot1 * Vector3.forward;
        }

        if(ShoulderState2 != null)
        {
            shoulderPos2 = ShoulderState2.Pose.Position + coordinateDisplacement;
        }

        if(ForearmBone != null)
        {
            forearmPos = ForearmBone.transform.position;
            forearmRot = ForearmBone.transform.rotation;
            forearmForward = Quaternion.AngleAxis(-90, forearmRot * Vector3.up) * forearmRot * Vector3.forward;
        }

        if(HandBone != null)
        {
            handPos = HandBone.transform.position;
            handRot = HandBone.transform.rotation;
            handForward = Quaternion.AngleAxis(-90, handRot * Vector3.up) * handRot * Vector3.forward;
        }

        if(IndexFingerBone != null)
        {
            fingerPos = IndexFingerBone.transform.position;
            fingerRot = IndexFingerBone.transform.rotation;
            fingerForward = Quaternion.AngleAxis(-90, fingerRot * Vector3.up) * fingerRot * Vector3.forward;
        }
        //Debug.DrawRay(fingerPos, fingerForward, Color.red);
        
        if(IndexFingerTip != null)
        {
            fingertipPos = IndexFingerTip.transform.position;
            fingertipRot = IndexFingerTip.transform.rotation;
            fingertipForward = Quaternion.AngleAxis(-90, IndexFingerTip.transform.up) * IndexFingerTip.transform.forward;
        }
        //Debug.DrawRay(fingertipPos, fingertipForward, Color.cyan

        //id, time and condition, position 0, 1
        String result = UserId.ToString() + ";" + getCurrentTimeMillis().ToString() + ";";
        //hmd/head, position 2, 3, 4
        //if(condition == 0)
        //{
            result += hmdPos.ToString("F8") + ";" + hmdRot.ToString("F8") + ";" + hmdForward.ToString("F8") + ";";
        //}
        //else
        //{
        //    result += headPos.ToString("F8") + ";" + headRot.ToString("F8") + ";" + headForward.ToString("F8") + ";";

        //}
        //shoulder, positions 5, 6
        //result += shoulderPos1.ToString("F8") + ";" + shoulderRot1.ToString("F8") + ";" + shoulderForward1.ToString("F8") + ";";
        result += shoulderPos1.ToString("F8") + ";" + shoulderPos2.ToString("F8") + ";";
        //forearm, position 7, 8, 9
        result += forearmPos.ToString("F8") + ";" + forearmRot.ToString("F8") + ";" + forearmForward.ToString("F8") + ";";
        //hand, position 10, 11, 12
        result += handPos.ToString("F8") + ";" + handRot.ToString("F8") + ";" + handForward.ToString("F8") + ";";
        //finger, position 13, 14, 15
        result += fingerPos.ToString("F8") + ";" + fingerRot.ToString("F8") + ";" + fingerForward.ToString("F8") + ";";
        //fingertip, position 16, 17, 18
        result += fingertipPos.ToString("F8") + ";" + fingertipRot.ToString("F8") + ";" + fingertipForward.ToString("F8");
        boneDataWriter.WriteLine(result);
    }

    private void saveRawData(OptitrackRigidBodyState ShoulderState1, OptitrackRigidBodyState ShoulderState2, OptitrackRigidBodyState HmdState, OptitrackRigidBodyState forearmState, OptitrackRigidBodyState handState, OptitrackRigidBodyState fingerState, OptitrackRigidBodyState headState)
    {
        OptitrackRigidBodyState rbState_upperArm = StreamingClient.GetLatestRigidBodyState(UpperArmId);

        //initializ with zeros, if no data is send
        Vector3 cameraPos = Vector3.zero;
        Quaternion cameraRot = Quaternion.identity;

        Vector3 headPos = Vector3.zero;
        Quaternion headRot = Quaternion.identity;

        Vector3 shoulderPos1 = Vector3.zero;
        Quaternion shoulderRot1 = Quaternion.identity;

        Vector3 shoulderPos2 = Vector3.zero;
        Quaternion shoulderRot2 = Quaternion.identity;

        Vector3 hmdPos = Vector3.zero;
        Quaternion hmdRot = Quaternion.identity;

        Vector3 upperArmPos = Vector3.zero;
        Quaternion upperArmRot = Quaternion.identity;

        Vector3 forearmPos = Vector3.zero;
        Quaternion forearmRot = Quaternion.identity;

        Vector3 handPos = Vector3.zero;
        Quaternion handRot = Quaternion.identity;

        Vector3 fingerPos = Vector3.zero;
        Quaternion fingerRot = Quaternion.identity;

        if(HmdCamera != null)
        {
            cameraPos = HmdCamera.transform.position;
            cameraRot = HmdCamera.transform.rotation;
        }

        if(headState != null)
        {
            headPos = headState.Pose.Position;
            headRot = headState.Pose.Orientation;
        }

        if (ShoulderState1 != null)
        {
            shoulderPos1 = ShoulderState1.Pose.Position;
            shoulderRot1 = ShoulderState1.Pose.Orientation;
        }

        if(ShoulderState2 != null)
        {
            shoulderPos2 = ShoulderState2.Pose.Position;
            shoulderRot2 = ShoulderState2.Pose.Orientation;
        }

        if (HmdState != null)
        {
            hmdPos = HmdState.Pose.Position;
            hmdRot = HmdState.Pose.Orientation;
        }

        if (rbState_upperArm != null)
        {
            upperArmPos = rbState_upperArm.Pose.Position;
            upperArmRot = rbState_upperArm.Pose.Orientation;
        }

        if (forearmState != null)
        {
            forearmPos = forearmState.Pose.Position;
            forearmRot = forearmState.Pose.Orientation;
        }

        if (handState != null)
        {
            handPos = handState.Pose.Position;
            handRot = handState.Pose.Orientation;
        }

        if (fingerState != null)
        {
            fingerPos = fingerState.Pose.Position;
            fingerRot = fingerState.Pose.Orientation;
        }

        //id, time, position 0, 1
        String result = UserId.ToString() + ";" + getCurrentTimeMillis().ToString() + ";";
        //camera, position 2, 3
        result += cameraPos.ToString("F8") + ";" + cameraRot.ToString("F8") + ";";
        //hmd body, position 4, 5
        result += hmdPos.ToString("F8") + ";" + hmdRot.ToString("F8") + ";";
        //head body, position 6, 7
        result += headPos.ToString("F8") + ";" + headRot.ToString("F8") + ";";
        //shoulder 1 body, position 8, 9
        result += shoulderPos1.ToString("F8") + ";" + shoulderRot1.ToString("F8") + ";";
        //shoulder 2 body, position 10, 11
        result += shoulderPos2.ToString("F8") + ";" + shoulderRot2.ToString("F8") + ";";
        //upper arm body, position 12, 13
        result += upperArmPos.ToString("F8") + ";" + upperArmRot.ToString("F8") + ";";
        //forearm body, position 14, 15
        result += forearmPos.ToString("F8") + ";" + forearmRot.ToString("F8") + ";";
        //hand body, position 16, 17
        result += handPos.ToString("F8") + ";" + handRot.ToString("F8") + ";";
        //finger, position 18, 19
        result += fingerPos.ToString("F8") + ";" + fingerRot.ToString("F8");
        rawDataWriter.WriteLine(result);
    }

    private long getCurrentTimeMillis()
    {
        DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return (long)(DateTime.UtcNow - startTime).TotalMilliseconds;
    }

    void OnApplicationQuit()
    {
        if (rawDataWriter != null)
        {
            rawDataWriter.Close();
        }
        if(boneDataWriter != null)
        {
            boneDataWriter.Close();
        }
        if(clickDataWriter != null)
        {
            clickDataWriter.Close();
        }
        Debug.Log("End of application, writer closed.");
    }

    public void switchStudySetup(int i)
    {
        Debug.Log("switched setup: " + UserId);
        if (!MainGui.resuming)
        {
            ++handCounter;
            SwitchHand(handCounter);

            //if (currentHand != null)
            //{
            //    currentHand.gameObject.SetActive(false);
            //}

            //currentHand = hands[i];
            //if (currentHand != null)
            //{
            //    ForearmBone = currentHand.ForearmBone;
            //    HandBone = currentHand.HandBone;
            //    IndexFingerBone = currentHand.IndexFingerBone;
            //    IndexFingerTip = currentHand.IndexFingerTip;
            //}

            //currentHand.gameObject.SetActive(true);
            //    if (UserId % 2 == 0)
            //    {
            //        condition = 1;
            //        //HmdCamera.GetComponent<Camera>().enabled = false;
            //        //BeamerCamera.GetComponent<Camera>().enabled = true;
            //        onText.GetComponent<MeshRenderer>().enabled = false;
            //        offText.GetComponent<MeshRenderer>().enabled = true;
            //    }
            //    else
            //    {
            //        condition = 0;
            //        //HmdCamera.GetComponent<Camera>().enabled = true;
            //        //BeamerCamera.GetComponent<Camera>().enabled = false;
            //        onText.GetComponent<MeshRenderer>().enabled = true;
            //        offText.GetComponent<MeshRenderer>().enabled = false;
            //    }
            //    canvasPosition.setTargetVisibility(false);
            //}
            //else
            //{
            //    if (MainGui.UserId % 2 == 0)
            //    {
            //        condition = 1;
            //        //HmdCamera.GetComponent<Camera>().enabled = false;
            //        //BeamerCamera.GetComponent<Camera>().enabled = true;
            //        onText.GetComponent<MeshRenderer>().enabled = false;
            //        offText.GetComponent<MeshRenderer>().enabled = true;
            //    }
            //    else
            //    {
            //        condition = 0;
            //        //HmdCamera.GetComponent<Camera>().enabled = true;
            //        //BeamerCamera.GetComponent<Camera>().enabled = false;
            //        onText.GetComponent<MeshRenderer>().enabled = true;
            //        offText.GetComponent<MeshRenderer>().enabled = false;
            //    }
        }
        clickVisible = false;
        clickBlocked = false;
    }

    public void activateTlx()
    {
        tlx = true;
    }

}
