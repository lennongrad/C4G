using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ResolutionDisplayController : MonoBehaviour
{
    public GameObject cardPrefab;
    public CardResolutionController cardResolutionController;

    public Text TargetCountText;
    public Button SubmitButton;

    CardController currentCard;

    void Awake()
    {
        cardResolutionController.RegisterCardAdded(onCardAdded);
        cardResolutionController.RegisterCardRemoved(onCardRemoved);
        cardResolutionController.RegisterTargetCountChanged(onTargetCountChanged);
    }

    void onCardAdded(CardController cardController)
    {
        currentCard = cardController;

        currentCard.transform.SetParent(this.transform);
        currentCard.horizontalEdge = RectTransform.Edge.Right;
        currentCard.TargetX = 15;
        currentCard.TargetY = 35;
        currentCard.TargetRotation = 0;
    }

    void onCardRemoved()
    {
        currentCard = null;
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
