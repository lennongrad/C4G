using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[System.Serializable]
public class CardEffect
{
    public static int InfiniteTarget = 4;
    public int minTargets = 0;
    public int maxTargets = 0;

    public MonoScript conditionScript;
    [SerializeReference] public CardEffectCondition condition;

    public MonoScript qualityScript;
    [SerializeReference] public CardEffectQuality targetQuality;

    public MonoScript predicateScript;
    [SerializeReference] public CardEffectPredicate predicate;

    public void OnInputGUI()
    {
        EditorGUI.indentLevel++;

        EditorGUILayout.BeginHorizontal();
        string targetString;
        if (maxTargets == InfiniteTarget)
        {
            if (minTargets == InfiniteTarget)
                targetString = "all";
            else
                targetString = minTargets + "+";
        }
        else if (maxTargets == 0)
        {
            targetString = "none";
        }
        else
        {
            targetString = minTargets + "-" + maxTargets;
        }
        EditorGUILayout.LabelField("Targets (" + targetString + ")");
        /// theres no version of MinMaxSlider for ints which made me want to tear my hair out....
        float minTargetsFloat = minTargets; float maxTargetsFloat = maxTargets;
        EditorGUILayout.MinMaxSlider(ref minTargetsFloat, ref maxTargetsFloat, 0, InfiniteTarget);
        minTargets = (int)minTargetsFloat; maxTargets = (int)maxTargetsFloat;
        EditorGUILayout.EndHorizontal();

        conditionInfo();
        EditorGUILayout.Space(4);
        targetQualityInfo();
        EditorGUILayout.Space(4);
        predicateInfo();

        if(predicate != null && targetQuality != null && predicate.TargetType != targetQuality.TargetType)
            EditorGUILayout.HelpBox("The target type of the quality does not match the predicate.", MessageType.Error);

        EditorGUI.indentLevel--;
    }

    /// <summary>
    ///  GUI configuration for the card's condition information
    /// </summary>
    void conditionInfo()
    {
        MonoScript lastConditionScript = conditionScript;
        conditionScript = (MonoScript)EditorGUILayout.ObjectField("Condition", conditionScript, typeof(MonoScript), false);
        if (conditionScript == null)
        {
            string[] result = AssetDatabase.FindAssets("Condition_NoCondition");

            if (result.Length == 1)
            {
                string path = AssetDatabase.GUIDToAssetPath(result[0]);
                conditionScript = (MonoScript)AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript));
            }
        }

        if (conditionScript != lastConditionScript)
            condition = (CardEffectCondition)Activator.CreateInstance(conditionScript.GetClass());

        if (condition != null)
            condition.OnInputGUI();
    }

    /// <summary>
    ///  GUI configuration for the card's targetting quality
    /// </summary>
    void targetQualityInfo()
    {

        MonoScript lastQualityScript = qualityScript;
        qualityScript = (MonoScript)EditorGUILayout.ObjectField("Quality", qualityScript, typeof(MonoScript), false);
        if (qualityScript == null)
        {
            string[] result = AssetDatabase.FindAssets("Quality_Tiles_NoQuality");

            if (result.Length == 1)
            {
                string path = AssetDatabase.GUIDToAssetPath(result[0]);
                qualityScript = (MonoScript)AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript));
            }
        }

        if (qualityScript != lastQualityScript)
            targetQuality = (CardEffectQuality)Activator.CreateInstance(qualityScript.GetClass());

        if (targetQuality != null)
            targetQuality.OnInputGUI();
    }

    /// <summary>
    /// GUI configuration for the effect's predicate (effect)
    /// </summary>
    void predicateInfo()
    {

        MonoScript lastPredicateScript = predicateScript;
        predicateScript = (MonoScript)EditorGUILayout.ObjectField("Predicate", predicateScript, typeof(MonoScript), false);
        if (predicateScript == null)
        {
            string[] result = AssetDatabase.FindAssets("Predicate_NoPredicate");

            if (result.Length == 1)
            {
                string path = AssetDatabase.GUIDToAssetPath(result[0]);
                predicateScript = (MonoScript)AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript));
            }
        }

        if (predicateScript != lastPredicateScript)
            predicate = (CardEffectPredicate)Activator.CreateInstance(predicateScript.GetClass());

        if (predicate != null)
            predicate.OnInputGUI();
    }

    /// <summary>
    /// Textual description of the effect
    /// </summary>
    public string GetDescription(WorldInfo worldInfo) {
        // no targets so just ignore targetting, or no predicate to get type from
        if (condition == null || targetQuality == null || predicate == null)
        {
            return "";
        }

        string returnString = condition.GetDescription(worldInfo);

        if(maxTargets > 0)
        {
            // need plural for qualities, ie "enemy" vs "enemies"
            bool isPlural = true;

            if (maxTargets == InfiniteTarget)
            {
                if (minTargets == InfiniteTarget)
                {
                    returnString += "For each ";
                    isPlural = false;
                }
                else if (minTargets > 0)
                {
                    returnString += "For " + minTargets.ToWord() + " or more target ";
                }
                else
                {
                    returnString += "For any number of target ";
                }
            }
            else
            {
                /// if maxTargets is 0, card does not target at all, so dont even display qualities
                if (minTargets > 0)
                {
                    if(minTargets == 1 && maxTargets == 1)
                    {
                        returnString += "For one target ";
                        isPlural = false;
                    }
                    else
                    {
                        returnString += "For each of ";
                        for (int i = minTargets; i <= maxTargets; i++)
                        {
                            returnString += i.ToWord();
                            if (i != maxTargets && (maxTargets - minTargets == 1))
                                returnString += " or ";
                            else if (i != maxTargets)
                                returnString += ", or ";
                        }
                        returnString += " target ";
                    }
                }
                else
                {
                    returnString += "For each of up to " + maxTargets.ToWord() + " target ";
                    if (maxTargets == 1)
                        isPlural = false;
                }
            }

            returnString += targetQuality.GetDescription(worldInfo, isPlural);
        }

        if (returnString == "")
            returnString += predicate.GetDescription(worldInfo).FirstCharToUpper();
        else
            returnString += ", " + predicate.GetDescription(worldInfo);

        return returnString;
    }
}
