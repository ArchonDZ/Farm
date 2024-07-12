using UnityEngine;
using Zenject;

public class Seedbed : MonoBehaviour, IInteractionPlace
{
    [Inject] private GridSystem gridSystem;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    public void Interaction(PlacementHelper placementHelper, CollectibleObject collectibleObject)
    {
        if (placementHelper.CanBePlacedOnTile)
        {
            collectibleObject.Spend();
            gridSystem.InitializeObjectOnCellPosition(collectibleObject.CollectiblePackage.CollectibleItem.InitializableItem.InitializableObject, mainCamera.ScreenToWorldPoint(Input.mousePosition))
                    .Initialize(collectibleObject.CollectiblePackage.CollectibleItem.InitializableItem, null);
        }
    }
}
