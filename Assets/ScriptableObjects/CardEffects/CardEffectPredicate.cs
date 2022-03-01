using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Class that card effect predicates inherit from.
/// A predicate is the actual effect of the card effect, such
/// as dealing damage to targets.
/// </summary>
[System.Serializable]
public abstract class CardEffectPredicate
{
    /// <summary>
    /// Used to easily duplicate a base class script for a new condition
    /// </summary>
    [MenuItem("Utilities/Card Effects/Make Empty Predicate")]
    public static void MakeEmptyPredicate()
    {
        string[] result = AssetDatabase.FindAssets("Predicate_ l:Sample");
        if (result.Length == 1)
        {
            string path = AssetDatabase.GUIDToAssetPath(result[0]);
            string newPath = "Assets/ScriptableObjects/CardEffects/CardPredicates/Predicate_.cs";

            if (!AssetDatabase.CopyAsset(path, newPath))
                Debug.LogWarning($"Failed to copy {path}");
        }
    }

    public abstract Card.TargetType TargetType { get; }

    /// <summary>
    /// Called to set up GUI for class-specific customization
    /// </summary>
    public abstract void InputGUI();
    public abstract void PerformPredicate(TargetInfo targetInfo, WorldInfo worldInfo, ResolutionInfo resolutionInfo);
    public abstract string GetDescription();

    /// <summary>
    /// Method called in CardGenerator window to display the information the
    /// script needs for customization
    /// </summary>
    public void OnInputGUI()
    {
        EditorGUI.indentLevel += 1;
        InputGUI();
        EditorGUI.indentLevel -= 1;
    }
}
