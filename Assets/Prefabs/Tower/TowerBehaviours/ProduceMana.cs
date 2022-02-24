using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProduceMana : TowerBehaviour
{
    /// <summary>
    /// Event that is invoked at the end of a cycle
    /// </summary>
    public GameEvent cycleEnd;

    /// <summary>
    /// The player's mana pool
    /// </summary>
    public ManaPoolController manaPool;

    /// <summary>
    /// The mana type to be produced by the tower
    /// </summary>
    public Mana.ManaType manaType;

    protected override void Initiate()
    {
        cycleEnd.RegisterListener(OnCycleEnd);
    }

    // empty because mana tower reacts to cycles, not to behave calls
    protected override void Behave(){ return;  }

    public void OnCycleEnd()
    {
        if(performBehaviour && mainController.PerformBehaviours)
            manaPool.AddMana(Mana.ManaType.Spades, 1);
    }
}
