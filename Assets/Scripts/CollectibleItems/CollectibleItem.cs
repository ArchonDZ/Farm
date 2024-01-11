using UnityEngine;

[CreateAssetMenu(fileName = "NewCollectibleItem", menuName = "Farm/CollectibleItem")]
public class CollectibleItem : ScriptableObject
{
    public int Id;
    public ItemType ItemType;
    public string Name;
    public Sprite Icon;
}

public enum ItemType
{
    None = 0,
    Seed = 1
}