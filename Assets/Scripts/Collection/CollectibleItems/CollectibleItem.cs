using UnityEngine;

public abstract class CollectibleItem : ScriptableObject
{
    public int Id;
    public string Name;
    public Sprite Icon;
    public InitializableItem InitializableItem;
}