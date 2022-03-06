using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Predicate_DrawCards : CardEffectPredicate
{
    public override Card.TargetType TargetType { get { return Card.TargetType.None; } }

    public int cardAmount = 1;

    public override void InputGUI()
    {
        cardAmount = EditorGUILayout.IntField("# of cards: ", cardAmount);
    }

    public override void PerformPredicate(TargetInfo targetInfo, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
    }

    public override string GetDescription(WorldInfo worldInfo)
    {
        return "draw " + cardAmount + " cards.";
    }
}
