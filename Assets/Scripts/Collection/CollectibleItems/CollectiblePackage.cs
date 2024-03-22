using System;

[Serializable]
public class CollectiblePackage
{
    public CollectibleData CollectibleData;
    public CollectibleItem CollectibleItem;

    public CollectiblePackage(CollectibleData collectibleData, CollectibleItem collectibleItem)
    {
        CollectibleData = collectibleData;
        CollectibleItem = collectibleItem;
    }
}
