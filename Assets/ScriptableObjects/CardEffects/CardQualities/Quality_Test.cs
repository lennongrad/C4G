using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Quality_Test : CardEffectQuality
{
    public override Card.TargetType TargetType { get { return Card.TargetType.Tiles; } }

    public Tile.TileType type = Tile.TileType.Floor;

    public override void InputGUI()
    {
        type = (Tile.TileType)EditorGUILayout.EnumPopup("Tile Type", type);
    }

    public override bool CheckQuality(TileController tileController, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        return tileController.Type == type;
    }

    public override string GetDescription(bool isPlural)
    {
        return isPlural ? "tiles." : "tile.";
    }
}
