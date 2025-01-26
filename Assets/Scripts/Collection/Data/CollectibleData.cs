using R3;
using System;
using UnityEngine;

[Serializable]
public class CollectibleData
{
    [SerializeField] private int id;
    [SerializeField] private readonly SerializableReactiveProperty<int> count = new SerializableReactiveProperty<int>();

    public int Id => id;
    public ReadOnlyReactiveProperty<int> Count => count;

    public CollectibleData(int _id, int _count)
    {
        id = _id;
        count.Value = _count;
    }

    public void Add(int _count)
    {
        if (0 < _count)
        {
            count.Value += _count;
        }
    }

    public bool TrySpend(int _count)
    {
        if (0 < _count && _count <= count.CurrentValue)
        {
            count.Value -= _count;
            return true;
        }
        return false;
    }
}
