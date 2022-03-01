using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Quality_ : CardEffectQuality
{
    public override Card.TargetType TargetType { get { return Card.TargetType.Tiles; } }

    public override void InputGUI()
    {
    }

    public override bool CheckQuality(TileController tileController, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        return true;
    }

    public override string GetDescription(bool isPlural)
    {
        return "";
    }
}