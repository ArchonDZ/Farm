using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragableUIFromCurtain : DragableUI
{
	public event Action OnLeftCurtainEvent;

	[SerializeField] private RectTransform _rectTransformBorder;

	private CurtainPanel _curtainPanel;

	public override void OnDrag(PointerEventData eventData)
	{
		base.OnDrag(eventData);
		if (_dragableImage.rectTransform.position.x < _rectTransformBorder.position.x - (_rectTransformBorder.rect.width / 2f) || _rectTransformBorder.position.x + (_rectTransformBorder.rect.width / 2f) < _dragableImage.rectTransform.position.x)
		{
			_curtainPanel.ClosePanel();
			eventData.pointerDrag = null;
			OnEndDrag(eventData);
			OnLeftCurtainEvent?.Invoke();
		}
	}

	public void Initialize(CurtainPanel curtainPanel)
	{
		_curtainPanel = curtainPanel;
	}
}
