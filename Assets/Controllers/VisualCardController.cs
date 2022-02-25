using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class VisualCardController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// The visual element showing the card's name
    /// </summary>
    public Text CardName;

    CardModel cardModel;
    /// <summary>
    /// The card that is being visually represented
    /// </summary>
    public CardModel Model
    {
        get { return cardModel; }
        set
        {
            cardModel = value;
            modelChanged();
        }
    }

    public float TargetX = 0f;
    float targetXSpeed = 0f;

    public float TargetY = 0f;
    float targetYSpeed = 0f;

    public float TargetRotation = 0f;
    float targetRotationSpeed = 0;
    Vector3 currentEulerAngles = Vector3.zero;

    public float TargetScaleX = 1f;
    float targetScaleXSpeed = 0;

    public float TargetScaleY = 1f;
    float targetScaleYSpeed = 0;

    public float Width = 112f;
    public float Height {
        get
        {
            return Width * 7f / 5f;
        }
    }

    Action<VisualCardController> cbHovered;
    public void RegisterHovered(Action<VisualCardController> cb) { cbHovered += cb; }
    Action<VisualCardController> cbUnhovered;
    public void RegisterUnhovered(Action<VisualCardController> cb) { cbUnhovered += cb; }

    /// <summary>
    /// Used when public Model is set to update card's visuals
    /// </summary>
    void modelChanged()
    {
        CardData data = cardModel.Data;

        CardName.text = data.CardName;
    }

    void FixedUpdate()
    {
        RectTransform cardTransform = GetComponent<RectTransform>();

        // set position with damping
        cardTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, Mathf.SmoothDamp(cardTransform.offsetMin.x, TargetX, ref targetXSpeed, .15f), Width);
        cardTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, Mathf.SmoothDamp(cardTransform.offsetMin.y, TargetY, ref targetYSpeed, .15f), Height);

        // set rotation with damping
        currentEulerAngles = new Vector3(cardTransform.eulerAngles.x, cardTransform.eulerAngles.y, Mathf.SmoothDamp(currentEulerAngles.z, TargetRotation, ref targetRotationSpeed, .15f));
        cardTransform.eulerAngles = currentEulerAngles;

        // set scales with damping
        if(TargetScaleX != 0 && TargetScaleY != 0)
            cardTransform.localScale = new Vector3(Mathf.SmoothDamp(cardTransform.localScale.x, TargetScaleX, ref targetScaleXSpeed, .15f), Mathf.SmoothDamp(cardTransform.localScale.y, TargetScaleY, ref targetScaleYSpeed, .15f), 1);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cbHovered(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cbUnhovered(this);
    }
}
