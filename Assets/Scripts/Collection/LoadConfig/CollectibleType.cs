using System;

[Flags]
public enum CollectibleType
{
    Common = 1 << 0,
    Seed = 1 << 1,
    Crop = 1 << 2,
}
