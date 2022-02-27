using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResolutionDisplayController : MonoBehaviour
{
    public GameObject visualCardPrefab;
    public CardResolutionController cardResolutionController;

    GameObject visualCard;

    void Awake()
    {
        cardResolutionController.RegisterCardAdded(OnCardAdded);
        cardResolutionController.RegisterCardRemoved(OnCardRemoved);
    }

    void OnCardAdded(VisualCardController visualCardController)
    {
        visualCard = visualCardController.gameObject;
        visualCard.transform.SetParent(this.transform);
        visualCardController.horizontalEdge = RectTransform.Edge.Right;
        visualCardController.TargetX = 15;
        visualCardController.TargetY = 0;
        visualCardController.TargetRotation = 0;
    }

    void OnCardRemoved()
    {
        SimplePool.Despawn(visualCard);
        visualCard = null;
    }
}
