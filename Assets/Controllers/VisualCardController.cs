using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualCardController : MonoBehaviour
{
    /// <summary>
    /// The visual element showing the card's name
    /// </summary>
    public Text CardName;

    CardModel cardModel;
    /// <summary>
    /// The card that is being visually represented
    /// </summary>
    public CardModel Model
    {
        get { return cardModel; }
        set
        {
            cardModel = value;
            modelChanged();
        }
    }

    void modelChanged()
    {
        CardData data = cardModel.Data;

        CardName.text = data.CardName;
    }
}
