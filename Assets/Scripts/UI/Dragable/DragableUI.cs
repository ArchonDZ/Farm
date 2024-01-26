using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public event Action OnBeginDragEvent;
    public event Action OnEndDragEvent;

    [SerializeField] protected Image dragableImage;

    private Canvas canvas;
    private Vector3 originPos;

    public bool IsDragging { get; private set; }

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        IsDragging = true;
        dragableImage.maskable = false;
        originPos = dragableImage.rectTransform.anchoredPosition;
        OnBeginDragEvent?.Invoke();
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        dragableImage.rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsDragging = false;
        dragableImage.maskable = true;
        dragableImage.rectTransform.anchoredPosition = originPos;
        OnEndDragEvent?.Invoke();
    }
}
