using System;
using System.Collections.Generic;

[Serializable]
public class WeightedList<T>
{
    public List<Weighted<T>> Values = new List<Weighted<T>>();

    public T GetRandomValue()
    {
        int totalWeight = 0;
        for (int i = 0; i < Values.Count; i++)
        {
            totalWeight += Values[i].Weight;
        }

        int randomWeightValue = UnityEngine.Random.Range(1, totalWeight + 1);
        int processedWeight = 0;
        for (int i = 0; i < Values.Count; i++)
        {
            processedWeight += Values[i].Weight;
            if (randomWeightValue <= processedWeight)
            {
                return Values[i].Item;
            }
        }

        return default;
    }
}
