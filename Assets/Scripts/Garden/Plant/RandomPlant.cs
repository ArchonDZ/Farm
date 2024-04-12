using UnityEngine;

public class RandomPlant : Plant
{
    public override void Initialize(InitializableItem initializableItem, PlaceableData placeableData)
    {
        if (initializableItem is RandomPlantItem randomPlantItem)
        {
            initializableItem = randomPlantItem.Plants[Random.Range(0, randomPlantItem.Plants.Count)];
        }
        base.Initialize(initializableItem, placeableData);
    }
}
