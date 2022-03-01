using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Condition_Test : CardEffectCondition
{
    public string texty;

    public int val;

    public override void InputGUI()
    {
        texty = EditorGUILayout.TextField("Texty:", texty);
        val = EditorGUILayout.IntField("Val: ", val);
    }

    public override bool CheckCondition(WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        return (Random.Range(1, val + 1) == 1);
    }

    public override string GetDescription()
    {
        return "";
    }
}
