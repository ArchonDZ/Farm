using BayatGames.SaveGameFree;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class CollectionSystem : MonoBehaviour
{
    [Header("Collection Lists")]
    [SerializeField] private CollectionList collectionListSeed;

    private List<CollectibleData> collectibleDataList;
    private List<PlaceableData> placeableDataList;
    private List<CollectibleItem> collectibleItemList;
    private List<InitializableItem> initializableItemList;

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
            int collectibleDataIndex = collectibleDataList.FindIndex(x => x.Id == drops[i].CollectibleItem.Id);
            if (collectibleDataIndex != -1)
            {
                collectibleDataList[collectibleDataIndex].Count += drops[i].Count;
                GetCollectionListByType(drops[i].CollectibleItem.ItemType)?.UpdateObject(drops[i].CollectibleItem.Id);
            }
            else
            {
                CollectibleData data = new CollectibleData(drops[i].CollectibleItem.Id, drops[i].Count);
                collectibleDataList.Add(data);
                GetCollectionListByType(drops[i].CollectibleItem.ItemType)?.AddItem(new CollectiblePackage(data, drops[i].CollectibleItem));
            }
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

    [ContextMenu("Load")]
    private void Load()
    {
        LoadData();
        LoadResources();
        InitializeCollectionLists();
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
        placeableDataList = SaveGame.Load<List<PlaceableData>>("save_placeable.dat", false, "FarmOfDmitryZinovsky");
    }

    private void LoadResources()
    {
        collectibleItemList = Resources.LoadAll<CollectibleItem>("CollectibleItems").ToList();
        initializableItemList = Resources.LoadAll<InitializableItem>("InitializableItems").ToList();
    }

    private void InitializeCollectionLists()
    {
        collectibleDataList ??= new List<CollectibleData>() { new CollectibleData(1, 3) };

        for (int i = 0; i < collectibleDataList.Count; i++)
        {
            int collectibleItemIndex = collectibleItemList.FindIndex(x => x.Id == collectibleDataList[i].Id);
            if (collectibleItemIndex != -1)
            {
                GetCollectionListByType(collectibleItemList[collectibleItemIndex].ItemType)?.AddItem(new CollectiblePackage(collectibleDataList[i], collectibleItemList[collectibleItemIndex]));
            }
        }
    }

    private void InitializePlaceableObjects()
    {
        placeableDataList ??= new List<PlaceableData>();

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

    private CollectionList GetCollectionListByType(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.Seed => collectionListSeed,
            _ => null
        };
    }
}
