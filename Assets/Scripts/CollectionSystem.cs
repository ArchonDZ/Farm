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

    private void InitializeLists()
    {
        collectibleDataList ??= new List<CollectibleData>() { new CollectibleData(1, 3) };

        for (int i = 0; i < collectibleDataList.Count; i++)
        {
            int collectibleItemIndex = collectibleItemList.FindIndex(x => x.Id == collectibleDataList[i].Id);
            if (collectibleItemIndex != -1)
            {
                GetListByType(collectibleItemList[collectibleItemIndex].ItemType).Add(new CollectiblePackage(collectibleDataList[i], collectibleItemList[collectibleItemIndex]));
            }
        }
        collectionListSeed.Load(collectiblePackageSeed);
    }

    private List<CollectiblePackage> GetListByType(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.None => null,
            ItemType.Seed => collectiblePackageSeed,
            _ => null
        };
    }
}
