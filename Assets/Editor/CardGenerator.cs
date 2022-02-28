using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

/// <summary>
/// A Unity editor window to make card data
/// </summary>
public class CardGenerator : EditorWindow
{
    /// <summary>
    /// Path which cards are automatically saved at
    /// </summary>
    private string cardPath = "ScriptableObjects/Cards/";

    /// <summary>
    /// Card name used in filename of the card
    /// </summary>
    private string cardName;
    /// <summary>
    /// The in-game name of the card; different from card name which is for backend use
    /// </summary>
    private string cardTitle;
    /// <summary>
    /// The type of the card
    /// </summary>
    private Card.CardType cardType;
    /// The subtypes of each of the card types
    private Card.TowerSubtype towerSubtypes;
    private Card.SpellSubtype spellSubtypes;
    private Card.SkillSubtype skillSubtypes;

    /// <summary>
    /// The tower prefab which is placed by a Tower card
    /// </summary>
    private GameObject towerPrefab;
    /// <summary>
    /// The list of effects for an Instant card; happens in order
    /// </summary>
    private List<CardEffect> cardEffects = new List<CardEffect>();

    GUIStyle boldStyle = new GUIStyle();

    [MenuItem("Utilities/Card Generator")]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow<CardGenerator>("Card Generator");
        window.minSize = new Vector2(400, 200);
        window.Show();
    }

    [MenuItem("Utilities/Close Card Generator")]
    public static void CloseWindow()
    {
        if (EditorWindow.HasOpenInstances<CardGenerator>())
        {
            var window = GetWindow<CardGenerator>("Card Generator");
            window.Close();
        }
    }

    void CreateGUI()
    {
        if (cardName != "")
            loadCard();

        boldStyle.fontStyle = FontStyle.Bold;
    }

    void OnGUI()
    {
        GUI.contentColor = Color.white;

        EditorGUILayout.Space(3);
        fileInfo();
        EditorGUILayout.Space(7);

        basicInfo();
        EditorGUILayout.Space(7);

        if (cardType == Card.CardType.Tower)
            towerInfo();
        else if (cardType == Card.CardType.Spell || cardType == Card.CardType.Skill)
            instantInfo();

    }

    /// <summary>
    /// Information about the costs,name,type,etc of a card
    /// </summary>
    void basicInfo()
    {
        GUI.backgroundColor = Color.clear;
        EditorGUILayout.BeginFoldoutHeaderGroup(true, "Card Information");

        GUI.backgroundColor = Color.grey;
        cardTitle = EditorGUILayout.TextField("Card Name", cardTitle);
        cardType = (Card.CardType)EditorGUILayout.EnumPopup("Card Type", cardType);

        EditorGUI.indentLevel++;
        switch (cardType)
        {
            case Card.CardType.Tower:
                towerSubtypes = (Card.TowerSubtype)EditorGUILayout.EnumPopup("Subtype", towerSubtypes); break;
            case Card.CardType.Spell:
                spellSubtypes = (Card.SpellSubtype)EditorGUILayout.EnumPopup("Subtype", spellSubtypes); break;
            case Card.CardType.Skill:
                skillSubtypes = (Card.SkillSubtype)EditorGUILayout.EnumPopup("Subtype", skillSubtypes); break;
        }
        EditorGUI.indentLevel--;

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    /// <summary>
    /// Information for a Tower card, mostly just the what tower it spawns
    /// </summary>
    void towerInfo()
    {
        GUI.backgroundColor = Color.clear;
        EditorGUILayout.BeginFoldoutHeaderGroup(true, "Tower Configuration");

        GUI.backgroundColor = Color.grey;
        towerPrefab = (GameObject)EditorGUILayout.ObjectField("Tower Prefab", towerPrefab, typeof(GameObject), false);

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    /// <summary>
    /// Information for a Spell/Skill card, mostly just the effects it has
    /// </summary>
    void instantInfo()
    {
        GUI.backgroundColor = Color.clear;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginFoldoutHeaderGroup(true, "Spell/Skill Configuration");
        GUI.backgroundColor = Color.grey;
        if (GUILayout.Button("Add Effect", EditorStyles.miniButtonLeft))
            cardEffects.Add(new CardEffect());

        if (cardEffects.Count == 0)
            GUI.backgroundColor = Color.white;
        else
            GUI.backgroundColor = Color.grey;
        if (GUILayout.Button("Remove Effect", EditorStyles.miniButtonRight) && cardEffects.Count > 0)
            cardEffects.RemoveAt(cardEffects.Count - 1);
        EditorGUILayout.EndHorizontal();

        for(int i = 0; i < cardEffects.Count; i++)
        {
            EditorGUILayout.LabelField("Effect " + i, EditorStyles.boldLabel);
            effectInfo(cardEffects[i]);
            EditorGUILayout.Space(5);
        }


        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    void effectInfo(CardEffect cardEffect)
    {
        EditorGUI.indentLevel++;

        MonoScript lastConditionScript = cardEffect.conditionScript;
        cardEffect.conditionScript = (MonoScript)EditorGUILayout.ObjectField("Condition", cardEffect.conditionScript, typeof(MonoScript), false);
        if (cardEffect.conditionScript != null)
        {
            if (cardEffect.conditionScript != lastConditionScript)
                cardEffect.condition = (CardEffectCondition)Activator.CreateInstance(cardEffect.conditionScript.GetClass());
        }
        else
        {
            cardEffect.condition = null;
        }

        if (cardEffect.condition != null)
            cardEffect.condition.InputGUI();

        EditorGUI.indentLevel--;
    }

    /// <summary>
    /// Information about the file representing this card
    /// </summary>
    void fileInfo()
    {
        GUI.backgroundColor = Color.clear;
        EditorGUILayout.BeginFoldoutHeaderGroup(true, "File Information");

        GUI.backgroundColor = Color.grey;
        cardName = EditorGUILayout.TextField("Filename:", cardName);

        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.grey;
        if (GUILayout.Button("Load Card", EditorStyles.miniButtonLeft))
            loadCard();
        if (GUILayout.Button("Reset Card", EditorStyles.miniButtonMid))
            resetCard();
        if (GUILayout.Button("Save Card", EditorStyles.miniButtonRight))
            saveCard();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    /// <summary>
    /// Load data from any existing card file using the card name for filename
    /// </summary>
    void loadCard()
    {
        if (cardName == "")
            return;

        // list of assets that have the correct name
        string[] result = AssetDatabase.FindAssets(cardName);

        if(result.Length == 1)
        {
            // just use the first result, shouldnt be multiple generally
            string path = AssetDatabase.GUIDToAssetPath(result[0]);
            CardData loadedData = (CardData)AssetDatabase.LoadAssetAtPath(path, typeof(CardData));

            // use loadedData to get information about card file
            cardTitle = loadedData.CardTitle;
            cardType = loadedData.Type;
            towerSubtypes = loadedData.TowerSubtypes;
            skillSubtypes = loadedData.SkillSubtypes;
            spellSubtypes = loadedData.SpellSubtypes;

            cardEffects = loadedData.CardEffects;
            towerPrefab = loadedData.TowerPrefab;
        }
    }

    /// <summary>
    /// Reset the values of the card
    /// </summary>
    void resetCard()
    {
       
    }

    void saveCard()
    {
        CardData cardData;

        // list of assets that have the card name
        string[] result = AssetDatabase.FindAssets(cardName);

        // if one already exists with this name
        if (result.Length == 1)
        {
            // just use the first result, shouldnt be multiple generally
            string path = AssetDatabase.GUIDToAssetPath(result[0]);
            cardData = (CardData)AssetDatabase.LoadAssetAtPath(path, typeof(CardData));
        }
        else // no files already have this card name so make a new asset
        {
            cardData = CreateInstance<CardData>();
        }

        cardData.CardTitle = cardTitle;
        cardData.Type = cardType;
        cardData.TowerSubtypes = towerSubtypes;
        cardData.SkillSubtypes = skillSubtypes;
        cardData.SpellSubtypes = spellSubtypes;
        
        cardData.CardEffects = cardEffects;
        cardData.TowerPrefab = towerPrefab;

        // if we have to create this new asset from scratch rather than saving to existing
        if (result.Length == 0)
        {
            string fileName = cardPath + cardName + ".asset";
            AssetDatabase.CreateAsset(cardData, fileName);
        }

        // have to set it as dirty so that Unity knows that it actually needs to save it
        EditorUtility.SetDirty(cardData);
        AssetDatabase.SaveAssets();
    }
}
