using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Master class for controlling high level aspects of the card game, such as moving cards between zones.
/// </summary>
public class CardGameController : MonoBehaviour
{
    /// <summary>
    /// The scriptable object that manages the player's resources (including mana)
    /// </summary>
    public PlayerResourceManager playerResourceManager;

    /// <summary>
    /// Event that is invoked at the end of a cycle
    /// </summary>
    public GameEvent cycleEnd;
    
    /// <summary>
    /// The cards the player starts the match with in their deck
    /// </summary>
    public List<CardData> InitialDeck;

    /// <summary>
    /// The zone representing the user's deck of cards that they can draw from
    /// </summary>
    public CardZone DeckZone;
    /// <summary>
    /// The zone representing the user's hand and thus the cards they can play
    /// </summary>
    public CardZone HandZone;

    /// <summary>
    /// The largest number of cards a player can have in hand without being prevented from drawing cards.
    /// See gameplay document for more information.
    /// </summary>
    int maximumHandSize = 7; 

    void Start()
    {
        DeckZone.ConjureList(InitialDeck);
        DeckZone.Shuffle();

        cycleEnd.RegisterListener(OnCycleEnd);

        startRound();
    }

    /// <summary>
    /// Called at the beginning of each round of the game
    /// </summary>
    void startRound()
    {
        while(!AtMaximumHandSize())
        {
            DrawCard();
        }
    }

    /// <summary>
    /// Returns whether or not the player is at their maximum hand size. 
    /// Pass in an int to see if the player would also be at max hand size with that many cards added/removed
    /// </summary>
    public bool AtMaximumHandSize(int withAdditional = 0)
    {
        return HandZone.Count >= maximumHandSize;
    }

    /// <summary>
    /// Draw one or more cards from the deck into hand.
    /// </summary>
    void DrawCard(int numberOfCards = 1)
    {
        for(int i = 0; i < numberOfCards; i++)
        {
            if (!AtMaximumHandSize() && DeckZone.Count > 0)
            {
                HandZone.Add(DeckZone.Pop());
            }
            else
            {
                playerResourceManager.WisdomTotal += 10;
            }
        }
    }

    public void OnCycleEnd()
    {
        if(true || playerResourceManager.WisdomTotal > 20 && !AtMaximumHandSize())
        {
            DrawCard();
        }
    }
}
