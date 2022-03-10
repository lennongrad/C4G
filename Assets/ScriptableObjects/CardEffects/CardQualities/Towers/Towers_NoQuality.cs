using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Towers_NoQuality : CardEffectQuality
{
    public override Card.TargetType TargetType { get { return Card.TargetType.Towers; } }

    public override void InputGUI()
    {
    }

    public override bool CheckQuality(TileController tileController, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        return true;
    }

    public override string GetDescription(WorldInfo worldInfo, bool isPlural)
    {
        return "";
    }
}
