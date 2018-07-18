using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PositionScript : MonoBehaviour {
    
    public int Repetitions = 1;

    public Vector2[] positions;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    public GameObject MainRigidBody;
    private MainRigidBody mainRigidBodyScript;
    public GameObject xDisplay;
    public GameObject yDisplay;
    public GameObject center;

    private float tempX;
    private float tempY;

    private int currentPosition;
    private int[] StudyOrder;
    //private bool secondStudy = false;
    private int currentStudyIndex = 0;

    private int UserId;

    // Use this for initialization
    void Start()
    {
        //TODO has do be from gui level, only for testing
        //UserId = 42;
        UserId = MainGui.UserId;

        mainRigidBodyScript = (MainRigidBody)MainRigidBody.GetComponent(typeof(MainRigidBody));
        resetStudy();

        positions = new Vector2[35];
        for(int i = 0; i < positions.Length; i++)
        {
            positions[i] = new Vector2((float)((i % 7) + 1) * 0.443125f, ((float)Math.Floor((double)i / 7.0d) + 1.0f) * 0.3195f);
        }

        //for resuming
        if(MainGui.resuming)
        {
            currentPosition = MainGui.clickCounter;
            
            if(currentPosition >= StudyOrder.Length)
            {
                currentStudyIndex = currentPosition / StudyOrder.Length;
                currentPosition %= StudyOrder.Length;
                mainRigidBodyScript.switchStudySetup(currentStudyIndex);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void SetTargetPos(Vector2 position)
    {
        float x = position.x;
        float y = position.y;
        if (x > maxX)
        {
            x = maxX;
        }
        if (x < minX)
        {
            x = minX;
        }
        if (y > maxY)
        {
            y = maxY;
        }
        if (y < minY)
        {
            y = minY;
        }
        if (xDisplay != null)
        {
            xDisplay.transform.localPosition = new Vector3(x, xDisplay.transform.localPosition.y, xDisplay.transform.localPosition.z);
        }
        if(yDisplay != null)
        {
            yDisplay.transform.localPosition = new Vector3(yDisplay.transform.localPosition.x, y, yDisplay.transform.localPosition.z);
        }
        if(center != null)
        {
            center.transform.localPosition = new Vector3(x, y, center.transform.localPosition.z);
        }
    }

    public Vector3 getCurrentTargetPos()
    {
        return center.transform.localPosition;
    }

    public void nextPosition()
    {
        Debug.Log("cur: " + currentPosition + "; + len: " + StudyOrder.Length);
        //TLX sheets
        if (/*currentPosition + 1 == (int)(StudyOrder.Length * 0.5f) ||*/ currentPosition + 1 == (int)(StudyOrder.Length))
        {
            mainRigidBodyScript.activateTlx();
        }
        if(currentPosition < StudyOrder.Length)
        {
            Debug.Log("setting current pos: " + currentPosition);
            SetTargetPos(positions[StudyOrder[currentPosition]]);
            currentPosition++;
        }
        else
        {
            if (currentStudyIndex < mainRigidBodyScript.TestingConfig.hands.Count - 1)
            {
                //secondStudy = true;
                resetStudy();
                mainRigidBodyScript.switchStudySetup(++currentStudyIndex);
            }
            else
            {
                Debug.Log("end reached");
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
    }

    public void resetStudy()
    {
        setTargetVisibility(false);
        currentPosition = 0;
        StudyOrder = getShuffeledOrder();
    }

    private int[] getShuffeledOrder()
    {
        //using the Fisher-Yates-Shuffle
        System.Random random = new System.Random(UserId);
        int[] tempOrder = new int[Repetitions * positions.Length];
        for (int i = 0; i < tempOrder.Length; i++)
        {
            tempOrder[i] = i % positions.Length;
        }
        for(int j = 0; j < tempOrder.Length - 2; j++)
        {
            int tempPos = random.Next(j, tempOrder.Length);
            int swapValue = tempOrder[j];
            tempOrder[j] = tempOrder[tempPos];
            tempOrder[tempPos] = swapValue;
        }
        return tempOrder;
    }

    public void setTargetVisibility(bool NewVisibility)
    {
        xDisplay.GetComponent<MeshRenderer>().enabled = NewVisibility;
        yDisplay.GetComponent<MeshRenderer>().enabled = NewVisibility;
        center.GetComponent<MeshRenderer>().enabled = NewVisibility;
    }

}
