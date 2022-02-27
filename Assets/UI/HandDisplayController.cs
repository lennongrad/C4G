using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class HandDisplayController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject visualCardPrefab;
    public CardResolutionController cardResolutionController;

    public float HorizontalCardDisplacement;
    public float VerticalCardDisplacement;
    public float RotationCardDisplacement;
    public float HoveredCardHorizontalDisplacement;
    public float HoveredCardVerticalDisplacement;
    public float HoveredCardHorizontalScale = 1f;
    public float HoveredCardVerticalScale = 1f;

    public GameObject cardHolder;

    /// <summary>
    /// The card zone representing the player's hand
    /// </summary>
    public CardZone HandZone;

    /// <summary>reg
    /// List of the actual card objects the player sees
    /// </summary>
    List<GameObject> visualCards = new List<GameObject>();

    /// <summary>
    /// The last card in hand to be hovered over by the user. Null if they haven't hovered
    /// </summary>
    VisualCardController lastHovered = null;

    Action<bool> cbHoveredChanged;
    public void RegisterHoveredChanged(Action<bool> cb) { cbHoveredChanged += cb; }

    void Awake()
    {
        cardHolder = transform.GetChild(0).gameObject;
        HandZone.RegisterCardsAdded(OnCardsAdded);
        SimplePool.Preload(visualCardPrefab, 7, cardHolder.transform);
    }

    int debugTimer = 0;
    void FixedUpdate()
    {
        debugTimer += 1;
        if(debugTimer > 10)
        {
            debugTimer = 0;
            CardVisualUpdate();
        }
    }

    /// <summary>
    /// Needs to be called whenever something about the visual cards changes so it can update their position
    /// </summary>
    void CardVisualUpdate()
    {
        bool pastHovered = false;

        for (int i = 0; i < visualCards.Count; i++)
        {
            RectTransform cardTransform = visualCards[i].GetComponent<RectTransform>();
            VisualCardController controller = visualCards[i].GetComponent<VisualCardController>();

            // set the scales back to normal to start off
            controller.TargetScaleX = 1;
            controller.TargetScaleY = 1;

            // set target x position
            controller.TargetX = (controller.Width - HorizontalCardDisplacement) * ((float)i - (float)visualCards.Count / 2f - .5f) + cardHolder.GetComponent<RectTransform>().rect.width / 2f;

            // set target y position, with cards in the middle being higher
            controller.TargetY = VerticalCardDisplacement * Mathf.Log(1 + -Mathf.Abs(i + 1 - ((float)visualCards.Count + 1f) / 2f) + ((float)visualCards.Count - 1f) / 2f);

            // set target rotation angle
            controller.TargetRotation = RotationCardDisplacement * -((float)i - (float)(visualCards.Count - 1) / 2f);

            // if a card is hovered we have a move others over
            if (lastHovered != null)
            {
                if (visualCards[i] == lastHovered.gameObject)
                {
                    pastHovered = true;
                    // make it bigger
                    controller.TargetScaleX = HoveredCardHorizontalScale;
                    controller.TargetScaleY = HoveredCardVerticalScale;
                    // shift up since itll be bigger
                    controller.TargetY += HoveredCardVerticalDisplacement;
                }
                else if (pastHovered)
                {
                    // already updated the hovered card so shift right
                    controller.TargetX += HoveredCardHorizontalDisplacement;
                }
                else
                {
                    // havent updated the hovered card yet so shift left
                    controller.TargetX -= HoveredCardHorizontalDisplacement;
                }
            }
        }
    }

    void onCardHover(VisualCardController visualCardController)
    {
        if(visualCards.Contains(visualCardController.gameObject))
        {
            lastHovered = visualCardController;
            CardVisualUpdate();
        }
    }

    void onCardUnhover(VisualCardController visualCardController)
    {
        if(lastHovered == visualCardController)
        {
            lastHovered = null;
            CardVisualUpdate();
        }
    }

    void onCardPlayed(VisualCardController visualCardController)
    {
        if(!cardResolutionController.IsBusy)
        {
            cardResolutionController.PlayCard(visualCardController);

            visualCards.Remove(visualCardController.gameObject);
            //SimplePool.Despawn(visualCardController.gameObject);
            CardVisualUpdate();
        }
    }

    void OnCardsAdded(List<CardModel> addedCards)
    {
        foreach(CardModel model in addedCards)
        {
            GameObject newCard = SimplePool.Spawn(visualCardPrefab, this.transform.position, Quaternion.identity);
            visualCards.Add(newCard);
            newCard.transform.SetParent(this.transform);

            VisualCardController visualCardController = newCard.GetComponent<VisualCardController>();
            visualCardController.Model = model;

            visualCardController.RegisterHovered(onCardHover);
            visualCardController.RegisterUnhovered(onCardUnhover);
            visualCardController.RegisterPlayed(onCardPlayed);
        }

        CardVisualUpdate();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cbHoveredChanged(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cbHoveredChanged(false);
    }
}
