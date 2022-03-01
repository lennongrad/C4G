using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 
/// </summary>
[System.Serializable]
public abstract class CardEffectQuality
{
    /// <summary>
    /// Used to easily duplicate a base class script for a new condition
    /// </summary>
    [MenuItem("Utilities/Card Effects/Make Empty Quality")]
    public static void MakeEmptyQuality()
    {
        string[] result = AssetDatabase.FindAssets("Quality_ l:Sample");
        if (result.Length == 1)
        {
            string path = AssetDatabase.GUIDToAssetPath(result[0]);
            string newPath = "Assets/ScriptableObjects/CardEffects/CardQualities/Quality_.cs";

            if (!AssetDatabase.CopyAsset(path, newPath))
                Debug.LogWarning($"Failed to copy {path}");
        }
    }

    public abstract Card.TargetType TargetType { get; }
    /// <summary>
    /// Method called in CardGenerator window to display the information the
    /// script needs for customization
    /// </summary>
    public abstract void InputGUI();

    public virtual bool CheckQuality(TileController tile, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        return false;
    }
    public virtual bool CheckQuality(TowerController tower, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        return false;
    }
    public virtual bool CheckQuality(EnemyController enemy, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        return false;
    }
    public virtual bool CheckQuality(CardModel card, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        return false;
    }

    public abstract string GetDescription(bool isPlural);

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
