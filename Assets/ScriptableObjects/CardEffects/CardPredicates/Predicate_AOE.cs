using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[System.Serializable]
public class Predicate_AOE : CardEffectPredicate
{
    public override Card.TargetType TargetType { get { return Card.TargetType.Tiles; } }

    float damage = 3f;

    public override void InputGUI()
    {
        damage = EditorGUILayout.FloatField("Damage", damage);
        AffectedArea = EditorGUILayout.ObjectField("AOE:", AffectedArea, typeof(AreaOfEffect), true) as AreaOfEffect;
    }

    public override void PerformPredicate(TargetInfo targetInfo, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        targetInfo.AOETargetting(AffectedArea, worldInfo, targetInfo.Direction);

        foreach(EnemyController enemy in targetInfo.AOEEnemies[1])
            enemy.DirectDamage(damage);
    }

    public override string GetDescription(WorldInfo worldInfo)
    {
        return "deal " + damage + " damage to each enemy adjacent to that tile.";
    }
}
