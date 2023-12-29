using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Canvas canvas;
    [SerializeField] protected RectTransform rectTransform;
    [SerializeField] private Image image;

    private Vector3 originPos;

    public void OnBeginDrag(PointerEventData eventData)
    {
        image.maskable = false;
        originPos = rectTransform.anchoredPosition;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.maskable = true;
        rectTransform.anchoredPosition = originPos;
    }
}
