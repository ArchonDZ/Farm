using UnityEngine;
using Zenject;

public class Seedbed : MonoBehaviour, IInteractionPlace
{
	[Inject] private readonly GridSystem _gridSystem;
	[Inject] private readonly Camera _mainCamera;

	public void Interaction(PlacementHelper placementHelper, CollectibleObject collectibleObject)
	{
		if (collectibleObject.CollectibleItem is SeedCollectibleItem seedCollectibleItem)
		{
			if (placementHelper.CanBePlacedOnTile)
			{
				collectibleObject.Spend();
				_gridSystem.InitializeObjectOnCellPosition(seedCollectibleItem.InitializableItem.InitializableObject, _mainCamera.ScreenToWorldPoint(Input.mousePosition))
						.Initialize(seedCollectibleItem.InitializableItem, null);
			}
		}
	}

	public void Interaction(PlacementHelper placementHelper, PlaceableObject placeableObject)
	{
		if (!placeableObject.TryGetComponent(out Plant _)) return;

		if (placementHelper.CanBePlacedOnTile)
		{
			placeableObject.transform.position = _gridSystem.GetGridPosition(placementHelper.transform.position);
		}

		placementHelper.FinishReplacePlaceable();
	}
}
