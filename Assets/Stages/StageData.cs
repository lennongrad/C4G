using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/StageData", order = 1), System.Serializable]
public class StageData : ScriptableObject
{
    public string stageTitle;
    public StageLayout[] layout;
}

