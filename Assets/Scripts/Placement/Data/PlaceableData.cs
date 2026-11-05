using System;
using UnityEngine;

[Serializable]
public class PlaceableData
{
	[SerializeField] private int _id;
	[SerializeField] private Vector3 _position;

	public int Id => _id;
	public Vector3 Position { get => _position; set => _position = value; }

	public PlaceableData(int id, Vector3 position)
	{
		_id = id;
		_position = position;
	}
}
