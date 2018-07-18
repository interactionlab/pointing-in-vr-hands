using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "HandsConfig", menuName = "Configs", order = 1)]
public class HandsConfig : ScriptableObject
{
    public int ConfigId;
    public List<GameObject> hands = new List<GameObject>();
}
