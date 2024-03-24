using System;
using UnityEngine;

[Serializable]
public class PlantPlaceableData : PlaceableData
{
    public int Stage;
    public PlantState State;

    public PlantPlaceableData(int id, Vector3 position, int stage, PlantState state) : base(id, position)
    {
        Stage = stage;
        State = state;
    }
}
