using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameArea : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public event Action OnPointerClickEvent;

    private bool isDragging;

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isDragging)
        {
            OnPointerClickEvent?.Invoke();
        }
    }
}
