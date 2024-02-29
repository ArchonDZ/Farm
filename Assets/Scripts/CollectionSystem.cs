using BayatGames.SaveGameFree;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectionSystem : MonoBehaviour
{
    [Header("Collection Lists")]
    [SerializeField] private CollectionList collectionListSeed;

    private List<CollectibleData> collectibleDataList;
    private List<CollectibleItem> collectibleItemList;

    private List<CollectiblePackage> collectiblePackageSeed = new List<CollectiblePackage>();

    void Awake()
    {
        Load();
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

    [ContextMenu("Load")]
    private void Load()
    {
        collectibleDataList = SaveGame.Load<List<CollectibleData>>("save_collectible.dat", true, "FarmOfDmitryZinovsky");
        collectibleItemList = Resources.LoadAll<CollectibleItem>("CollectibleItems").ToList();
        InitializeLists();
    }

    [ContextMenu("Save")]
    private void Save()
    {
        SaveGame.Save("save_collectible.dat", collectibleDataList, true);
    }

    private CollectionList GetCollectionListByType(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.Seed => collectionListSeed,
            _ => null
        };
    }

    private void InitializeLists()
    {
        collectibleDataList ??= new List<CollectibleData>() { new CollectibleData(1, 3) };

        for (int i = 0; i < collectibleDataList.Count; i++)
        {
            int collectibleItemIndex = collectibleItemList.FindIndex(x => x.Id == collectibleDataList[i].Id);
            if (collectibleItemIndex != -1)
            {
                GetPackagesListByType(collectibleItemList[collectibleItemIndex].ItemType)?.Add(new CollectiblePackage(collectibleDataList[i], collectibleItemList[collectibleItemIndex]));
            }
        }
        collectionListSeed.Load(collectiblePackageSeed);
        collectiblePackageSeed.Clear();
    }

    private List<CollectiblePackage> GetPackagesListByType(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.Seed => collectiblePackageSeed,
            _ => null
        };
    }
}
