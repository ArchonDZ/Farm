using System;
using UnityEngine;

[Serializable]
public class PlantPlaceableData : PlaceableData
{
    [SerializeField] private int stage;
    [SerializeField] private PlantState state;

    public int Stage => stage;
    public PlantState State => state;

    public PlantPlaceableData(int id, Vector3 position, int _stage, PlantState _state) : base(id, position)
    {
        stage = _stage;
        state = _state;
    }

    public void SetStage(int _stage)
    {
        stage = _stage;
    }

    public void SetState(PlantState _state)
    {
        state = _state;
    }
}
