using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Tiles_TowerPlacementAllowed : CardEffectQuality
{
    public override Card.TargetType TargetType { get { return Card.TargetType.Tiles; } }

    public override void InputGUI()
    {
    }

    public override bool CheckQuality(TileController tileController, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        return tileController.PresentTower == null && (tileController.Type == Tile.TileType.Floor || tileController.Type == Tile.TileType.Raised);
    }

    public override string GetDescription(WorldInfo worldInfo, bool isPlural)
    {
        return "tile";
    }
}
