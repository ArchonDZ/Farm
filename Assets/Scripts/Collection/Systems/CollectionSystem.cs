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
    private List<CollectibleDatabase> collectibleDatabaseList;
    private List<CollectibleItem> collectibleItems = new List<CollectibleItem>();
    private List<InitializableItem> initializableItemList;
    #endregion

    #region Data
    private List<CollectibleData> collectibleDataList;
    private List<CollectiblePackage> collectiblePackages = new List<CollectiblePackage>();
    private List<PlaceableData> placeableDataList;
    #endregion

    [Inject] private GridSystem gridSystem;

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
        if (drop.Count == 0) return;

        int collectibleDataIndex = collectibleDataList.FindIndex(x => x.Id == drop.CollectibleItem.Id);
        if (collectibleDataIndex != -1)
        {
            collectibleDataList[collectibleDataIndex].Add(drop.Count);
        }
        else
        {
            CollectibleData data = new CollectibleData(drop.CollectibleItem.Id, drop.Count);
            CollectiblePackage package = new CollectiblePackage(data, drop.CollectibleItem);
            collectibleDataList.Add(data);
            collectiblePackages.Add(package);
            OnCollectibleAddEvent?.Invoke(package);
        }
    }

    public void AddPlaceable(PlaceableData placeableData)
    {
        placeableDataList.Add(placeableData);
    }

    public void RemovePlaceable(PlaceableData placeableData)
    {
        placeableDataList.Remove(placeableData);
    }

    public bool TryGetCollectibleData(CollectibleLoadConfig loadConfig, out List<CollectiblePackage> resultCollectiblePackage)
    {
        resultCollectiblePackage = new List<CollectiblePackage>();

        List<CollectibleDatabase> databases = collectibleDatabaseList.FindAll(x => (loadConfig.FullDatabase & x.CollectibleType) > 0);
        for (int i = 0; i < databases.Count; i++)
            resultCollectiblePackage.AddRange(collectiblePackages.FindAll(x => databases[i].CollectibleList.Contains(x.CollectibleItem)));

        resultCollectiblePackage.AddRange(collectiblePackages.FindAll(x =>
            loadConfig.IDDistinct.Contains(x.CollectibleItem.Id)
        ));

        resultCollectiblePackage.AddRange(collectiblePackages.FindAll(x =>
            loadConfig.IDRanges.Exists(r => r.Start <= x.CollectibleItem.Id && x.CollectibleItem.Id < r.End)
        ));

        return resultCollectiblePackage.Count > 0;
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
        SaveGame.Save("save_collectible.dat", collectibleDataList);
        SaveGame.Save("save_placeable.dat", placeableDataList);
    }

    private void LoadData()
    {
        collectibleDataList = SaveGame.Load<List<CollectibleData>>("save_collectible.dat", false, "FarmOfDmitryZinovsky");
        collectibleDataList ??= new List<CollectibleData>() { new CollectibleData(1001, 3), new CollectibleData(1002, 3) };
        placeableDataList = SaveGame.Load<List<PlaceableData>>("save_placeable.dat", false, "FarmOfDmitryZinovsky");
        placeableDataList ??= new List<PlaceableData>();
    }

    private void LoadResources()
    {
        collectibleDatabaseList = Resources.LoadAll<CollectibleDatabase>("CollectibleItems").ToList();
        collectibleDatabaseList.ForEach(x => collectibleItems.AddRange(x.CollectibleList));
        initializableItemList = Resources.LoadAll<InitializableItem>("InitializableItems").ToList();
    }

    private void InitializeCollectionPackages()
    {
        for (int i = 0; i < collectibleDataList.Count; i++)
        {
            int collectibleItemIndex = collectibleItems.FindIndex(x => x.Id == collectibleDataList[i].Id);
            if (collectibleItemIndex != -1)
            {
                collectiblePackages.Add(new CollectiblePackage(collectibleDataList[i], collectibleItems[collectibleItemIndex]));
            }
        }
    }

    private void InitializePlaceableObjects()
    {
        for (int i = 0; i < placeableDataList.Count; i++)
        {
            int initializebleItemIndex = initializableItemList.FindIndex(x => x.Id == placeableDataList[i].Id);
            if (initializebleItemIndex != -1)
            {
                if (placeableDataList[i] is PlantPlaceableData plantPlaceableData)
                {
                    (gridSystem.InitializeObjectOnCellPosition(initializableItemList[initializebleItemIndex].InitializableObject, placeableDataList[i].Position) as Plant)?
                        .Initialize(initializableItemList[initializebleItemIndex], plantPlaceableData);
                }
            }
        }
    }
}
