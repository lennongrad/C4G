using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Predicate_EnemyDamage : CardEffectPredicate
{
    public override Card.TargetType TargetType { get { return Card.TargetType.Enemies; } }

    public float damage = 1f;

    public override void InputGUI()
    {
        damage = EditorGUILayout.FloatField("Damage: ", damage);
    }

    public override void PerformPredicate(TargetInfo targetInfo, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        foreach (EnemyController enemy in targetInfo.Enemies)
            enemy.DirectDamage(damage);
    }

    public override string GetDescription(WorldInfo worldInfo)
    {
        return "";
    }
}
