using UnityEngine;

public class CompostBin : MonoBehaviour, IInteractionPlace
{
    public void Interaction(PlacementHelper placementHelper, CollectibleObject collectibleObject)
    {
        Debug.Log("ToCompost");
    }
}
