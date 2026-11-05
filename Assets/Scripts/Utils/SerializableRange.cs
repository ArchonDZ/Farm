using System;
using UnityEngine;

[Serializable]
public struct SerializableRange
{
	[SerializeField][Min(1)] private int _start;
	[SerializeField][Min(0)] private int _length;

	public readonly int Start => _start;
	public readonly int End => _start + _length;

	public SerializableRange(int start, int length)
	{
		_start = start;
		_length = length;
	}
}
