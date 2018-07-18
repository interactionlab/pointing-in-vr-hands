using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositioningToggle : MonoBehaviour
{

    [SerializeField] Renderer Marker;
    bool toggled = true;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Marker.enabled = !Marker.enabled;
        }
    }
}
