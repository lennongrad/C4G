using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[System.Serializable]
public class CardData : ScriptableObject
{
    public string CardTitle = "";
    //public List<Mana.ManaType> ManaCost = new List<Mana.ManaType>();
    public int[] ManaCosts = { 0, 0, 0, 0, 0 };

    public Card.CardType Type;
    public Card.TowerSubtype TowerSubtypes;
    public Card.SpellSubtype SpellSubtypes;
    public Card.SkillSubtype SkillSubtypes;

    public List<CardEffect> CardEffects = new List<CardEffect>();
    public GameObject TowerPrefab;


    static public Color GetColorOfManaType(Mana.ManaType type)
    {
        switch (type)
        {
            case Mana.ManaType.None: return new Color(0.637416f, 0.681f, 0.6787649f);
            case Mana.ManaType.Clubs: return Color.green;
            case Mana.ManaType.Spades: return new Color(.8f, 0, .8f);
            case Mana.ManaType.Hearts: return Color.red; 
            case Mana.ManaType.Diamonds: return Color.yellow;
        }
        return Color.white;
    }

    static public Sprite GetSpriteOfManaType(Mana.ManaType type)
    {
        switch (type)
        {
            case Mana.ManaType.None: return Resources.Load<Sprite>("CostIcons/none");
            case Mana.ManaType.Clubs: return Resources.Load<Sprite>("CostIcons/clubs");
            case Mana.ManaType.Spades: return Resources.Load<Sprite>("CostIcons/spades");
            case Mana.ManaType.Hearts: return Resources.Load<Sprite>("CostIcons/hearts");
            case Mana.ManaType.Diamonds: return Resources.Load<Sprite>("CostIcons/diamonds");
        }
        return null;
    }

    static public string GetUnicodeOfManaType(Mana.ManaType type)
    {
        switch (type)
        {
            case Mana.ManaType.None: return "-";
            case Mana.ManaType.Clubs: return "♧";
            case Mana.ManaType.Spades: return "♤";
            case Mana.ManaType.Hearts: return "♡";
            case Mana.ManaType.Diamonds: return "♢";
        }
        return "-";
    }

    public Dictionary<Mana.ManaType, int> ManaCostDictionary
    {
        get
        {
            return new Dictionary<Mana.ManaType, int>()
            {
                { Mana.ManaType.None, ManaCosts[0] },
                { Mana.ManaType.Clubs, ManaCosts[1] },
                { Mana.ManaType.Spades, ManaCosts[2] },
                { Mana.ManaType.Hearts, ManaCosts[3] },
                { Mana.ManaType.Diamonds, ManaCosts[4] }
            };
        }
    }

    public int ManaValue
    {
        get
        {
            return ManaCosts.Sum();
        }
    }

    /// <summary>
    /// Called in Card Generator to generate the editor GUI for editing this data
    /// </summary>
    public void OnInputGUI()
    {
        GUI.backgroundColor = Color.clear;
        EditorGUILayout.BeginFoldoutHeaderGroup(true, "Card Information");

        GUI.backgroundColor = Color.grey;
        CardTitle = EditorGUILayout.TextField("Card Name", CardTitle);
        Type = (Card.CardType)EditorGUILayout.EnumPopup("Card Type", Type);

        EditorGUI.indentLevel++;
        switch (Type)
        {
            case Card.CardType.Tower:
                TowerSubtypes = (Card.TowerSubtype)EditorGUILayout.EnumPopup("Subtype", TowerSubtypes); break;
            case Card.CardType.Spell:
                SpellSubtypes = (Card.SpellSubtype)EditorGUILayout.EnumPopup("Subtype", SpellSubtypes); break;
            case Card.CardType.Skill:
                SkillSubtypes = (Card.SkillSubtype)EditorGUILayout.EnumPopup("Subtype", SkillSubtypes); break;
        }
        EditorGUI.indentLevel--;


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Mana Costs", GUILayout.Width(80));
        GUILayout.FlexibleSpace();
        int i = 0;
        foreach (Mana.ManaType type in System.Enum.GetValues(typeof(Mana.ManaType)))
        {
            GUI.backgroundColor = GetColorOfManaType(type);
            ManaCosts[i] = (int)EditorGUILayout.IntField("", ManaCosts[i], GUILayout.Width((Screen.width - 170) / System.Enum.GetValues(typeof(Mana.ManaType)).Length));
            i++;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        foreach (Mana.ManaType type in System.Enum.GetValues(typeof(Mana.ManaType)))
            EditorGUILayout.LabelField(GetUnicodeOfManaType(type), GUILayout.Width((Screen.width - 170) / System.Enum.GetValues(typeof(Mana.ManaType)).Length));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(3);
        EditorGUILayout.HelpBox(GetDescription(null), MessageType.None);
        EditorGUILayout.Space(3);

        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space(7);

        if (Type == Card.CardType.Tower)
            towerInfo();
        else if (Type == Card.CardType.Spell || Type == Card.CardType.Skill)
            instantInfo();
    }

    /// <summary>
    /// Information for a Tower card, mostly just the what tower it spawns
    /// </summary>
    void towerInfo()
    {
        GUI.backgroundColor = Color.clear;
        EditorGUILayout.BeginFoldoutHeaderGroup(true, "Tower Configuration");

        GUI.backgroundColor = Color.grey;
        TowerPrefab = (GameObject)EditorGUILayout.ObjectField("Tower Prefab", TowerPrefab, typeof(GameObject), false);

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
            CardEffects.Add(new CardEffect());

        if (CardEffects.Count == 0)
            GUI.backgroundColor = Color.white;
        else
            GUI.backgroundColor = Color.grey;
        if (GUILayout.Button("Remove Effect", EditorStyles.miniButtonRight) && CardEffects.Count > 0)
            CardEffects.RemoveAt(CardEffects.Count - 1);
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < CardEffects.Count; i++)
        {
            EditorGUILayout.LabelField("Effect " + i, EditorStyles.boldLabel);
            CardEffects[i].OnInputGUI();
            EditorGUILayout.Space(5);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    public string GetDescription(WorldInfo worldInfo)
    {
        string resultString = "";
        foreach(CardEffect effect in CardEffects)
        {
            resultString += effect.GetDescription(worldInfo) + "\n";
        }
        return resultString;
    }
}
