using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Predicate_Test : CardEffectPredicate
{
    public override Card.TargetType TargetType { get { return Card.TargetType.Tiles; } }

    public int damage = 0;

    public override void InputGUI()
    {
        damage = EditorGUILayout.IntField("Damage: ", damage);
    }

    public override void PerformPredicate(TargetInfo targetInfo, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        Debug.Log(damage);
    }

    public override string GetDescription()
    {
        return "";
    }
}
