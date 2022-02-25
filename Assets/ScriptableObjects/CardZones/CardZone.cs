using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CardZone", order = 1), System.Serializable]
public class CardZone : ScriptableObject
{
    /// <summary>
    /// Whether or not order matters in this zone. Typically only true for the deck
    /// </summary>
    public bool isOrdered;

    /// <summary>
    /// The number of cards in the zone
    /// </summary>
    public int Count { get { return cards.Count; } }

    /// <summary>
    /// The list of cards in the zone currently
    /// </summary>
    [System.NonSerialized]
    List<CardModel> cards = new List<CardModel>();

    Action<List<CardModel>> cbCardsAdded;
    /// <summary>
    /// Register a function to be called whenever one or more new cards enters this zone
    /// </summary>
    public void RegisterCardsAdded(Action<List<CardModel>> cb) { cbCardsAdded += cb; }

    Action<List<CardModel>> cbCardsRemoved;
    /// <summary>
    /// Register a function to be called whenever one or more new cards are removed from this zone
    /// </summary>
    public void RegisterCardsRemoved(Action<List<CardModel>> cb) { cbCardsRemoved += cb; }

    /// <summary>
    /// Create new card models for the card data passed in then add it to this zone
    /// </summary>
    public List<CardModel> ConjureList(List<CardData> cardDataList)
    {
        List<CardModel> addedCards = new List<CardModel>();
        foreach (CardData cardData in cardDataList)
        {
            CardModel newCard = new CardModel(cardData);

            cards.Add(newCard);
            addedCards.Add(newCard);
        }

        if(cbCardsAdded != null)
            cbCardsAdded(addedCards);
        return addedCards;
    }

    /// <summary>
    /// Randomize the order of the cards in the zone
    /// </summary>
    public void Shuffle()
    {
        if(isOrdered)
            cards.Shuffle();
    }

    // might want better methods for moving between zones
    public void Add(CardModel card)
    {
        if (cbCardsAdded != null)
            cbCardsAdded(card.IndividualList());
        cards.Add(card);
    }

    public CardModel Pop()
    {
        if (!isOrdered)
            return null;
        else
        {
            CardModel removedCard = cards[0];
            cards.RemoveAt(0);

            if (cbCardsRemoved != null)
                cbCardsRemoved(removedCard.IndividualList());
            return removedCard;
        }
    }
}
