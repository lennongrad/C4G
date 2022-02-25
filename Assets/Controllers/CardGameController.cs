using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    int maximumHandSize = 3; //7;

    void Start()
    {
        DeckZone.ConjureList(InitialDeck);
        DeckZone.Shuffle();

        cycleEnd.RegisterListener(OnCycleEnd);

        startRound();

        HandZone.PrintDebug();
    }

    void FixedUpdate()
    {
        
    }

    void startRound()
    {
        while(!AtMaximumHandSize())
        {
            DrawCard();
        }
    }

    public bool AtMaximumHandSize(int withAdditional = 0)
    {
        return HandZone.Count >= maximumHandSize;
    }

    void DrawCard(int numberOfCards = 1)
    {
        for(int i = 0; i < numberOfCards; i++)
        {
            if (!AtMaximumHandSize())
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
        if(playerResourceManager.WisdomTotal > 20 && !AtMaximumHandSize())
        {
            DrawCard();
        }
    }
}
