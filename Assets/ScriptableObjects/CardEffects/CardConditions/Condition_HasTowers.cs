using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Condition_HasTowers : CardEffectCondition
{
    int numberOfTowers;

    public override void InputGUI()
    {
        numberOfTowers = (int)EditorGUILayout.IntField("Number of towers: ", numberOfTowers);
    }

    public override bool CheckCondition(WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        return true;
    }

    public override string GetDescription(WorldInfo worldInfo)
    {
        return "";
    }
}
