using System.Collections.Generic;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(SidePanel))]
public class CollectionList : MonoBehaviour
{
    [SerializeField] private SidePanel sidePanel;
    [SerializeField] private CollectibleObject prefabCollectibleObject;
    [SerializeField] private Transform parent;
    [SerializeField] private CollectibleLoadConfig loadConfig;

    [Inject] private DiContainer diContainer;
    [Inject] private CollectionSystem collectionSystem;

    void Start()
    {
        collectionSystem.OnCollectibleAddEvent += OnCollectibleAddEvent;
        if (collectionSystem.TryGetCollectibleData(loadConfig, out List<CollectiblePackage> collectiblePackages))
        {
            for (int i = 0; i < collectiblePackages.Count; i++)
            {
                AddItem(collectiblePackages[i]);
            }
        }
    }

    private void OnCollectibleAddEvent(CollectiblePackage collectiblePackage)
    {
        if (collectionSystem.CheckComplianceConfig(loadConfig, collectiblePackage))
            AddItem(collectiblePackage);
    }

    private void AddItem(CollectiblePackage package)
    {
        CollectibleObject collectibleObject = diContainer.InstantiatePrefabForComponent<CollectibleObject>(prefabCollectibleObject, parent);
        collectibleObject.Initialize(package, sidePanel);
    }
}
