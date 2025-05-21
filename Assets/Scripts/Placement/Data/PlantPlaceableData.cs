using System;
using UnityEngine;

[Serializable]
public class PlantPlaceableData : PlaceableData
{
    public int Stage;
    public PlantState State;

    public DateTime TimeStartGrowth;
    public DateTime TimeStartPest;
    public DateTime TimeStartThirst;
    public DateTime TimeStartFertilize;

    public float RemainingGrowthTime;
    public float RemainingPestTime;
    public float RemainingThirstTime;
    public float RemainingFertilizeTime;
    public float AccelerationGrowth;

    public bool IsFertilized;

    public PlantPlaceableData(int id, Vector3 position, PlantState _state) : base(id, position)
    {
        State = _state;
    }
}
