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
    private string cardPath = "Assets/ScriptableObjects/Cards/";
    CardData cardData;

    /// <summary>
    /// Name of the folder to save and load the card from
    /// </summary>
    private string folderName;

    /// <summary>
    /// Card name used in filename of the card
    /// </summary>
    private string cardName;

    /// <summary>
    /// Whether to show the warning to the user that they tried to save over an existing object
    /// </summary>
    private bool showSaveOverWarning = false;

    GUIStyle boldStyle = new GUIStyle();

    [MenuItem("Utilities/Card Generator/Open Window")]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow<CardGenerator>("Card Generator");
        window.minSize = new Vector2(400, 200);
        window.Show();
    }

    [MenuItem("Utilities/Card Generator/Close Window")]
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

        GUI.backgroundColor = Color.clear;
        EditorGUILayout.BeginFoldoutHeaderGroup(true, "File Information");

        GUI.backgroundColor = Color.grey;
        folderName = EditorGUILayout.TextField("Folder:", folderName);

        string lastCardName = cardName;
        cardName = EditorGUILayout.TextField("Filename:", cardName);
        if (cardName != lastCardName)
            cardData = Instantiate(cardData);

        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.grey;
        if (GUILayout.Button("Load Card", EditorStyles.miniButtonLeft))
            loadCard();
        if (GUILayout.Button("New Card", EditorStyles.miniButtonMid))
            newCard();
        if (GUILayout.Button("Save Card", EditorStyles.miniButtonRight))
            saveCard();
        EditorGUILayout.EndHorizontal();

        if(showSaveOverWarning)
            EditorGUILayout.HelpBox("Card already exists with that filename.", MessageType.Error);

        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(7);

        cardData.OnInputGUI();
    }

    /// <summary>
    /// Load data from any existing card file using the card name for filename
    /// </summary>
    void loadCard()
    {
        showSaveOverWarning = false;

        if (cardName == "")
        {
            newCard();
            return;
        }

        // list of assets that have the correct name
        string[] result = AssetDatabase.FindAssets(cardName, new string[] { "Assets/ScriptableObjects/Cards/" + folderName });

        if (result.Length == 1)
        {
            // just use the first result, shouldnt be multiple generally
            string path = AssetDatabase.GUIDToAssetPath(result[0]);
            cardData = (CardData)AssetDatabase.LoadAssetAtPath(path, typeof(CardData));
        }
        else
        {
            newCard();
        }

    }

    /// <summary>
    /// Reset the values of the card
    /// </summary>
    void newCard()
    {
        cardData = (CardData)ScriptableObject.CreateInstance(typeof(CardData));
        showSaveOverWarning = false;
        cardName = "";
    }

    void saveCard()
    {
        string[] result = AssetDatabase.FindAssets(cardName, new string[] { "Assets/ScriptableObjects/Cards/" + folderName });
        if (result.Length == 0)
        {
            // if we have to create this new asset from scratch rather than saving to existing
            string fileName = cardPath + folderName + "/" + cardName + ".asset";
            AssetDatabase.CreateAsset(cardData, fileName);
            showSaveOverWarning = false;
        }
        else if(result.Length == 1)
        {
            string path = AssetDatabase.GUIDToAssetPath(result[0]);
            CardData loadedCardData = (CardData)AssetDatabase.LoadAssetAtPath(path, typeof(CardData));
            if(loadedCardData != cardData)
            {
                // theres already an object with this name whcih isnt this object so abort
                showSaveOverWarning = true;
                return;
            }
            else
            {
                // filename already exists.. because weve loaded from it
                showSaveOverWarning = false;
            }
        }
        else
        {
            // multiple files with this name so who knows
            showSaveOverWarning = true;
            return;
        }

        // have to set it as dirty so that Unity knows that it actually needs to save it
        EditorUtility.SetDirty(cardData);
        AssetDatabase.SaveAssets();
    }
}
