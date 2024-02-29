using System;

[Serializable]
public class Drop
{
    public int Count;
    public CollectibleItem CollectibleItem;

    public Drop(int count, CollectibleItem collectibleItem)
    {
        Count = count;
        CollectibleItem = collectibleItem;
    }
}
