using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CardTabsController : MonoBehaviour
{
    Action<Card.SortOption> cbChangeSortOption;
    /// <summary>
    /// Register a method to be called when the user changes what option to sort by
    /// </summary>
    public void RegisterChangeSortOptionCB(Action<Card.SortOption> cb) { cbChangeSortOption -= cb; cbChangeSortOption += cb; }

    public void DeckCollectionReady()
    {
        OnClick(transform.GetChild(0).GetComponent<SortOptionController>());
    }

    public void OnClick(SortOptionController clickedOption)
    {
        foreach(Transform child in transform)
        {
            SortOptionController sortOption = child.GetComponent<SortOptionController>();

            if(sortOption != null)
            {
                sortOption.SetCurrentOption(sortOption == clickedOption);
            }
        }

        cbChangeSortOption(clickedOption.sortOption);
    }
}
