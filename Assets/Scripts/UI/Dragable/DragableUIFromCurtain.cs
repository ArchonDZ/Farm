using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class DragableUIFromCurtain : DragableUI
{
    [SerializeField] private RectTransform rectTransformBorder;

    [Inject] private GridSystem gridSystem;
    
    private CurtainPanel curtainPanel;

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        if (dragableImage.rectTransform.position.x < rectTransformBorder.position.x - (rectTransformBorder.rect.width / 2f) || rectTransformBorder.position.x + (rectTransformBorder.rect.width / 2f) < dragableImage.rectTransform.position.x)
        {
            curtainPanel.ClosePanel();
            //gridSystem.InitiazeObjectOnPosition(item.Prefab, Camera.main.ScreenToWorldPoint(dragableImage.rectTransform.position));
            eventData.pointerDrag = null;
            OnEndDrag(eventData);
        }
    }

    public void Initialize(CurtainPanel curtainPanel)
    {
        this.curtainPanel = curtainPanel;
    }
}
