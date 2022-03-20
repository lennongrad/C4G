using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

// for when you want only a specific part of the UI element to be clickable/hoverable 
public class UICollider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    Action cbPointerEntered;
    public void RegisterPointerEntered(Action cb) { cbPointerEntered -= cb; cbPointerEntered += cb; }
    public void UnregisterPointerEntered(Action cb) { cbPointerEntered -= cb; }

    Action cbPointerExited;
    public void RegisterPointerExited(Action cb) { cbPointerExited -= cb; cbPointerExited += cb; }
    public void UnregisterPointerExited(Action cb) { cbPointerExited -= cb; }

    Action cbClicked;
    public void RegisterClicked(Action cb) { cbClicked -= cb; cbClicked += cb; }
    public void UnregisterClicked(Action  cb) { cbClicked -= cb; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (cbPointerEntered != null)
            cbPointerEntered();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (cbPointerExited != null)
            cbPointerExited();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (cbClicked != null)
            cbClicked();
    }
}
