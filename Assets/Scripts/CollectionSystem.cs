using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectionSystem : MonoBehaviour
{
    [SerializeField] private List<SeedItem> seedList;

    private Dictionary<SeedItem, int> dictionarySeeds = new Dictionary<SeedItem, int>();

    public List<SeedItem> SeedList { get => seedList; set => seedList = value; }

    void Awake()
    {
        seedList = Resources.LoadAll<SeedItem>("CollectibleItems/SeedItems").ToList();
    }
}
