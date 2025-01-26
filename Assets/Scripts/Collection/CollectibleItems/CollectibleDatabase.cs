using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectibleDatabase", menuName = "Farm/CollectibleItem/Database", order = -1)]
public class CollectibleDatabase : ScriptableObject
{
    [SerializeField] private CollectibleType collectibleType;
    [SerializeField] private List<CollectibleItem> collectibleItemList = new List<CollectibleItem>();

    public CollectibleType CollectibleType => collectibleType;
    public List<CollectibleItem> CollectibleList => collectibleItemList;
}
