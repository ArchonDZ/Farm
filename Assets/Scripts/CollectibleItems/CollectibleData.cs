using System;

[Serializable]
public class CollectibleData
{
    public int Id;
    public int Count;

    public CollectibleData(int id, int count)
    {
        Id = id;
        Count = count;
    }
}
