using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Manages the process of resolving the action of a player playing a card
/// </summary>
public class CardResolutionController : MonoBehaviour
{
    Action<CardModel> cbResolutionFinished;
    /// <summary>
    /// Register a function to be called when this card's resolution is complete
    /// </summary>
    public void RegisterResolutionFinished(Action<CardModel> cb) { cbResolutionFinished += cb; }

    Action<VisualCardController> cbCardAdded;
    /// <summary>
    /// Register a function to be called when a new active card is set.
    /// </summary>
    public void RegisterCardAdded(Action<VisualCardController> cb) { cbCardAdded += cb; }

    Action cbCardRemoved;
    /// <summary>
    /// Register a function to be called when the active card is removed, such as upon resolution
    /// </summary>
    public void RegisterCardRemoved(Action cb) { cbCardRemoved += cb; }

    /// <summary>
    /// The card that the controller is attempting to resolve
    /// </summary>
    CardModel activeCard = null;

    /// <summary>
    /// Public access to tell whether a card is currently resolving
    /// </summary>
    public bool IsBusy
    {
        get
        {
            return activeCard != null;
        }
    }

    void FixedUpdate()
    {
        if (activeCard != null)
            attemptResolution();
    }
    
    /// <summary>
    /// Request for a card to be played.
    /// </summary>
    public bool PlayCard(VisualCardController visualCard)
    {
        if(!IsBusy)
        {
            setActiveCard(visualCard.Model);
            if (cbCardAdded != null)
                cbCardAdded(visualCard);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void attemptResolution()
    {
        if(activeCard.Data.CardEffects.Count > 0)
            Debug.Log(activeCard.Data.CardEffects[0].condition.CheckCondition());
    }

    private void setActiveCard(CardModel cardModel)
    {
        activeCard = cardModel;
    }

    private void removeActiveCard()
    {
        activeCard = null;
        if (cbCardRemoved != null)
            cbCardRemoved();
    }
}
