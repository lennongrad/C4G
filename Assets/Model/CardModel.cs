using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Logical model for one card. 
/// </summary>
public class CardModel
{
    /// <summary>
    /// The actual information about the card.
    /// </summary>
    public CardData Data;

    public CardModel(CardData cardData)
    {
        Data = cardData;
    }
}
