using BayatGames.SaveGameFree;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class CollectionSystem : MonoBehaviour
{
	public event Action<CollectiblePackage> OnCollectibleAddEvent;

	#region Databases
	private List<CollectibleDatabase> _collectibleDatabaseList;
	private readonly List<CollectibleItem> _collectibleItems = new();
	private List<InitializableItem> _initializableItemList;
	#endregion

	#region Data
	private List<CollectibleData> _collectibleDataList;
	private readonly List<CollectiblePackage> _collectiblePackages = new();
	private List<PlaceableData> _placeableDataList;
	#endregion

	[Inject] private readonly GridSystem _gridSystem;

	void Awake()
	{
		Load();
	}

	void OnDisable()
	{
		Save();
	}

	public void AddDrops(List<Drop> drops)
	{
		for (int i = 0; i < drops.Count; i++)
		{
			AddDrop(drops[i]);
		}
	}

	public void AddDrop(Drop drop)
	{
		if (drop == null) return;
		if (drop.CollectibleItem == null) return;

		int collectibleDataIndex = _collectibleDataList.FindIndex(x => x.Id == drop.CollectibleItem.Id);
		if (collectibleDataIndex != -1)
		{
			_collectibleDataList[collectibleDataIndex].Add(drop.Count);
		}
		else
		{
			var data = new CollectibleData(drop.CollectibleItem.Id, drop.Count);
			var package = new CollectiblePackage(data, drop.CollectibleItem);
			_collectibleDataList.Add(data);
			_collectiblePackages.Add(package);
			OnCollectibleAddEvent?.Invoke(package);
		}
	}

	public void AddPlaceable(PlaceableData placeableData)
	{
		_placeableDataList.Add(placeableData);
	}

	public void RemovePlaceable(PlaceableData placeableData)
	{
		_placeableDataList.Remove(placeableData);
	}

	public bool TryGetCollectibleData(CollectibleLoadConfig loadConfig, out List<CollectiblePackage> resultCollectiblePackage)
	{
		resultCollectiblePackage = new List<CollectiblePackage>();

		List<CollectibleDatabase> databases = _collectibleDatabaseList.FindAll(x => (loadConfig.FullDatabase & x.CollectibleType) > 0);
		for (int i = 0; i < databases.Count; i++)
			resultCollectiblePackage.AddRange(_collectiblePackages.FindAll(x => databases[i].CollectibleList.Contains(x.CollectibleItem)));

		resultCollectiblePackage.AddRange(_collectiblePackages.FindAll(x =>
			loadConfig.IDDistinct.Contains(x.CollectibleItem.Id)
		));

		resultCollectiblePackage.AddRange(_collectiblePackages.FindAll(x =>
			loadConfig.IDRanges.Exists(r => r.Start <= x.CollectibleItem.Id && x.CollectibleItem.Id < r.End)
		));

		return resultCollectiblePackage.Count > 0;
	}

	public bool CheckComplianceConfig(CollectibleLoadConfig loadConfig, CollectiblePackage collectiblePackage)
	{
		List<CollectibleDatabase> databases = _collectibleDatabaseList.FindAll(x => (loadConfig.FullDatabase & x.CollectibleType) > 0);
		for (int i = 0; i < databases.Count; i++)
			if (databases[i].CollectibleList.Contains(collectiblePackage.CollectibleItem))
				return true;

		if (loadConfig.IDDistinct.Contains(collectiblePackage.CollectibleItem.Id))
			return true;

		if (loadConfig.IDRanges.Exists(r => r.Start <= collectiblePackage.CollectibleItem.Id && collectiblePackage.CollectibleItem.Id < r.End))
			return true;

		return false;
	}

	public bool TryGetCollectibleData(CollectibleItem collectibleItem, out CollectibleData collectibleData)
	{
		CollectiblePackage package = _collectiblePackages.Find(x => x.CollectibleItem.Equals(collectibleItem));
		if (package != null)
		{
			collectibleData = package.CollectibleData;
			return true;
		}
		collectibleData = null;
		return false;
	}

	[ContextMenu("Load")]
	private void Load()
	{
		LoadData();
		LoadResources();
		InitializeCollectionPackages();
		InitializePlaceableObjects();
	}

	[ContextMenu("Save")]
	private void Save()
	{
		SaveGame.Save("save_collectible.dat", _collectibleDataList);
		SaveGame.Save("save_placeable.dat", _placeableDataList);
	}

	private void LoadData()
	{
		_collectibleDataList = SaveGame.Load<List<CollectibleData>>("save_collectible.dat", false, "FarmOfDmitryZinovsky");
		_collectibleDataList ??= new List<CollectibleData>() { new CollectibleData(1001, 3), new CollectibleData(1002, 3) };
		_placeableDataList = SaveGame.Load<List<PlaceableData>>("save_placeable.dat", false, "FarmOfDmitryZinovsky");
		_placeableDataList ??= new List<PlaceableData>();
	}

	private void LoadResources()
	{
		_collectibleDatabaseList = Resources.LoadAll<CollectibleDatabase>("CollectibleItems").ToList();
		_collectibleDatabaseList.ForEach(x => _collectibleItems.AddRange(x.CollectibleList));
		_initializableItemList = Resources.LoadAll<InitializableItem>("InitializableItems").ToList();
	}

	private void InitializeCollectionPackages()
	{
		for (int i = 0; i < _collectibleDataList.Count; i++)
		{
			int collectibleItemIndex = _collectibleItems.FindIndex(x => x.Id == _collectibleDataList[i].Id);
			if (collectibleItemIndex != -1)
			{
				_collectiblePackages.Add(new CollectiblePackage(_collectibleDataList[i], _collectibleItems[collectibleItemIndex]));
			}
		}
	}

	private void InitializePlaceableObjects()
	{
		for (int i = 0; i < _placeableDataList.Count; i++)
		{
			int initializebleItemIndex = _initializableItemList.FindIndex(x => x.Id == _placeableDataList[i].Id);
			if (initializebleItemIndex != -1)
			{
				if (_placeableDataList[i] is PlantPlaceableData plantPlaceableData)
				{
					var plant = _gridSystem.InitializeObjectOnCellPosition(_initializableItemList[initializebleItemIndex].InitializableObject, _placeableDataList[i].Position) as Plant;
					if (plant != null)
						plant.Initialize(_initializableItemList[initializebleItemIndex], plantPlaceableData);
				}
			}
		}
	}
}
