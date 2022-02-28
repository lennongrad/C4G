using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CardData", order = 1), System.Serializable]
public class CardData : ScriptableObject
{
    public string CardTitle = "";
    public Card.CardType Type;
    public Card.TowerSubtype TowerSubtypes;
    public Card.SpellSubtype SpellSubtypes;
    public Card.SkillSubtype SkillSubtypes;

    public List<CardEffect> CardEffects = new List<CardEffect>();
    public GameObject TowerPrefab;
}
