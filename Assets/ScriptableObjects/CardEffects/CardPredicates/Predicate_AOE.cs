using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Predicate_AOE : CardEffectPredicate
{
    public override Card.TargetType TargetType { get { return Card.TargetType.Tiles; } }
    public CardEffectPredicate effect;

    public override void InputGUI()
    {
        if(AffectedArea == null)
            AffectedArea = new AreaOfEffect();

        AffectedArea.Max = 1;
        AffectedArea.Names = new string[] { "Unaffected", "Affected"};

        AffectedArea.OnInputGUI();
    }

    public override void PerformPredicate(TargetInfo targetInfo, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        foreach(TileController targetTile in targetInfo.Tiles)
        {
            List<TileController>[] affectedTiles = worldInfo.worldController.GetAreaAroundTile(targetTile, AffectedArea);

            foreach (TileController tile in affectedTiles[1])
            {
                tile.Ping(20);
            }
        }
    }

    public override string GetDescription(WorldInfo worldInfo)
    {
        return "";
    }
}
