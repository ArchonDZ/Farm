using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] protected Image dragableImage;

    private Canvas canvas;
    private Vector3 originPos;

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragableImage.maskable = false;
        originPos = dragableImage.rectTransform.anchoredPosition;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        dragableImage.rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragableImage.maskable = true;
        dragableImage.rectTransform.anchoredPosition = originPos;
    }
}
