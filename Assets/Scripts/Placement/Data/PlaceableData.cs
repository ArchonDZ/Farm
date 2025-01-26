using System;
using UnityEngine;

[Serializable]
public class PlaceableData
{
    [SerializeField] private int id;
    [SerializeField] private Vector3 position;

    public int Id => id;
    public Vector3 Position => position;

    public PlaceableData(int _id, Vector3 _position)
    {
        id = _id;
        position = _position;
    }
}
