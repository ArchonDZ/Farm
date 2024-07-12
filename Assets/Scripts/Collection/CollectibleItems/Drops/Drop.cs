using System;
using UnityEngine;

[Serializable]
public class Drop
{
    [Min(0)] public int Count;
    public CollectibleItem CollectibleItem;

    public Drop(int count, CollectibleItem collectibleItem)
    {
        Count = count;
        CollectibleItem = collectibleItem;
    }
}
