using System;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(PlaceableObject))]
public class PlacementHelper : MonoBehaviour
{
	[SerializeField, Min(1)] private int _intervalCheck = 5;
	[SerializeField] private int _layerMaskInteractionPlace = 7;
	[SerializeField] private SpriteRenderer _spriteRenderer;
	[SerializeField] private PlaceableObject _placeableObject;
	[SerializeField] private PlacementPreview _placementPreview;

	[Inject] private readonly Camera _mainCamera;

	private Action _result;
	private Action<Action> _action;
	private Func<bool> _canPlace;
	private CollectibleObject _collectibleObject;
	private PlaceableObject _replaceableObject;

	public bool CanBePlacedOnTile => _canPlace?.Invoke() ?? false;

	void Update()
	{
		UpdatePosition();
		_action?.Invoke(_result);
	}

	public void ActivateCollectibleForEveryFrame(CollectibleObject collectibleObject, Sprite sprite)
	{
		_action = EveryFrameInterval;
		ActivateCollectible(collectibleObject, sprite);
	}

	public void ActivateCollectibleForMouseUp(CollectibleObject collectibleObject, Sprite sprite)
	{
		_action = MouseUp;
		ActivateCollectible(collectibleObject, sprite);
	}

	public void ActivateReplacePlaceable(PlaceableObject placeableObject)
	{
		if (!placeableObject.gameObject.TryGetComponent(out SpriteRenderer spriteRenderer)) return;

		_replaceableObject = placeableObject;
		_replaceableObject.gameObject.SetActive(false);
		_replaceableObject.Clear();

		_action = MouseUp;
		_canPlace = _replaceableObject.CanBePlaced;
		_result = InteractionPlaceableObject;
		Activate(spriteRenderer.sprite);
		EnablePreview();
	}

	public void FinishReplacePlaceable()
	{
		_replaceableObject.Place();
		_replaceableObject.gameObject.SetActive(true);
	}

	public void Deactivate()
	{
		_action = null;
		_canPlace = null;
		_result = null;
		gameObject.SetActive(false);
	}

	private void ActivateCollectible(CollectibleObject collectibleObject, Sprite sprite)
	{
		this._collectibleObject = collectibleObject;
		_canPlace = _placeableObject.CanBePlaced;
		_result = InteractionCollectible;
		Activate(sprite);
	}

	private void Activate(Sprite sprite)
	{
		_spriteRenderer.sprite = sprite;
		gameObject.SetActive(true);
		UpdatePosition();
	}

	private void UpdatePosition()
	{
		Vector3 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
		mousePos.z = 0;
		transform.position = mousePos;
	}

	private void EveryFrameInterval(Action action)
	{
		if (Input.GetMouseButtonUp(0))
		{
			Deactivate();
			return;
		}

		if (Time.frameCount % _intervalCheck == 0)
		{
			action?.Invoke();
		}
	}

	private void MouseUp(Action action)
	{
		if (Input.GetMouseButtonUp(0))
		{
			action?.Invoke();
			Deactivate();
		}
	}

	private void InteractionCollectible()
	{
		if (TryRaycast(out IInteractionPlace interactionPlace))
		{
			interactionPlace.Interaction(this, _collectibleObject);
		}
	}

	private void InteractionPlaceableObject()
	{
		if (TryRaycast(out IInteractionPlace interactionPlace))
		{
			interactionPlace.Interaction(this, _replaceableObject);
		}
		else
		{
			FinishReplacePlaceable();
		}
		DisablePreview();
	}

	private bool TryRaycast(out IInteractionPlace interactionPlace)
	{
		Vector2 dragPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
		RaycastHit2D hit = Physics2D.Raycast(dragPos, Vector2.zero, float.PositiveInfinity, 1 << _layerMaskInteractionPlace);
		if (hit.transform != null)
		{
			return hit.transform.TryGetComponent(out interactionPlace);
		}
		interactionPlace = null;
		return false;
	}

	private void EnablePreview()
	{
		if (_placementPreview != null && _replaceableObject != null)
			_placementPreview.Enable(_replaceableObject);
	}

	private void DisablePreview()
	{
		if (_placementPreview != null)
			_placementPreview.Disable();
	}
}
