using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeckListData
{
    public int DebugInt = 0;

    bool changedCards = true;
    public List<CardData> cards = new List<CardData>();
    public List<CardData> Cards
    {
        get { return cards; }
        set
        {
            cards = value;
            changedCards = true;
        }
    }

    Dictionary<CardData, int> cardsCount;
    public Dictionary<CardData, int> CardsCount
    {
        get
        {
            if(cardsCount == null || changedCards)
            {
                changedCards = false;
                cardsCount = new Dictionary<CardData, int>();

<<<<<<< HEAD
                foreach (CardData card in cards)
                    if (card != null)
=======
                foreach(CardData card in cards)
                {
                    if(card != null)
>>>>>>> main
                    {
                        if (!cardsCount.ContainsKey(card))
                            cardsCount[card] = 1;
                        else
                            cardsCount[card] += 1;
                    }
<<<<<<< HEAD
=======
                }
>>>>>>> main
            }

            return cardsCount;
        }
    }

    public DeckListData()
    {
    }
}
