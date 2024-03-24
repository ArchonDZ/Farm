using System;

[Serializable]
public class InitializeblePackage
{
    public PlaceableData PlaceableData;
    public InitializableItem InitializableItem;

    public InitializeblePackage(PlaceableData placeableData, InitializableItem initializableItem)
    {
        PlaceableData = placeableData;
        InitializableItem = initializableItem;
    }
}
