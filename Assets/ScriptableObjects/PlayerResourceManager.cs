using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResourceManager : ScriptableObject
{
    Dictionary<Mana.ManaType, int> manaTotal;

    int lifeTotal;
    public int LifeTotal { get { return lifeTotal; } set { lifeTotal = value;  } }

    int wisdomTotal;
    public int WisdomTotal { get { return wisdomTotal; } set { wisdomTotal = value; } }

    int creativityTotal;
    public int CreativityTotal { get { return creativityTotal; } set { creativityTotal = value; } }

    public void OnEnable()
    {
        Reset();
    }

    public string debugString()
    {
        string returnString = "";
        foreach(KeyValuePair<Mana.ManaType, int> kvp in manaTotal)
        {
            returnString += kvp.Key.ToString() + ": " + kvp.Value + "\n";
        }
        returnString += "Life: " + lifeTotal + "\n";
        returnString += "Wisdom: " + wisdomTotal + "\n";
        returnString += "Creativity: " + creativityTotal + "\n";

        return returnString;
    }

   public void Reset()
    {
        manaTotal = new Dictionary<Mana.ManaType, int>()
        {
            { Mana.ManaType.Clubs, 0 },
            { Mana.ManaType.Spades, 0 },
            { Mana.ManaType.Hearts, 0},
            { Mana.ManaType.Diamonds, 0 }
        };
        lifeTotal = 20;
        wisdomTotal = 0;
        creativityTotal = 0;
    }

    public void AddMana(Mana.ManaType type, int amount)
    {
        manaTotal[type] += amount;
    }
}
