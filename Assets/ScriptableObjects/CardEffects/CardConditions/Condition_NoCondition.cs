using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Condition_NoCondition : CardEffectCondition
{

    public override void InputGUI()
    {
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
