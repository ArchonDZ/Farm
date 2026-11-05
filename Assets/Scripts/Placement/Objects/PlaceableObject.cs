using R3;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class PlaceableObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public event Action OnPlaceEvent;

	[SerializeField] private BoundsInt _area = new(Vector3Int.zero, Vector3Int.one);
	[SerializeField] private bool _isReplaceable;
	[SerializeField] private float _replacementInterval = 1f;

	[Inject] private readonly GridSystem _gridSystem;
	[Inject] private readonly PlacementHelper _placementHelper;

	private IDisposable _disposableTimerToReplace;

	public bool IsPlaced { get; private set; }
	public BoundsInt Area => _area;

	#region IPointerDownHandler
	public void OnPointerDown(PointerEventData eventData)
	{
		if (_isReplaceable)
		{
			_disposableTimerToReplace = Observable.Timer(TimeSpan.FromSeconds(_replacementInterval))
				.Subscribe((_) => _placementHelper.ActivateReplacePlaceable(this));
		}
	}
	#endregion

	#region IPointerUpHandler
	public void OnPointerUp(PointerEventData eventData)
	{
		if (_isReplaceable)
		{
			_disposableTimerToReplace?.Dispose();
		}
	}
	#endregion

	void OnDestroy()
	{
		_disposableTimerToReplace?.Dispose();
	}

	public bool CanBePlaced()
	{
		BoundsInt areaTemp = _area;
		areaTemp.position = _gridSystem.GridLayout.WorldToCell(transform.position);
		return _gridSystem.CanTakeArea(areaTemp);
	}

	public void Place()
	{
		BoundsInt areaTemp = _area;
		areaTemp.position = _gridSystem.GridLayout.WorldToCell(transform.position);
		_gridSystem.TakeArea(areaTemp);
		IsPlaced = true;
		OnPlaceEvent?.Invoke();
	}

	public bool TryPlace()
	{
		if (CanBePlaced())
		{
			Place();
			return true;
		}

		return false;
	}

	public void Clear()
	{
		if (IsPlaced)
		{
			BoundsInt areaTemp = _area;
			areaTemp.position = _gridSystem.GridLayout.WorldToCell(transform.position);
			_gridSystem.ClearArea(areaTemp);
			IsPlaced = false;
		}
	}
}
