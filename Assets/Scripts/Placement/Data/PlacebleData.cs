using System;
using UnityEngine;

[Serializable]
public class PlacebleData
{
    public int Id;
    public Vector3 Position;

    public PlacebleData(int id, Vector3 position)
    {
        Id = id;
        Position = position;
    }
}
