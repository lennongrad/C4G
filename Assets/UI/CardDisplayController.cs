using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplayController : MonoBehaviour
{
    public GameObject visualCardPrefab;

    /// <summary>
    /// The card zone representing the player's hand
    /// </summary>
    public CardZone HandZone;

    List<GameObject> visualCards = new List<GameObject>();

    void Start()
    {
        HandZone.RegisterCardsAdded(OnCardsAdded);

        SimplePool.Preload(visualCardPrefab, 7, this.transform);
    }

    void FixedUpdate()
    {
        for(int i = 0; i < visualCards.Count; i++)
        {
            RectTransform cardTransform = visualCards[i].GetComponent<RectTransform>();
            cardTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, (cardTransform.rect.width + 20) * i, cardTransform.rect.width);
        }
    }

    void OnCardsAdded(List<CardModel> addedCards)
    {
        foreach(CardModel model in addedCards)
        {
            GameObject newCard = SimplePool.Spawn(visualCardPrefab, this.transform.position, Quaternion.identity);
            visualCards.Add(newCard);

            VisualCardController visualCardController = newCard.GetComponent<VisualCardController>();
            visualCardController.Model = model;
        }
    }
}
