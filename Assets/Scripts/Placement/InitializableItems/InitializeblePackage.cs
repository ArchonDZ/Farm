using System;

[Serializable]
public class InitializeblePackage
{
    public PlacebleData PlacebleData;
    public InitializableItem InitializableItem;

    public InitializeblePackage(PlacebleData placebleData, InitializableItem initializableItem)
    {
        PlacebleData = placebleData;
        InitializableItem = initializableItem;
    }
}
