using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Class that card effect conditions inherit from.
/// A condition is a script that is run when trying to resolve a 
/// card effect to determine if that effect should happen.
/// </summary>
[System.Serializable]
public abstract class CardEffectCondition
{
    /// <summary>
    /// Used to easily duplicate a base class script for a new condition
    /// </summary>
    [MenuItem("Utilities/Card Effects/Make Empty Condition")]
    public static void MakeEmptyCondition()
    {
        string[] result = AssetDatabase.FindAssets("Condition_", new string[] { "Assets/ScriptableObjects/CardEffects/Samples" });
        if (result.Length == 1)
        {
            string path = AssetDatabase.GUIDToAssetPath(result[0]);
            string newPath = "Assets/ScriptableObjects/CardEffects/CardConditions/Condition_.cs";

            if (!AssetDatabase.CopyAsset(path, newPath))
                Debug.LogWarning($"Failed to copy {path}");
        }
    }

    /// <summary>
    /// Called to set up GUI for class-specific customization
    /// </summary>
    public abstract void InputGUI();
    /// <summary>
    /// Method called during runtime to actually check if the condition
    /// is true (and whether to run the effect preedicate)
    /// </summary>
    public abstract bool CheckCondition(WorldInfo worldInfo, ResolutionInfo resolutionInfo);

    public abstract string GetDescription(WorldInfo worldInfo);

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
