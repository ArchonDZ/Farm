using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectibleDatabase", menuName = "Farm/CollectibleItem/Database", order = -1)]
public class CollectibleDatabase : ScriptableObject
{
	[SerializeField] private CollectibleType _collectibleType;
	[SerializeField] private List<CollectibleItem> _collectibleItemList = new();

	public CollectibleType CollectibleType => _collectibleType;
	public List<CollectibleItem> CollectibleList => _collectibleItemList;
}
