using System;
using UnityEngine;

[Serializable]
public class PlantPlacebleData : PlacebleData
{
    public PlantState State;

    public PlantPlacebleData(int id, Vector3 position, PlantState state) : base(id, position)
    {
        State = state;
    }
}
