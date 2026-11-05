using R3;
using System;
using UnityEngine;

[Serializable]
public class CollectibleData
{
	[SerializeField] private int _id;
	[SerializeField] private readonly SerializableReactiveProperty<int> _count = new();

	public int Id => _id;
	public ReadOnlyReactiveProperty<int> Count => _count;

	public CollectibleData(int _id, int _count)
	{
		this._id = _id;
		this._count.Value = _count;
	}

	public void Add(int _count)
	{
		if (0 < _count)
		{
			this._count.Value += _count;
		}
	}

	public bool TrySpend(int _count)
	{
		if (0 < _count && _count <= this._count.CurrentValue)
		{
			this._count.Value -= _count;
			return true;
		}
		return false;
	}
}
