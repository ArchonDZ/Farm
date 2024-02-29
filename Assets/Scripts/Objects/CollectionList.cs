using System.Collections.Generic;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(SidePanel))]
public class CollectionList : MonoBehaviour
{
    [SerializeField] private SidePanel sidePanel;
    [SerializeField] private CollectibleObject prefabCollectibleObject;
    [SerializeField] private Transform parent;

    [Inject] private DiContainer diContainer;

    private Dictionary<int, CollectibleObject> collectibleObjects = new Dictionary<int, CollectibleObject>();

    public void Load(List<CollectiblePackage> packages)
    {
        for (int i = 0; i < packages.Count; i++)
        {
            AddNewCollectibleObject(packages[i]);
        }
    }

    public void AddItem(CollectiblePackage package)
    {
        AddNewCollectibleObject(package);
    }

    public void UpdateObject(int id)
    {
        collectibleObjects[id].UpdateObject();
    }

    private void AddNewCollectibleObject(CollectiblePackage package)
    {
        CollectibleObject collectibleObject = diContainer.InstantiatePrefabForComponent<CollectibleObject>(prefabCollectibleObject, parent);
        collectibleObject.Initialize(package, sidePanel);
        collectibleObjects.Add(package.CollectibleItem.Id, collectibleObject);
    }
}
