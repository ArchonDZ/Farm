using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class DragableUIFromCurtain : DragableUI
{
    [SerializeField] private RectTransform rectTransformBorder;
    [SerializeField] private CurtainPanel curtainPanel;
    [SerializeField] private CollectibleItem item;

    [Inject] private GridSystem gridSystem;

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        if (dragableImage.rectTransform.position.x < rectTransformBorder.position.x - (rectTransformBorder.rect.width / 2f) || rectTransformBorder.position.x + (rectTransformBorder.rect.width / 2f) < dragableImage.rectTransform.position.x)
        {
            curtainPanel.ClosePanel();
            gridSystem.InitiazeObjectOnPosition(item.prefab, Camera.main.ScreenToWorldPoint(dragableImage.rectTransform.position));
            eventData.pointerDrag = null;
            OnEndDrag(eventData);
        }
    }

    public void Initialize(CurtainPanel curtainPanel, CollectibleItem item)
    {
        this.curtainPanel = curtainPanel;
        this.item = item;
    }
}
