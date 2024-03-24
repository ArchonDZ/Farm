using System;
using UnityEngine;

[Serializable]
public class PlaceableData
{
    public int Id;
    public Vector3 Position;

    public PlaceableData(int id, Vector3 position)
    {
        Id = id;
        Position = position;
    }
}
