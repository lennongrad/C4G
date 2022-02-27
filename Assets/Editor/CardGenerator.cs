using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

    private CardEffectCondition condition;

    private Card.CardType cardType;

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
            LoadCard();
    }

    void OnGUI()
    {
        // Basic card data entry fields
        cardName = EditorGUILayout.TextField("Card Name:", cardName);
        cardTitle = EditorGUILayout.TextField("Card Title:", cardTitle);
        EditorGUILayout.Space(5);

        // ObjectField
        condition = (CardEffectCondition)EditorGUILayout.ObjectField("Condition", condition, typeof(CardEffectCondition), false);

        cardType = (Card.CardType)EditorGUILayout.EnumPopup(cardType);

        // Rest of the buttons are a boring grey
        GUI.backgroundColor = Color.grey;
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load Card"))
            LoadCard();
        if (GUILayout.Button("Reset Card"))
            ResetCard();
        if (GUILayout.Button("Save Card"))
            SaveCard();
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Load data from any existing card file using the card name for filename
    /// </summary>
    void LoadCard()
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
        }
    }

    /// <summary>
    /// Reset the values of the card
    /// </summary>
    void ResetCard()
    {
    }

    void SaveCard()
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
        cardData.cardEffect = new CardEffect(condition);


        // if we have to create this new asset from scratch rather than saving to existing
        if(result.Length == 0)
        {
            string fileName = cardPath + cardName + ".asset";
            AssetDatabase.CreateAsset(cardData, fileName);
        }

        // have to set it as dirty so that Unity knows that it actually needs to save it
        EditorUtility.SetDirty(cardData);
        AssetDatabase.SaveAssets();
    }
}
