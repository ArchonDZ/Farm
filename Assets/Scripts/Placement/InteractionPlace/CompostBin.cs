using UnityEngine;

public class CompostBin : MonoBehaviour, IInteractionPlace
{
    public void Interaction(PlacementHelper placementHelper, CollectibleObject collectibleObject)
    {
        if (collectibleObject.CollectibleItem is SeedCollectibleItem ||
            collectibleObject.CollectibleItem is CropCollectibleItem)
        {
            collectibleObject.Spend();
            Debug.Log("ToCompost");
        }
    }
}
