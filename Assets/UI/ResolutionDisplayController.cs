using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ResolutionDisplayController : MonoBehaviour
{
    public GameObject visualCardPrefab;
    public CardResolutionController cardResolutionController;

    public Text TargetCountText;
    public Button SubmitButton;

    GameObject visualCard;

    void Awake()
    {
        cardResolutionController.RegisterCardAdded(onCardAdded);
        cardResolutionController.RegisterCardRemoved(onCardRemoved);
        cardResolutionController.RegisterTargetCountChanged(onTargetCountChanged);
    }

    void onCardAdded(VisualCardController visualCardController)
    {
        visualCard = visualCardController.gameObject;
        visualCard.transform.SetParent(this.transform);
        visualCardController.horizontalEdge = RectTransform.Edge.Right;
        visualCardController.TargetX = 15;
        visualCardController.TargetY = 35;
        visualCardController.TargetRotation = 0;
    }

    void onCardRemoved()
    {
        SimplePool.Despawn(visualCard);
        visualCard = null;
    }

    void onTargetCountChanged(int currentTargets, int targetsMax, bool allowSubmit)
    {
        if(targetsMax == 0)
            TargetCountText.text = "";
        else
            TargetCountText.text = currentTargets + " / " + targetsMax;

        SubmitButton.gameObject.SetActive(allowSubmit);
    }
}
