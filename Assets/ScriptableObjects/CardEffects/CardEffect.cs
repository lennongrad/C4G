using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardEffect 
{
    public string text;
    public CardEffectCondition cardEffectCondition;

    public CardEffect(CardEffectCondition cardEffectCondition)
    {
        this.cardEffectCondition = cardEffectCondition;
    }
}
