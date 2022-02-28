using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CardEffectConditionTest : CardEffectCondition
{
    public string texty;

    public int val;

    public override void InputGUI()
    {
        EditorGUI.indentLevel += 1;
        texty = EditorGUILayout.TextField("Texty:", texty);
        val = EditorGUILayout.IntField("Val: ", val);
        EditorGUI.indentLevel -= 1;
    }

    public override bool CheckCondition()
    {
        return (Random.Range(1, val + 1) == 1);
    }
}
