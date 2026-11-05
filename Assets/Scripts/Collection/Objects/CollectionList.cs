using System.Collections.Generic;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(SidePanel))]
public class CollectionList : MonoBehaviour
{
	[SerializeField] private SidePanel _sidePanel;
	[SerializeField] private CollectibleObject _prefabCollectibleObject;
	[SerializeField] private Transform _parent;
	[SerializeField] private CollectibleLoadConfig _loadConfig;

	[Inject] private readonly DiContainer _diContainer;
	[Inject] private readonly CollectionSystem _collectionSystem;

	void Start()
	{
		_collectionSystem.OnCollectibleAddEvent += OnCollectibleAddEvent;
		if (_collectionSystem.TryGetCollectibleData(_loadConfig, out List<CollectiblePackage> collectiblePackages))
		{
			for (int i = 0; i < collectiblePackages.Count; i++)
			{
				AddItem(collectiblePackages[i]);
			}
		}
	}

	void OnDestroy()
	{
		_collectionSystem.OnCollectibleAddEvent -= OnCollectibleAddEvent;
	}

	private void OnCollectibleAddEvent(CollectiblePackage collectiblePackage)
	{
		if (_collectionSystem.CheckComplianceConfig(_loadConfig, collectiblePackage))
			AddItem(collectiblePackage);
	}

	private void AddItem(CollectiblePackage package)
	{
		CollectibleObject collectibleObject = _diContainer.InstantiatePrefabForComponent<CollectibleObject>(_prefabCollectibleObject, _parent);
		collectibleObject.Initialize(package, _sidePanel);
	}
}
