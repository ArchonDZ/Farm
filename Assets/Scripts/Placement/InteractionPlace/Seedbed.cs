using UnityEngine;
using Zenject;

public class Seedbed : MonoBehaviour, IInteractionPlace
{
    [Inject] private GridSystem gridSystem;
    [Inject] private Camera mainCamera;

    public void Interaction(PlacementHelper placementHelper, CollectibleObject collectibleObject)
    {
        if (collectibleObject.CollectibleItem is SeedCollectibleItem seedCollectibleItem)
        {
            if (placementHelper.CanBePlacedOnTile)
            {
                collectibleObject.Spend();
                gridSystem.InitializeObjectOnCellPosition(seedCollectibleItem.InitializableItem.InitializableObject, mainCamera.ScreenToWorldPoint(Input.mousePosition))
                        .Initialize(seedCollectibleItem.InitializableItem, null);
            }
        }
    }
}
