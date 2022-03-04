using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Quality_Tiles_MatchesType : CardEffectQuality
{
    public override Card.TargetType TargetType { get { return Card.TargetType.Tiles; } }

    public Tile.TileType matchingType;

    public override void InputGUI()
    {
        matchingType = (Tile.TileType)EditorGUILayout.EnumFlagsField("Tile Type", matchingType);
    }

    public override bool CheckQuality(TileController tileController, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        return  matchingType.HasFlag(tileController.Type);
    }

    public override string GetDescription(WorldInfo worldInfo, bool isPlural)
    {
        return "";
    }
}
