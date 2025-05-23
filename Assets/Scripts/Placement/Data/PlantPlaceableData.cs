using System;
using UnityEngine;

[Serializable]
public class PlantPlaceableData : PlaceableData
{
    public int Stage;
    public PlantState State;

    public DateTime GrowthTime;
    public DateTime PestTime;
    public DateTime ThirstTime;

    public bool IsFertilized;

    public PlantPlaceableData(int id, Vector3 position, PlantState _state) : base(id, position)
    {
        State = _state;
    }
}
