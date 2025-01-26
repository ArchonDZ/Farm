using System;
using UnityEngine;

[Serializable]
public struct SerializableRange
{
    [SerializeField] [Min(1)] private int start;
    [SerializeField] [Min(0)] private int length;

    public int Start => start;
    public int End => start + length;

    public SerializableRange(int start, int length)
    {
        this.start = start;
        this.length = length;
    }
}
