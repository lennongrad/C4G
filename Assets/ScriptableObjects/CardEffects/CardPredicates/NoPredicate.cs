using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class NoPredicate : CardEffectPredicate
{
    public override Card.TargetType TargetType { get { return Card.TargetType.None; } }

    public override void InputGUI()
    { 
    }

    public override void PerformPredicate(TargetInfo targetInfo, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
    }

    public override string GetDescription(WorldInfo worldInfo)
    {
        return "";
    }
}
