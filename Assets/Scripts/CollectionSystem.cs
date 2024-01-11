using BayatGames.SaveGameFree;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectionSystem : MonoBehaviour
{
    [SerializeField] private CollectionList collectionListSeed;

    private List<CollectibleItem> collectibleItemList = new List<CollectibleItem>();
    [SerializeField] private List<CollectibleData> collectibleDataList = new List<CollectibleData>();

    private List<CollectiblePackage> collectiblePackageSeed = new List<CollectiblePackage>();

    [ContextMenu("Load")]
    public void Load()
    {
        collectibleDataList = SaveGame.Load<List<CollectibleData>>("save_collectible.dat", true, "FarmOfDmitryZinovsky");
        collectibleItemList = Resources.LoadAll<CollectibleItem>("CollectibleItems").ToList();

        for (int i = 0; i < collectibleDataList.Count; i++)
        {
            int collectibleItemIndex = collectibleItemList.FindIndex(x => x.Id == collectibleDataList[i].Id);
            if (collectibleItemIndex != -1)
            {
                switch (collectibleItemList[collectibleItemIndex].ItemType)
                {
                    case ItemType.None:
                        break;
                    case ItemType.Seed:
                        collectiblePackageSeed.Add(new CollectiblePackage(collectibleDataList[i], collectibleItemList[collectibleItemIndex]));
                        break;
                }
            }
        }
        collectionListSeed.Load(collectiblePackageSeed);
    }

    [ContextMenu("Save")]
    public void Save()
    {
        SaveGame.Save("save_collectible.dat", collectibleDataList, true);
    }
}
