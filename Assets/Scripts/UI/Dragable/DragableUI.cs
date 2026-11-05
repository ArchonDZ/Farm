using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public event Action OnBeginDragEvent;
	public event Action OnEndDragEvent;

	[SerializeField] protected Image _dragableImage;

	private Canvas _canvas;
	private Vector3 _originPos;

	public bool IsDragging { get; private set; }

	void Awake()
	{
		_canvas = GetComponentInParent<Canvas>();
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		IsDragging = true;
		_dragableImage.maskable = false;
		_originPos = _dragableImage.rectTransform.anchoredPosition;
		OnBeginDragEvent?.Invoke();
	}

	public virtual void OnDrag(PointerEventData eventData)
	{
		_dragableImage.rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		IsDragging = false;
		_dragableImage.maskable = true;
		_dragableImage.rectTransform.anchoredPosition = _originPos;
		OnEndDragEvent?.Invoke();
	}
}
