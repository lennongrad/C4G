using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ManaPoolController : ScriptableObject
{
    Dictionary<Mana.ManaType, int> manaCount;

    public void OnEnable()
    {
        manaCount = new Dictionary<Mana.ManaType, int>()
        {
            { Mana.ManaType.Clubs, 0 },
            { Mana.ManaType.Spades, 0 },
            { Mana.ManaType.Hearts, 0},
            { Mana.ManaType.Diamonds, 0 }
        };
    }

    public string debugString()
    {
        string returnString = "";
        foreach(KeyValuePair<Mana.ManaType, int> kvp in manaCount)
        {
            returnString += kvp.Key.ToString() + " " + kvp.Value + "\n";
        }
        return returnString;
    }

    public void AddMana(Mana.ManaType type, int amount)
    {
        manaCount[type] += amount;
    }
}
