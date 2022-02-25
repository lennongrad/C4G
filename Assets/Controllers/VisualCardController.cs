using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// The UI element representing a card in the user's hand or otherwise viewable by the user
/// </summary>
public class VisualCardController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
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

    /// <summary>
    /// Which edge to align the card with horizontally
    /// </summary>
    public RectTransform.Edge horizontalEdge = RectTransform.Edge.Left;
    /// <summary>
    /// Which edge to align the card with vertically
    /// </summary>
    public RectTransform.Edge verticalEdge = RectTransform.Edge.Bottom;

    /// <summary>
    /// The x position to move the visual card to over time
    /// </summary>
    public float TargetX = 0f;
    float targetXSpeed = 0f;

    /// <summary>
    /// The y position (from the bottom of the parent) to move the visual card to over time
    /// </summary>
    public float TargetY = 0f;
    float targetYSpeed = 0f;

    /// <summary>
    /// The z-rotation to move the visual card to over time
    /// </summary>
    public float TargetRotation = 0f;
    float targetRotationSpeed = 0;
    Vector3 currentEulerAngles = Vector3.zero;
    
    /// <summary>
    /// The horizontal scale to change the visual card to over time
    /// </summary>
    public float TargetScaleX = 1f;
    float targetScaleXSpeed = 0;

    /// <summary>
    /// The vertical scale to change the visual card to over time
    /// </summary>
    public float TargetScaleY = 1f;
    float targetScaleYSpeed = 0;

    /// <summary>
    /// The width (before scaling) of the card
    /// </summary>
    public float Width = 112f;
    /// <summary>
    /// The height (before scaling) of the card
    /// </summary>
    public float Height {
        get
        {
            return Width * 7f / 5f;
        }
    }
    
    /// <summary>
    /// The ticks since the last time the card was clicked.
    /// </summary>
    public int doubleClickedTimer = 0;

    Action<VisualCardController> cbHovered;
    /// <summary>
    /// Register a function to be called when the user hovers over this card
    /// </summary>
    public void RegisterHovered(Action<VisualCardController> cb) { cbHovered += cb; }

    Action<VisualCardController> cbUnhovered;
    /// <summary>
    /// Register a function to be called when the useer stops hovering over this card
    /// </summary>
    public void RegisterUnhovered(Action<VisualCardController> cb) { cbUnhovered += cb; }

    Action<VisualCardController> cbPlayed;
    /// <summary>
    /// Register a function to be called when the user attempts to play this card
    /// </summary>
    public void RegisterPlayed(Action<VisualCardController> cb) { cbPlayed += cb; }

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
        if (doubleClickedTimer > 0)
            doubleClickedTimer--;

        RectTransform cardTransform = GetComponent<RectTransform>();

        // set horizontal position with damping
        if(horizontalEdge == RectTransform.Edge.Left)
            cardTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, Mathf.SmoothDamp(cardTransform.offsetMin.x, TargetX, ref targetXSpeed, .15f), Width);
        else
            cardTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, Mathf.SmoothDamp(-cardTransform.offsetMax.x, TargetX, ref targetXSpeed, .15f), Width);

        // set vertical position with damping
        if (verticalEdge == RectTransform.Edge.Bottom)
            cardTransform.SetInsetAndSizeFromParentEdge(verticalEdge, Mathf.SmoothDamp(cardTransform.offsetMin.y, TargetY, ref targetYSpeed, .15f), Height);
        else
            cardTransform.SetInsetAndSizeFromParentEdge(verticalEdge, Mathf.SmoothDamp(-cardTransform.offsetMax.y, TargetY, ref targetYSpeed, .15f), Height);

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

    public void OnPointerClick(PointerEventData eventData)
    {
        if(doubleClickedTimer > 0)
        {
            if(cbPlayed != null)
                cbPlayed(this);
            doubleClickedTimer = 0;
        }
        else
        {
            doubleClickedTimer = 25;
        }
    }
}
