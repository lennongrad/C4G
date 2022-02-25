using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResolutionDisplayController : MonoBehaviour
{
    public GameObject visualCardPrefab;
    public CardResolutionController cardResolutionController;

    GameObject visualCard;

    void Start()
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

        //visualCard.SetActive(true);
        //visualCard = SimplePool.Spawn(visualCardPrefab, this.transform.position, Quaternion.identity);

        //VisualCardController visualCardController = visualCard.GetComponent<VisualCardController>();
        //visualCardController.Model = cardModel;
    }

    void OnCardRemoved()
    {
        SimplePool.Despawn(visualCard);
        visualCard = null;
        //if(visualCard.GetComponent<VisualCardController>().Model == cardModel)
        //    visualCard.SetActive(false);
    }
}
