using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreparationScript : MonoBehaviour {

    public GameObject canvas;
    private PositionScript positionScript;

    private bool targetVisibility;
    private bool blocked;
    private float clickTime;

	// Use this for initialization
	void Start () {
        if (canvas != null)
        {
            positionScript = (PositionScript)canvas.GetComponent(typeof(PositionScript));
        }
        else
        {
            Debug.LogError("canvas not found");
        }
        targetVisibility = false;
        blocked = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.I))
        {
            SceneManager.LoadScene("HandsOnRecordingScene");
        }
        if(Input.GetKeyDown(KeyCode.PageUp) || Input.GetKeyDown(KeyCode.PageDown))
        {
            if(!targetVisibility)
            {
                //simply get a random position
                Vector2 tempPos = new Vector2(Random.Range(positionScript.minX + 0.2f, positionScript.maxX - 0.2f), Random.Range(positionScript.minY + 0.2f, positionScript.maxY - 0.2f));
                positionScript.SetTargetPos(tempPos);
                positionScript.setTargetVisibility(true);
                targetVisibility = true;
            }
            else
            {
                if(!blocked)
                {
                    blocked = true;
                    clickTime = Time.time;
                }
            }
        }
        if (blocked)
        {
            if (Time.time - clickTime >= 1.0f)
            {
                positionScript.setTargetVisibility(false);
                blocked = false;
                targetVisibility = false;
            }
        }
	}
}
