using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class DragableUIInventory : DragableUI
{
    [SerializeField] private InventoryPanel inventoryPanel;
    [SerializeField] private RectTransform rectTransformBorder;
    [SerializeField] private GameObject prefab;

    [Inject] private GridSystem gridSystem;

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        if (rectTransform.position.x < rectTransformBorder.position.x - (rectTransformBorder.rect.width / 2f) || rectTransformBorder.position.x + (rectTransformBorder.rect.width / 2f) < rectTransform.position.x)
        {
            inventoryPanel.ClosePanel();
            gridSystem.InitiazeObjectOnPosition(prefab, Camera.main.ScreenToWorldPoint(rectTransform.position));
            eventData.pointerDrag = null;
            OnEndDrag(eventData);
        }
    }
}
